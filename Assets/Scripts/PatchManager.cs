using System.Collections;
using TMPro;
using UnityEngine;
using System.Linq;

public class PatchManager : MonoBehaviour
{
    // GameData Objects 
    public GameObject gameData;
    public GameData patchData;
    public int trial;


    // Controllers
    private TaskController taskController; 

    // Patch
    private Patch patch;
    private LSLStream lslStream;

    // GameObjects 
    public LSD LSD;
    public TrainingAFeedback TrainingAFeedback;

    public GameObject boxObj; // object
    public GameObject trainingAFeedbackScreen;
    public GameObject fixationCross;
    public GameObject leaveStayDecisionScreen;
    
    public GameObject intertrialScreen;
    public TMP_Text optionalText;
    public TMP_Text continueText;

    private GameManager gameManager;

    private PatchUtilities patchUtils;

    public float[] rewards;
    public bool useBlueEnv = true; // in the blue (default) environment
    public bool inChoicePhase = false; //choice phase 
    public bool? leave = null; //nullable bool. Null: not decided; leave = true, left; leave = false; stay 
    private bool training;



    private void Awake()
    {
        //set up references to scripts & objects
        gameManager = GetComponent<GameManager>();

        patchData = gameData.GetComponent<GameData>(); //lists of patches (patches are arrays)
        patch = GetComponent<Patch>();

        patchUtils = GetComponent<PatchUtilities>();

        lslStream = GetComponent<LSLStream>();

        taskController = GetComponent<TaskController>();


    }

    public void StartTask()
    {

        // START THE EEG STREM AND STOP
        //aslo be very clear that training = false
        // call it inTraining? tbh though events might deal with this better,
        // but tbh probs not 
        StartCoroutine(taskController.ExecuteTask());
        //StartCoroutine(Task
        //// stope the eeg stream 
    }

    public void StartTrainingA()
    {
        StartCoroutine(TrainingA());
    }

    private IEnumerator TrainingA()
    {
        int count = 0;

        int[] trialsA = { 18, 42, 86, 27, 65, 39, 76, 4, 13, 53 }; //matlab 1 based indexing
#if UNITY_STANDALONE_WIN
        lslStream.StreamStart(); // start lsl stream on task
        lslStream.TriggerLSLEvent("Starting Training A");
#endif

        training = true;

        while (count < trialsA.Length)
        {
            if (count == 0)
            {
                yield return StartCoroutine(Intertrial("Start of Training (A)", 0.5f));
            } else
            {
                yield return StartCoroutine(Intertrial());
            }

            trial = trialsA[count] -1; //c# 0 based 
            
            leave = true;
            SetPatch();
            yield return StartCoroutine(patch.StartPatchTraining(rewards, useBlueEnv));
            yield return StartCoroutine(Intertrial());
           
            leave = null;
            SetPatch();
            yield return StartCoroutine(patch.StartPatchTraining(rewards, useBlueEnv));

            inChoicePhase = true;
            trainingAFeedbackScreen.SetActive(true);
            yield return StartCoroutine(TrainingAFeedback.Choice());
            while (inChoicePhase == true)
            {
                yield return null;
            }

            // training A choice has now returned a choice:
            // give feedback on their decision 
            Debug.Log("Chose red? "+ leave.ToString());

            float redSum = patchData.ldGo[trial].Sum();
            float blueSum = patchData.rew2ld[trial].Sum();
            
            if ((redSum > blueSum & (leave ?? false)) || (blueSum > redSum & (!leave ?? false)))
            {
                yield return StartCoroutine(Intertrial("You made the correct decision"));
            }
            else
            {
                yield return StartCoroutine(Intertrial("You made the wrong decision"));
            }
            
            count++;
        }

        StartCoroutine(EndSession("End of Training (A)"));

    }

    public void StartTrainingB()
    {
        StartCoroutine(TrainingB());
    }


    private IEnumerator TrainingB()
    {
        int[] trialsB = { 7, 41, 81};

        int nSets = 3;
        int setCount = 0;

        training = true;

        while (setCount < nSets)
        {
            yield return StartCoroutine(Intertrial($"Start of Training (B) Set {setCount + 1}"));

            for (int i = 0; i < trialsB.Length; i++)
            {
                if (i != 0)
                {
                    yield return StartCoroutine(Intertrial("End of Patch", 0.5f));  // Regular intertrial
                }
                
                trial = trialsB[i]-1;
            
                leave = null;
                SetPatch();
                yield return StartCoroutine(patch.StartPatchTraining(rewards, useBlueEnv));
                leave = false;
                SetPatch();
                yield return StartCoroutine(patch.StartPatchTraining(rewards, useBlueEnv));
     
            }

            if (setCount < nSets - 1)
            {
                yield return StartCoroutine(Intertrial($"Set {setCount + 1} Complete"));
            }

            setCount++; // Increment the set counter
        }
        StartCoroutine(EndSession("End of Training (B)"));
    }

