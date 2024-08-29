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
    public bool inChoicePhase = false;
    public bool? leave = null; //nullable bool. Null: not decided; leave = true, left; leave = false; stay 

    private void Awake()
    {
        //set up refs to scripts 
        //patchData = gameData.GetComponent<GameData>(); //lists of patches (patches are arrays)
        //patch = GetComponent<Patch>();

        patchManager = GetComponent<PatchManager>();

        boxObj.SetActive(false);
        leaveStayDecisionScreen.SetActive(false);
        intertrialScreen.SetActive(true);

    }


    private void Start()
    {
        patchManager.StartTask();
        //patchManager.StartTrainingA();
    }

  
    

}