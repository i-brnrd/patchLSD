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

    public float[] rewards;
    private bool envB = true; // in the blue (default) environment
    // LSD vars 
    public bool inChoicePhase = false; //choice phase 
    public bool? leave = null; //nullable bool. Null: not decided; leave = true, left; leave = false; stay 


    private void Awake()
    {
        //set up references to scripts & objects 
        patchData = gameData.GetComponent<GameData>(); //lists of patches (patches are arrays)
        patch = GetComponent<Patch>();

    }

    public void StartTrainingA()
    {
        Debug.Log("training A");
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
            yield return StartCoroutine(Intertrial("Completed Trial " + count.ToString()));
            count++;
        
        }

    }

    private IEnumerator Intertrial(string displayMessage = null)
    {
        if (!string.IsNullOrEmpty(displayMessage)) {
            Debug.Log(displayMessage);
            optionalText.text = displayMessage;
        } else
        {
            optionalText.text = " ";
        }
        Debug.Log(optionalText);
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


////these will all be public 
//private int[,] patchSets;
//private bool[,] isUsed;

//private int nSets = 18;
//private int nPatches = 5; //per set 

//// deal with this, is important 
//private int availableSets = 5;

//public void InitPatchIdx()
//{
//    // load in the actual patches elsewhere
//    // patchSets keeps track of the Row Number of each Patch in the correct index. 
//    for (int i = 0; i < nSets; i++)
//    {
//        for (int j = 0; j < nPatches; j++)
//        {
//            patchSets[i, j] = i * nSets + j;
//            isUsed[i, j] = false;
//        }
//    }


//}


//public void GetNextPatch()
//{
//    // for now make it actually random

//}

//public void StartTraining()
//{
//    Debug.Log("hello I am a training");
//    // whats a training patch going to look like 
//    // get a training patch & run it
//    // end of blue patch 
//    // get a defaul patch and run it
//    // end of red patch 
//    // repeat that like 3 times
//    // training completed - I think i need a Show Message thing 

//}






