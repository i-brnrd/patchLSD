using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using System.Linq;

public class SessionManager : MonoBehaviour
{
    // GameData Objects 
    public GameObject gameData;
    private GameData patchData;
    
    // Controllers
    private TaskController taskController;
    private TrainingAController trainingAController;
    private TrainingBController trainingBController;
    private TrainingCController trainingCController;

    // Other Scripts 
    private GameManager gameManager;
    private PatchUtilities patchUtils;
    private EegStream eegStream;
    private LSD lsd;

    // GameObjects 
    public GameObject leaveStayDecisionScreen;
    
    public GameObject intertrialScreen;
    public TMP_Text optionalText;
    public TMP_Text continueText;

 
    public int trialIdx;
    public int patchIdx;

    public int[] patchOrder;
    public bool[] truncationOrder;

    public float[] rewards;
    public bool useBlueEnv = true; // in the blue (default) environment
    public bool inChoicePhase = false; //choice phase 
    public bool? leave = null; //nullable bool. Null: not decided; leave = true, left; leave = false; stay 
 

    private void Awake()
    {
        //set up references to scripts & objects
        gameManager = GetComponent<GameManager>();
        patchData = gameData.GetComponent<GameData>(); //lists of patches (patches are arrays)
        eegStream = GetComponent<EegStream>();
        lsd = leaveStayDecisionScreen.GetComponent<LSD>();

        patchUtils = GetComponent<PatchUtilities>();

        taskController = GetComponent<TaskController>();
        trainingAController = GetComponent<TrainingAController>();
        trainingBController = GetComponent<TrainingBController>();
        trainingCController = GetComponent<TrainingCController>();


    }
    // Session Flow Methods
  
    public void StartTask()
    {
        eegStream.StartLSL();
        eegStream.LogMessage("Task Started");

        SetTrialAndPatchOrder();
        StartCoroutine(RunTask());
    }

    private IEnumerator RunTask()
    {
        
        yield return StartCoroutine(taskController.RunTask());
        eegStream.StopLSL(); // stops when task is really done
    }


    private void SetTrialAndPatchOrder()
    {
        // if not resuming, set trialIdx as 0; and get a new patch & truncation order
        if (!gameManager.resumeParticipant)
        {
            trialIdx = 0;
            patchOrder = patchUtils.GeneratePatchOrder();
            truncationOrder = patchUtils.GenerateTruncations();
            WriteOutPatchOrders();

        } else
        {
            // if resuming, load in trial, patch & truncation order from saved.
            Resume();
        }
       
    }

    public void StartTrainingA()
    {
        StartCoroutine(trainingAController.RunTrainingA());
    }

    public void StartTrainingB()
    {
        StartCoroutine(trainingBController.RunTrainingB());
    }

    public void StartTrainingC()
    {
        StartCoroutine(trainingCController.RunTrainingC());
    }

    // Session Manager Methods 
    public IEnumerator ShowPointsTrial(int trialIndex, int points)
    {
        eegStream.LogPoints(points);
        yield return StartCoroutine(Intertrial($"Completed Trial {trialIndex + 1}: Bonus Points {points} Points", 0.5f));
    }

    public IEnumerator ShowAccumulatedPoints(int accumulatedPoints)
    {
        yield return StartCoroutine(Intertrial($"You've earned {accumulatedPoints} Bonus Points", 0.5f));
    }

    public int GetPoints()
    {
        float ldGoSum = patchData.ldGo[patchIdx].Sum();
        float ldStaySum = patchData.ldStay[patchIdx].Sum();

        float points;

        if (leave == true)
        {

          points = ldGoSum - ldStaySum;

        }
        else
        {
           points = ldStaySum - ldGoSum;
        }

        int pointsAsInt = Mathf.RoundToInt(points * 100);

        return pointsAsInt;
    }


    public IEnumerator EndSession(string displayMessage = null)
    {
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(Intertrial(displayMessage, 0.5f));
        yield return new WaitForSeconds(0.1f);
        gameManager.EndSession();
    }
    

    public IEnumerator Intertrial(string displayMessage = null, float delay = 0f)
    {
        if (!string.IsNullOrEmpty(displayMessage)) {
            Debug.Log(displayMessage);
            optionalText.text = displayMessage;
        } else
        {
            optionalText.text = " ";
        }
        intertrialScreen.SetActive(true);
        continueText.gameObject.SetActive(false);

        // Optional delay before showing the continueText
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        continueText.gameObject.SetActive(true);

        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
        intertrialScreen.SetActive(false);
    }


    public float[] SetPatch()
    {
        if (leave == null)
        {
            useBlueEnv = true;
            return patchData.rew2ld[patchIdx];
        }
        else if (leave == true)
        {
            useBlueEnv = false;
            return patchData.ldGo[patchIdx];
        }
        else
        {
            useBlueEnv = true;
            return patchData.ldStay[patchIdx];
        }
    }

    public void BeginChoicePhase()
    {
        inChoicePhase = true;
        leaveStayDecisionScreen.SetActive(true);
        lsd.ChoicePhase();
    }

    public void MadeLSD(bool leaveIn)
    {
        leaveStayDecisionScreen.SetActive(false);
        leave = leaveIn;
        if (leaveIn)
        {
            eegStream.LogLeaveDecision();
            leave = true;
        } else
        {
            eegStream.LogStayDecision();
        }
      
    }

    public void WriteOutLSD()
    {
        string[] choicesToWrite = { (trialIdx + 1).ToString(), (patchIdx + 1).ToString(), leave.ToString() };
        string dataToWrite = string.Join(",", choicesToWrite);
        File.AppendAllText(gameManager.pathToChoiceData, dataToWrite + "\n");
    }

    public void WriteOutRewards(float[] rewards)
    {
        string dataToWrite = string.Join(",", rewards.Select(r => r.ToString()).ToArray());
        File.AppendAllText(gameManager.pathToRew2LD, dataToWrite + "\n");
    
    }

    public void WriteState()
    {
        Debug.Log("In Write State" + trialIdx.ToString());
        File.WriteAllText(Path.Combine(gameManager.pathToFolder, "state.txt"), trialIdx.ToString());

    }

    public void WriteOutPatchOrders()
    {
        string orderString = string.Join(",", patchOrder);
        File.WriteAllText(Path.Combine(gameManager.pathToFolder, "patchOrder.txt"), orderString);
        string truncationString = string.Join(",", truncationOrder);
        File.WriteAllText(Path.Combine(gameManager.pathToFolder, "truncationOrder.txt"), truncationString);
    }


    public void Resume()
    {
        string orderString = File.ReadAllText(Path.Combine(gameManager.pathToFolder, "patchOrder.txt"));
        patchOrder = orderString.Split(',').Select(int.Parse).ToArray();

        string truncationString = File.ReadAllText(Path.Combine(gameManager.pathToFolder, "truncationOrder.txt"));
        truncationOrder = truncationString.Split(',').Select(bool.Parse).ToArray();


        string idxString = File.ReadAllText(Path.Combine(gameManager.pathToFolder, "state.txt"));
        trialIdx = int.Parse(idxString);
     
    }

}








