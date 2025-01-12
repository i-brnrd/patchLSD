using UnityEngine;
using System;
using System.IO;


public class GameManager : MonoBehaviour
{

    // GameData Objects 
    public GameObject gameData;
    public GameObject startScreen;

    // Patch
    private SessionManager sessionManager;

    // GameObjects 
    public LSD LSD;

    public GameObject boxObj; // object 
    public GameObject fixationCross;

    public GameObject leaveStayDecisionScreen;
    public GameObject trainingAFeedbackScreen;
    public GameObject intertrialScreen;


    public bool eegFlag = false; 
    
    // Path To Log
    public string pathToLog;

    private void Awake()
    {
        sessionManager = GetComponent<SessionManager>();
        boxObj.SetActive(false);
        leaveStayDecisionScreen.SetActive(false);
        trainingAFeedbackScreen.SetActive(false);
}
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Esc Key pressed");
        }
    }


    private void InitLogFile()
    {
        DateTime currentTime = DateTime.Now;
        string fileName = currentTime.ToString("yyyy.MM.dd.HH.mm.ss");

        pathToLog = Application.persistentDataPath + "/" + fileName + ".txt";

        Debug.Log("Path to log file:  " + pathToLog);

        using StreamWriter dataOut = File.CreateText(pathToLog);
        dataOut.WriteLine("Task Initialised at: " + DateTime.Now.ToString());
        dataOut.WriteLine("Trial, Leave?(bool) - anything else?");
    }


    // need to block all input apart from clicks/ presses in game start screen
    // make a mob friendly tap as a spacebar alternative 

    // if WebGL then set eegOptions as unavaiable

    // log files init: HOW. eeg HOW. 

    private void Start()
    {
        InitLogFile();
        startScreen.SetActive(true);
    }

    public void PressedTrainingA()

    {
        startScreen.SetActive(false);
        sessionManager.StartTrainingA();
    }

    public void PressedTrainingB()
    {
        startScreen.SetActive(false);
        sessionManager.StartTrainingB();
    }

    public void PressedTrainingC()
    {
        startScreen.SetActive(false);
        sessionManager.StartTrainingC();
    }

    public void PressedRunTask()
    {
        startScreen.SetActive(false);
        sessionManager.StartTask();
    }

    public void EndSession()
    {
        startScreen.SetActive(true);
    }


    public void SaveData(string[] stringToSave)
    {
        string dataToWrite = string.Join(",", stringToSave);
        Debug.Log("Writing out: " + dataToWrite);
        using StreamWriter dataOut = File.AppendText(pathToLog);
        dataOut.WriteLine(dataToWrite);
    }

}