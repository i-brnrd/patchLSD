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


    public bool inChoicePhase = false; //choice phase
    public bool? leaveDecision = null;

    public int startTrialsAt;

    public int[] patchOrder;
    public bool[] truncationOrder;

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
    // Training A, B & C 
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

    // Task Flow Methods


    // Session Utils 
    public float[] SetPatch(bool? leave, int patchIdx)
    {
        if (leave == null)
        {
            return patchData.rew2ld[patchIdx];
        }
        else if (leave == true)
        {
            return patchData.ldGo[patchIdx];
        }
        else
        {
            return patchData.ldStay[patchIdx];
        }
    }

    public int GetPoints(bool? leave, int patchIdx)
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

        int pointsInt = Mathf.RoundToInt(points * 100);

        return pointsInt;
    }

    public IEnumerator EndSession(string displayMessage = null)
    {
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(Intertrial(displayMessage, 0.5f));
        yield return new WaitForSeconds(0.1f);
        eegStream.StopLSL();
        gameManager.EndSession();
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
        leaveDecision = leaveIn;
        if (leaveDecision == true)
        {
            eegStream.LogLeaveDecision();
        }
        else
        {
            eegStream.LogStayDecision();
        }
    }


    public void StartTask()
    {
        eegStream.StartLSL();
        eegStream.LogMessage("Task Started");


        if (!gameManager.resumeParticipant)
        {
            NewParticipant();

        }
        else
        {
            // if resuming, load in trial, patch & truncation order from saved.
            ResumeParticipant();
        }

        StartCoroutine(RunTask());
    }

    private IEnumerator RunTask()
    {
        
        yield return StartCoroutine(taskController.RunTask(startTrialsAt,patchOrder, truncationOrder));

    }

    public void NewParticipant()
    {
        startTrialsAt = 0;
        patchOrder = patchUtils.GeneratePatchOrder();
        truncationOrder = patchUtils.GenerateTruncations();
        WriteOutPresentationOrders();
    }

    public void ResumeParticipant()
    {
        // read in patch order; 
        string orderString = File.ReadAllText(Path.Combine(gameManager.pathToStateFolder, "patchOrder.log"));
        patchOrder = orderString.Split(',').Select(int.Parse).ToArray();

        string truncationString = File.ReadAllText(Path.Combine(gameManager.pathToStateFolder, "truncationOrder.log"));
        truncationOrder = truncationString.Split(',').Select(bool.Parse).ToArray();


        string idxString = File.ReadAllText(Path.Combine(gameManager.pathToStateFolder, "state.log"));
        startTrialsAt = int.Parse(idxString);

    }

    public void WriteOutPresentationOrders()
    {
        string orderString = string.Join(",", patchOrder);
        File.WriteAllText(Path.Combine(gameManager.pathToStateFolder, "patchOrder.log"), orderString);

        string truncationString = string.Join(",", truncationOrder);
        File.WriteAllText(Path.Combine(gameManager.pathToStateFolder, "truncationOrder.log"), truncationString);
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

        //while (!Input.GetKeyDown(KeyCode.Space))
        //{
        //    yield return null;
        //}

        while (
    !Input.GetKeyDown(KeyCode.Space)
#if UNITY_WEBGL
    && !Input.GetMouseButtonDown(0)
#endif
)
        {
            yield return null;
        }


        intertrialScreen.SetActive(false);
    }

    // Write out Behavioural Data

    // leave stay decision 
    public void WriteOutLSD(bool? leaveDecision, int trialIdx, int patchIdx)
    {
        string[] decisionToWrite = { (trialIdx + 1).ToString(), (patchIdx + 1).ToString(), leaveDecision.ToString() };
        string dataToWrite = string.Join(",", decisionToWrite);
        string pathToDecisionFile = gameManager.pathToBehaviouralFolder + "/Choice.txt";

        File.AppendAllText(pathToDecisionFile, dataToWrite + "\n");
    }

    // rewards schedule 
    public void WriteOutRewards(bool? leaveDecision, int trialIdx, int patchIdx)
    {
        float[] rewards;
        string dataToWrite;

        // Write Out Rew2LSD
        rewards = SetPatch(null, patchIdx);
        dataToWrite = string.Join(",", rewards.Select(r => r.ToString()).ToArray());
        File.AppendAllText(gameManager.pathToBehaviouralFolder + "/RewardToLSD.txt", dataToWrite + "\n");

        // Write Out LDStay
        rewards = SetPatch(false, patchIdx);
        dataToWrite = string.Join(",", rewards.Select(r => r.ToString()).ToArray());
        File.AppendAllText(gameManager.pathToBehaviouralFolder + "/LDStay.txt", dataToWrite + "\n");

        // Write Out LDLeave
        rewards = SetPatch(true, patchIdx);
        dataToWrite = string.Join(",", rewards.Select(r => r.ToString()).ToArray());
        File.AppendAllText(gameManager.pathToBehaviouralFolder + "/LDLeave.txt", dataToWrite + "\n");


        // Write Out What They Saw Post LSD
        if (!truncationOrder[trialIdx])
        {
            rewards = SetPatch(leaveDecision, patchIdx);
        }
        else
        {
            rewards = new float[] { 0f };
        }
        dataToWrite = string.Join(",", rewards.Select(r => r.ToString()).ToArray());
        File.AppendAllText(gameManager.pathToBehaviouralFolder + "/PostLSD.txt", dataToWrite + "\n");
    }


    public void WriteState(int trialIdx)
    {
        Debug.Log("In Write State" + trialIdx.ToString());
        File.WriteAllText(Path.Combine(gameManager.pathToStateFolder, "state.log"), trialIdx.ToString());
    }

}

