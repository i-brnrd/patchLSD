using System.Collections;
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

    // Temp for testing (patches)
    public float[] rewards;
    public float[] defaultPatch;

    // Environment Vats 
    private bool envB = true; // in the blue (default) environment
    // LSD vars 
    public bool inChoicePhase = false; //choice phase 
    public bool? leave = null; //nullable bool. Null: not decided; leave = true, left; leave = false; stay 

   
    private int counter = 0;

    private void Awake()
    {
        //set up references to scripts & objects 
        patchData = gameData.GetComponent<GameData>(); //lists of patches (patches are arrays)
        patch = GetComponent<Patch>();

    }

    public void StartTask()
    {
        NextTrial();
    }

 
    public void NextTrial()
    {
        trial = Random.Range(0, 89); // fix
        counter = counter + 1;
        Debug.Log(trial.ToString());
        intertrialScreen.SetActive(true);
        leave = null;
        envB = true;
        inChoicePhase = false;
        StartCoroutine(WaitForInput());
    }


    private IEnumerator WaitForInput()
    {

        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
        intertrialScreen.SetActive(false);

        NextPatch();
    }


    public void NextPatch() // in Patch Manager I hope; and we get the trial number via random selection. 
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
        Debug.Log(string.Join(", ", rewards));
        Debug.Log(rewards.Length);
        StartCoroutine(StartPatchCoroutine());
        Debug.Log("Counter " + counter.ToString());
    }

    private IEnumerator StartPatchCoroutine()
    {
        Debug.Log("In StartPatchCoroutine");

        Debug.Log(rewards);
        Debug.Log(envB);
        // Start the patch and wait until it is complete
        yield return StartCoroutine(patch.StartPatch(rewards, envB));

        Debug.Log("after StartCoroutine");

        // After the patch is complete, decide what to do next
        if (leave == null)
        {
            BeginChoicePhase();
        }
        else
        {
            NextTrial();
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
        Debug.Log("Left");
        inChoicePhase = false;
        leave = true;
        NextPatch();
    }

    public void ClickedStayLSD()
    {
        leaveStayDecisionScreen.SetActive(false);
        Debug.Log("Stayed");
        inChoicePhase = false;
        leave = false;
        NextPatch();

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






