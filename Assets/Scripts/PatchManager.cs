using System.Collections;
using TMPro;
using UnityEngine;
using System.Linq;

public class PatchManager : MonoBehaviour
{
    // This script is too long and chaotic
    // GameData Objects 
    public GameObject gameData;
    private GameData patchData;
    private int trial;

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
    private bool envB = true; // in the blue (default) environment
    public bool inChoicePhase = false; //choice phase 
    public bool? leave = null; //nullable bool. Null: not decided; leave = true, left; leave = false; stay 


    private void Awake()
    {
        //set up references to scripts & objects
        gameManager = GetComponent<GameManager>();

        patchData = gameData.GetComponent<GameData>(); //lists of patches (patches are arrays)
        patch = GetComponent<Patch>();

        patchUtils = GetComponent<PatchUtilities>();

        lslStream = GetComponent<LSLStream>();


    }

    public void StartTask()
    {
        StartCoroutine(Task());
    }

    private IEnumerator Task()
    {
        int count = 0;
        int nCounts = 90;
        int points;
        int accumulatedPoints = 0;
        bool truncate = true;

        int[] pointsFeedback = { Mathf.FloorToInt(nCounts / 4), Mathf.FloorToInt(nCounts / 2), Mathf.FloorToInt((nCounts * 3) / 4) };

#if UNITY_STANDALONE_WIN
        lslStream.StreamStart(); // start lsl stream on task
#endif

        yield return StartCoroutine(Intertrial("Starting Task"));
#if UNITY_STANDALONE_WIN
        lslStream.TriggerLSLEvent("Starting Task");
#endif
        while (count < nCounts) {
            leave = null;
            trial = patchUtils.GetTrial();
            SetPatch(); // takes in leave & returns env
#if UNITY_STANDALONE_WIN
            lslStream.TriggerLSLEvent("Start: Round " + count.ToString() + " of 90; Trial: " + trial.ToString());
#endif

            yield return StartCoroutine(patch.StartPatch(rewards, envB));

            if (leave == null)
            {
                BeginChoicePhase();
                while (leave == null)
                {
                    yield return null;
                }
            }
    
            string[]  choiceToWrite = { trial.ToString(), leave.ToString()};
            gameManager.SaveData(choiceToWrite);
            

            truncate = count % 3 != 0;

            if (!truncate)
            {
                SetPatch();
#if UNITY_STANDALONE_WIN
                lslStream.TriggerLSLEvent("Showing post-choice env; leave?: " + leave.ToString());
#endif
                yield return StartCoroutine(patch.StartPatch(rewards, envB));
            }

            points = GetPoints(trial, leave);
            accumulatedPoints += points;
            yield return StartCoroutine(ShowPointsTrial(count, points));
         

            if (pointsFeedback.Contains(count))
            {
                yield return StartCoroutine(ShowAccumulatedPoints(accumulatedPoints));
            }
#if UNITY_STANDALONE_WIN
            lslStream.TriggerLSLEvent("End: Round " + count.ToString() + " of 90; Trial: " + trial.ToString());
#endif

            count++;
        }
#if UNITY_STANDALONE_WIN
        lslStream.StreamStop(); // stop lsl stream
#endif
        StartCoroutine(EndSession("End of Task"));
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
            yield return StartCoroutine(patch.StartPatch(rewards, envB));
            yield return StartCoroutine(Intertrial());
           
            leave = null;
            SetPatch();
            yield return StartCoroutine(patch.StartPatch(rewards, envB));

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
                yield return StartCoroutine(patch.StartPatch(rewards, envB));
                leave = false;
                SetPatch();
                yield return StartCoroutine(patch.StartPatch(rewards, envB));
     
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
            Debug.Log(envB.ToString() + trial.ToString() + count.ToString());
            yield return StartCoroutine(patch.StartPatch(rewards, envB));

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
            yield return StartCoroutine(patch.StartPatch(rewards, envB));
            points = GetPoints(trial, leave);
            yield return StartCoroutine(ShowPointsTrial(count, points));
            count++;

        }
        StartCoroutine(EndSession("End of Training (C)"));
    }


    // Patch Manager Utils (TODO: move to patchUtils; keep patterns here AND/OR break up patterns)

    private IEnumerator ShowPointsTrial(int count, int points)
    {
        // Return the sum of both arrays and display points in the message
        yield return StartCoroutine(Intertrial($"Completed Trial {count + 1}: Bonus Points {points} Points", 0.5f));
    }

    private IEnumerator ShowAccumulatedPoints(int accumulatedPoints)
    {
        // Return the sum of both arrays and display points in the message
        yield return StartCoroutine(Intertrial($"You've earned {accumulatedPoints} Bonus Points", 0.5f));
    }


    private int GetPoints(int trial, bool? leave)
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


    private IEnumerator EndSession(string displayMessage = null)
    {
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(Intertrial(displayMessage, 0.5f));
        yield return new WaitForSeconds(0.1f);
        gameManager.EndSession();
    }
    

    private IEnumerator Intertrial(string displayMessage = null, float delay = 0f)
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
            envB = true;
            rewards = patchData.rew2ld[trial];
        }
        else if (leave == true)
        {
            envB = false;
            rewards = patchData.ldGo[trial];
        }
        else if (leave == false)
        {
            envB = true;
            rewards = patchData.ldStay[trial];
        }

    }

    public void BeginChoicePhase()
    {
        inChoicePhase = true;
        leaveStayDecisionScreen.SetActive(true);
        LSD.ChoicePhase();
    }

    public void ClickedLeaveLSD()
    {
        Debug.Log("Chose Leave in LSD");
        leaveStayDecisionScreen.SetActive(false);
        leave = true;
    }

    public void ClickedStayLSD()
    {
        Debug.Log("Chose Stay in LSD");
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

}








