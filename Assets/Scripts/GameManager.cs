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

    public GameObject startScreen;

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
        patchManager = GetComponent<PatchManager>();
        boxObj.SetActive(false);
        leaveStayDecisionScreen.SetActive(false);

    }

    // need to block all input apart from clicks/ presses in game start screen
    // make a mob friendly tap as a spacebar alternative 

    private void Start()
    {
        startScreen.SetActive(true);
    }

    public void PressedTrainingA()
    {
        startScreen.SetActive(false);
        Debug.Log("Pressed A");
        patchManager.StartTrainingA();

    }

    public void PressedTrainingB()
    {
        startScreen.SetActive(false);
        Debug.Log("Pressed B");
        patchManager.StartTrainingB();
    }

    public void PressedTrainingC()
    {
        startScreen.SetActive(false);
        Debug.Log("Pressed C");
        patchManager.StartTrainingC();
    }

    public void PressedRunTask()
    {
        startScreen.SetActive(false);
        Debug.Log("Pressed Run Task");
        patchManager.StartTask();
    }

    public void EndSession()
    {
        startScreen.SetActive(true);
    }
}