    public void StartTrainingC()
    {
        StartCoroutine(TrainingC());
    }


    private IEnumerator TrainingC()
    {
        int count = 0;
        int[] trialsC = { 55, 80, 20, 40, 60, 0, 35, 15, 10, 70, 5, 50, 85, 45, 25, 75, 30, 65 };
        int points = 0;

        training = true;

        while (count< trialsC.Length)
        {
            if (count == 0)
            {
                yield return StartCoroutine(Intertrial("Start of Training (C)", 0.5f));
            }
            else
            {
                yield return StartCoroutine(Intertrial());
            }
            trial = Random.Range(0, 89); 
            //trial = trialsC[count];
            leave = null;
            SetPatch(); // takes in leave & returns env
            Debug.Log(useBlueEnv.ToString() + trial.ToString() + count.ToString());
            yield return StartCoroutine(patch.StartPatchTraining(rewards, useBlueEnv));

            // After the patch is complete, decide what to do next
            if (leave == null)
            {
                BeginChoicePhase();

                while (leave == null)
                {
                    yield return null;
                }

            }
            SetPatch();
            yield return StartCoroutine(patch.StartPatchTraining(rewards, useBlueEnv));
            points = GetPoints();
            yield return StartCoroutine(ShowPointsTrial(count, points));
            count++;

        }
        StartCoroutine(EndSession("End of Training (C)"));
    }


    // Patch Manager Utils (TODO: move to patchUtils; keep patterns here AND/OR break up patterns)

    public IEnumerator ShowPointsTrial(int count, int points)
    {
#if UNITY_STANDALONE_WIN
        lslStream.TriggerLSLEvent(points.ToString());
#endif
        // Return the sum of both arrays and display points in the message
        yield return StartCoroutine(Intertrial($"Completed Trial {count + 1}: Bonus Points {points} Points", 0.5f));
    }

    public IEnumerator ShowAccumulatedPoints(int accumulatedPoints)
    {
        // Return the sum of both arrays and display points in the message
        yield return StartCoroutine(Intertrial($"You've earned {accumulatedPoints} Bonus Points", 0.5f));
    }


    public int GetPoints()
    {
        float ldGoSum = patchData.ldGo[trial].Sum();
        float ldStaySum = patchData.ldStay[trial].Sum();

        float points = 0.0f;

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

    public void SetPatch()
    {
        if (leave == null)
        {
            useBlueEnv = true;
            rewards = patchData.rew2ld[trial];
        }
        else if (leave == true)
        {
            useBlueEnv = false;
            rewards = patchData.ldGo[trial];
        }
        else if (leave == false)
        {
            useBlueEnv = true;
            rewards = patchData.ldStay[trial];
        }

    }

    public float[] SetPatchArg(bool? leave)
    {
        if (leave == null)
        {
            useBlueEnv = true;
            return patchData.rew2ld[trial];
        }
        else if (leave == true)
        {
            useBlueEnv = false;
            return patchData.ldGo[trial];
        }
        else
        {
            useBlueEnv = true;
            return patchData.ldStay[trial];
        }
    }

    public void BeginChoicePhase()
    {
        if (!training)
        {
#if UNITY_STANDALONE_WIN
            lslStream.TriggerLSLEvent("7");
#endif
        }
        inChoicePhase = true;
        leaveStayDecisionScreen.SetActive(true);
        LSD.ChoicePhase();
    }


    public void ClickedLeaveLSD()
    {
#if UNITY_STANDALONE_WIN
        lslStream.TriggerLSLEvent("40000");
#endif
        Debug.Log("Chose Leave in LSD");
        leaveStayDecisionScreen.SetActive(false);
        leave = true;
    }

    public void ClickedStayLSD()
    {
        Debug.Log("Chose Stay in LSD");

#if UNITY_STANDALONE_WIN
        lslStream.TriggerLSLEvent("30000");
#endif
        leaveStayDecisionScreen.SetActive(false);
        leave = false;
    }

    
    public void TrainingAChoice(bool red)
    {
        Debug.Log("Training A Chose leave?: " + red.ToString());
        trainingAFeedbackScreen.SetActive(false);
        inChoicePhase = false;
        leave = red;
    }


    public void WriteOutData()
    {
        // included to avoid using
        string[] choiceToWrite = { trial.ToString(), leave.ToString() };
        gameManager.SaveData(choiceToWrite);
        // actual 

    }



}








