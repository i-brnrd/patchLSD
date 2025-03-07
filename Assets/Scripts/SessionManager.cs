using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using System.Linq;

public class SessionManager : MonoBehaviour
{
    // Manager Objects with Associated Scripts

    public GameObject gameManagerObject;
    private GameManager gameManager;

    public GameObject gameDataObject;
    private RewardData patchData;
    private BehaviouralDataIO behaviouralData;


    public GameObject eegObject;
    private EegStream eegStream;

    public GameObject leaveStayDecisionScreen;
    private LSD lsd;

    public GameObject patchObject;
    private PatchUtilities patchUtils;

    // Controllers

    public GameObject taskObject;
    private TaskController taskController;

    public GameObject trainingObject;
    private TrainingAController trainingAController;
    private TrainingBController trainingBController;
    private TrainingCController trainingCController;

    // GameObjects (screens, tmpro)
  
    public GameObject intertrialScreen;
    public GameObject endTaskScreen;

    public TMP_Text optionalText;
    public TMP_Text continueText;

    //vars

    public bool inChoicePhase = false; 
    public bool? leaveDecision = null;

    public int startTrialsAt;
    public int[] patchOrder;
    public bool[] truncationOrder;

    // PATC STATE.log etc read in? 

 
    private void Awake()
    {
        //set up references to scripts 
        gameManager = gameManagerObject.GetComponent<GameManager>();
        patchData = gameDataObject.GetComponent<RewardData>(); //lists of patches (patches are arrays)
        behaviouralData = gameDataObject.GetComponent<BehaviouralDataIO>();

        eegStream = eegObject.GetComponent<EegStream>();
        lsd = leaveStayDecisionScreen.GetComponent<LSD>();
        patchUtils = patchObject.GetComponent<PatchUtilities>();
        taskController = taskObject.GetComponent<TaskController>();

        trainingAController = trainingObject.GetComponent<TrainingAController>();
        trainingBController = trainingObject.GetComponent<TrainingBController>();
        trainingCController = trainingObject.GetComponent<TrainingCController>();

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
    public void StartTask()
    {
        eegStream.StartLSL();

        if (!gameManager.resumeParticipant)
        {
            NewParticipant();

        }
        else
        {
            ResumeParticipant();
        }

        StartCoroutine(RunTask());
    }

    private IEnumerator RunTask()
    {

        yield return StartCoroutine(taskController.RunTask(startTrialsAt, patchOrder, truncationOrder));

    }

    public void NewParticipant()
    {
        startTrialsAt = 0;
        patchOrder = patchUtils.GeneratePatchOrder();
        truncationOrder = patchUtils.GenerateTruncations();
        behaviouralData.WriteOutPresentationOrders();
    }

    public void ResumeParticipant()
    {
        behaviouralData.ReadInPresentationOrders(); // sets starts trials at, patchOrder & truncationOrder from last recorded. 
    }

    // Session Manager Methods 
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

    public IEnumerator EndTrainingSession(string displayMessage = null)
    {
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(Intertrial(displayMessage, 0.5f));
        yield return new WaitForSeconds(0.1f);
        gameManager.EndSession();
    }

    public void EndTask()
    {
        eegStream.StopLSL();
        endTaskScreen.SetActive(true);
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

        // if WebGL; tell them they can tap/ mouseclick
#if UNITY_WEBGL
        continueText.text = "Press Spacebar or tap to continue";
        continueText.fontSize = 100;
#endif

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
   

}

