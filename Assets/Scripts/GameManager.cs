using UnityEngine;
using System;
using System.IO;


public class GameManager : MonoBehaviour
{

    // GameData Objects 
    public GameObject gameData;

    public GameObject startScreen;

    // Patch
    private PatchManager patchManager;

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

    public string pathToLog;

    private void Awake()
    {
        patchManager = GetComponent<PatchManager>();
        boxObj.SetActive(false);
        leaveStayDecisionScreen.SetActive(false);

    }


    private void InitLogFile()
    {
        DateTime currentTime = DateTime.Now;
        string fileName = currentTime.ToString("yyyy.MM.dd.HH.mm.ss");

        pathToLog = Application.persistentDataPath + "/" + fileName + ".txt";

        using StreamWriter dataOut = File.CreateText(pathToLog);
        dataOut.WriteLine("Task Initialised at: " + DateTime.Now.ToString());
        dataOut.WriteLine("Trial Number, Reward, Response Time, Response Chosen");
    }


    // need to block all input apart from clicks/ presses in game start screen
    // make a mob friendly tap as a spacebar alternative 

    private void Start()
    {
        InitLogFile();
        startScreen.SetActive(true);
    }

    public void PressedTrainingA()

    {
        using StreamWriter dataOut = File.AppendText(pathToLog);
        dataOut.WriteLine("Training A; no decisions recorded " + DateTime.Now.ToString());

        startScreen.SetActive(false);
        patchManager.StartTrainingA();

    }

    public void PressedTrainingB()
    {
        using StreamWriter dataOut = File.AppendText(pathToLog);
        dataOut.WriteLine("Training B; no decisions recorded " + DateTime.Now.ToString());

        startScreen.SetActive(false);
        patchManager.StartTrainingB();
    }

    public void PressedTrainingC()
    {
        using StreamWriter dataOut = File.AppendText(pathToLog);
        dataOut.WriteLine("Training C" + DateTime.Now.ToString());

        startScreen.SetActive(false);
        patchManager.StartTrainingC();
    }

    public void PressedRunTask()
    {
        startScreen.SetActive(false);
        patchManager.StartTask();
    }

    public void EndSession()
    {
        startScreen.SetActive(true);
    }
}