using System.Collections;
using TMPro;
using UnityEngine;

public class PatchManager : MonoBehaviour
{
    // GameData Objects 
    public GameObject gameData;
    private GameData patchData;
    private int trial;

    // Patch
    private Patch patch;

    // GameObjects 
    public LSD LSD;
    public GameObject boxObj; // object 
    public GameObject fixationCross;
    public GameObject leaveStayDecisionScreen;
    public GameObject intertrialScreen;
    public TMP_Text optionalText;

    private GameManager gameManager;
   

    public float[] rewards;
    private bool envB = true; // in the blue (default) environment
    public bool inChoicePhase = false; //choice phase 
    public bool? leave = null; //nullable bool. Null: not decided; leave = true, left; leave = false; stay 


    private void Awake()
    {
        //set up references to scripts & objects 
        patchData = gameData.GetComponent<GameData>(); //lists of patches (patches are arrays)
        patch = GetComponent<Patch>();
        gameManager = GetComponent<GameManager>();
    }

    public void StartTask()
    {
        StartCoroutine(Task());
    }

    private IEnumerator Task()
    {
        int count = 0;
        yield return StartCoroutine(Intertrial("Start of Task"));
        while (count < 90) {
            leave = null; 
            trial = Random.Range(0, 89); // fix
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
            yield return StartCoroutine(Intertrial("Completed Trial " + (count+1).ToString()));
            count++;
        
        }
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(Intertrial("End of Task"));
        yield return new WaitForSeconds(0.1f);
        gameManager.EndSession();


    }

    public void StartTrainingA()
    {
        StartCoroutine(TrainingA());
    }

    private IEnumerator TrainingA()
    {
        int count = 0;

        while (count < 1)
        {
            if (count == 0)
            {
                yield return StartCoroutine(Intertrial("Start of Training (A)"));
            } else
            {
                yield return StartCoroutine(Intertrial());
            }
            trial = Random.Range(0, 89); // fix

            leave = null;
            SetPatch();
            yield return StartCoroutine(patch.StartPatch(rewards, envB));
            yield return StartCoroutine(Intertrial());
            leave = true;
            SetPatch();
            yield return StartCoroutine(patch.StartPatch(rewards, envB));
           
            count++;
        }

        Debug.Log("end");
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(Intertrial("End of Training (A)"));
        yield return new WaitForSeconds(0.1f);
        gameManager.EndSession();

    }

    public void StartTrainingB()
    {
        StartCoroutine(TrainingB());
    }


    private IEnumerator TrainingB()
    {
        int count = 0;

        while (count < 10)
        {
            if (count == 0)
            {
                yield return StartCoroutine(Intertrial("Start of Training (B)"));
            }
            else
            {
                yield return StartCoroutine(Intertrial());
            }
            trial = Random.Range(0, 89); // fix

            leave = null;
            SetPatch();
            yield return StartCoroutine(patch.StartPatch(rewards, envB));
            yield return StartCoroutine(Intertrial());
            leave = true;
            SetPatch();
            yield return StartCoroutine(patch.StartPatch(rewards, envB));
            count++;
        }
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(Intertrial("End of Training (B)"));
        yield return new WaitForSeconds(0.1f);
        gameManager.EndSession();

    }

    public void StartTrainingC()
    {
        StartCoroutine(TrainingC());
    }


    private IEnumerator TrainingC()
    {
        int count = 0;

        while (count < 10)
        {
            if (count == 0)
            {
                yield return StartCoroutine(Intertrial("Start of Training (C)"));
            }
            else
            {
                yield return StartCoroutine(Intertrial());
            }
            trial = Random.Range(0, 89); // fix

            leave = null;
            SetPatch();
            yield return StartCoroutine(patch.StartPatch(rewards, envB));
            yield return StartCoroutine(Intertrial());
            leave = true;
            SetPatch();
            yield return StartCoroutine(patch.StartPatch(rewards, envB));
            yield return StartCoroutine(Intertrial());
            count++;
        }
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(Intertrial("End of Training (C)"));
        yield return new WaitForSeconds(0.1f);
        gameManager.EndSession();

    }


    // Patch Manager Utils
    

    private IEnumerator Intertrial(string displayMessage = null)
    {
        if (!string.IsNullOrEmpty(displayMessage)) {
            Debug.Log(displayMessage);
            optionalText.text = displayMessage;
        } else
        {
            optionalText.text = " ";
        }
        intertrialScreen.SetActive(true);
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
        leaveStayDecisionScreen.SetActive(false);
        leave = true;
    }

    public void ClickedStayLSD()
    {
        leaveStayDecisionScreen.SetActive(false);
        leave = false;
    }  

}








