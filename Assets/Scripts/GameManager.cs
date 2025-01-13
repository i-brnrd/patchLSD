using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using TMPro;


public class GameManager : MonoBehaviour
{

    // GameData Objects 
    public GameObject gameData;
    public GameObject startScreen;

    // Patch
    private SessionManager sessionManager;

    // Mode Toggles
    public Toggle trainingToggle;
    public Toggle playToggle;

    // GameObjects 
    public LSD LSD;

    public GameObject boxObj; // object 
    public GameObject fixationCross;

    public GameObject leaveStayDecisionScreen;
    public GameObject trainingAFeedbackScreen;
    public GameObject intertrialScreen;

    public GameObject trainingInputs;
    public GameObject playInputs;

    public Button taskButton;
    public Toggle eegToggle;
    public TMP_InputField participantID;
    public Toggle resumeToggle;


    public bool eegStreamOn = false;
    public bool resumeParticipant = false;

    // Path To Logs
    public string pathToFolder;
    public string pathToChoiceData;
    public string pathToRew2LD;

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

    // need to block all input apart from clicks/ presses in game start screen
    // make a mob friendly tap as a spacebar alternative 

    // if WebGL then set eegOptions as unavaiable

    // log files init: HOW. eeg HOW. 

    private void Start()
    {
        ToggleTrainingInputs(true);
        TogglePlayInputs(false);
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
        if (eegToggle.isOn)
        {
            eegStreamOn = true;
        }
        if (resumeToggle.isOn)
        {
            resumeParticipant = true;
        }

        ParticipantFolder();

        InitLogs();
        startScreen.SetActive(false);
        sessionManager.StartTask();
    }

    public void EndSession()
    {
        startScreen.SetActive(true);
    }

    // will want to smarten up and make sure all inactive at start 


    private void ParticipantFolder()
    {
        string folderName; 
        if (string.IsNullOrWhiteSpace(participantID.text))
        {
            // If it's empty we will use a datetime UUID 
            folderName = string.Format(@"{0}.txt", DateTime.Now.Ticks);
        }
        else
        {
            folderName = participantID.text;
        }

        pathToFolder = Path.Combine(Application.persistentDataPath, folderName);

        if (!Directory.Exists(pathToFolder))
        {
            Directory.CreateDirectory(pathToFolder);
        }

        Debug.Log("Path to patient folder " + pathToFolder);
    }


    //public void SaveData(string[] stringToSave)
    //{
    //    string dataToWrite = string.Join(",", stringToSave);
    //    Debug.Log("Writing out: " + dataToWrite);
    //    using StreamWriter dataOut = File.AppendText(pathToChoiceData);
    //    dataOut.WriteLine(dataToWrite);
    //}

    private void InitLogs()
    {

        DateTime currentTime = DateTime.Now;
        string timeStamp = currentTime.ToString("yyyy.MM.dd.HH.mm.ss");

        string choiceDataFileName = "ChoiceData_" + timeStamp + ".txt";
        string rew2ldFileName = "Rew2Ld_" + timeStamp + ".txt";

        pathToChoiceData = Path.Combine(pathToFolder, choiceDataFileName);
        pathToRew2LD = Path.Combine(pathToFolder, rew2ldFileName);

        string choiceDataHeader = "Task Initialised/ Resumed at: " + currentTime + "\n" + "Trial, Leave?(bool)";

        string rew2ldHeader ="Task Initialised/ Resumed at: " + currentTime + "\n" +"Rewards (the given patch) to Leave/Stay Decision";

        File.WriteAllText(pathToChoiceData, choiceDataHeader);
        File.WriteAllText(pathToRew2LD, rew2ldHeader);
    }

    // Setting interactable status for Play Modes 
    public void ToggleTrainingInputs(bool isOn)
    {
        Button[] trainingButtons = trainingInputs.GetComponentsInChildren<Button>();
        foreach (Button btn in trainingButtons)
        {
            btn.interactable = isOn;
        }
    }

    public void TogglePlayInputs(bool isOn)
    {
        if (isOn)
        {
            eegToggle.interactable = true;
            resumeToggle.interactable = true;
        }
        else
        {
            // Force off & disable interactivity
            eegToggle.isOn = false;
            resumeToggle.isOn = false;
            eegToggle.interactable = false;
            resumeToggle.interactable = false;
            participantID.text = string.Empty;
        }

        taskButton.interactable = isOn;
        participantID.interactable = isOn;
    }


}