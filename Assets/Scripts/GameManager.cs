using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour
{

    // GameData Objects 
    public GameObject gameData;
    private GameData patchData; // Contains lists of float arrays // cange to gameDaat and gameDataObj as we only ref that once 


    // Patch
    private PatchManager patchManager;
    private Patch patch;
    private int trial;

    // GameObjects 
    public LSD LSD;
    public GameObject boxObj; // object 
    public GameObject fixationCross;

    public GameObject leaveStayDecisionScreen;
    public GameObject intertrialScreen;

    // Temp for testing (patches)
    public float[] rewards;
    public float[] defaultPatch;

    // Envs 
    private bool envB = true; // in the blue env
    public bool inChoicePhase = false;
    public bool? leave = null; //nullable bool. Null: not decided; leave = true, left; leave = false; stay 

    private void Awake()
    {
        //set up refs to scripts 
        patchData = gameData.GetComponent<GameData>(); //lists of patches (patches are arrays)
        patch = GetComponent<Patch>();

        patchManager = GetComponent<PatchManager>();

        boxObj.SetActive(false);
        leaveStayDecisionScreen.SetActive(false);
        intertrialScreen.SetActive(true);

    }

    // idea is that PatchManager will be called upon to
    // manage the patches based upon the setting

    // I think choice phase needs to be in there too actually
    // MAanging setup of te patches and c



    // Start is called before the first frame update
    private void Start()
    {
        patchManager.StartTask();
        //NextTrial();
    }

    //public void NextTrial()
    //{
    //    trial = Random.Range(0, 89); // fix
    //    Debug.Log(trial.ToString());
    //    intertrialScreen.SetActive(true);
    //    leave = null;
    //    envB = true;
    //    inChoicePhase = false;
    //    StartCoroutine(WaitForInput());
    //}

    
    //private IEnumerator WaitForInput()
    //{
    //    while (!Input.GetKeyDown(KeyCode.Space))
    //    {
    //        yield return null;
    //    }
    //    intertrialScreen.SetActive(false);
    //    NextPatch();
    //}

    //public void BeginChoicePhase()
    //{
    //    inChoicePhase = true;
    //    leaveStayDecisionScreen.SetActive(true);
    //    LSD.ChoicePhase();
    //}

    //public void ClickedLeaveLSD()
    //{
    //    leaveStayDecisionScreen.SetActive(false);
    //    Debug.Log("Left");
    //    inChoicePhase = false;
    //    leave = true;
    //    NextPatch();
    //}

    //public void ClickedStayLSD()
    //{
    //    leaveStayDecisionScreen.SetActive(false);
    //    Debug.Log("Stayed");
    //    inChoicePhase = false;
    //    leave = false;
    //    NextPatch();

    //}


    //public void NextPatch() // in Patch Manager I hope; and we get the trial number via random selection. 
    //{
    //    if (leave== null) {
    //        envB = true;
    //        rewards = patchData.rew2ld[trial];
    //    } else if (leave == true)
    //    {
    //        envB = false;
    //        rewards = patchData.ldGo[trial];
    //    } else if (leave == false)
    //    {
    //        envB = true;
    //        rewards = patchData.ldStay[trial];
    //    }
    //    Debug.Log(string.Join(", ", rewards));
    //    patch.StartPatch(rewards, envB);
    //}

}