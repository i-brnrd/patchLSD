using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{

    // GameData Objects 
    public GameObject gameData;
    public GameObject startScreen;
    public GameObject pauseScreen;
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
    public TMP_Text taskText;
    public Toggle eegToggle;
    public TMP_InputField participantID;
    public Toggle resumeToggle;

    public bool isPaused = false;

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
        pauseScreen.SetActive(false);


#if UNITY_WEBGL
    // Hide EEG/resume toggles and participant ID input
    eegToggle.gameObject.SetActive(false);
    resumeToggle.gameObject.SetActive(false);
    participantID.gameObject.SetActive(false);
    taskText.gameObject.SetActive(false);
#endif
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = true;
            pauseScreen.SetActive(true);
        }
    }

    // if WebGL then set eegOptions as unavaiable

    private void Start()
    {
        SetStartState();
       startScreen.SetActive(true);
    }

    private void SetStartState()
    {
        isPaused = false;
        eegStreamOn = false;
        resumeParticipant = false;
        ToggleTrainingInputs(true);
        TogglePlayInputs(false);
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
        if (eegToggle != null && eegToggle.isOn)
        {
            eegStreamOn = true;
        }
        if (resumeToggle != null && resumeToggle.isOn)
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
        SetStartState();
        pauseScreen.SetActive(false);
        startScreen.SetActive(true);
    }

    public void ResumeSession()
    {
        StartCoroutine(ResumeDelay());
       
    }

    private IEnumerator ResumeDelay()
    {
        yield return new WaitForSeconds(0.4f);
        isPaused = false;
        pauseScreen.SetActive(false);
        // Optionally, you can reset the pause flag here if not handled in SessionManager
    }

    // will want to smarten up and make sure all inactive at start 


    private void ParticipantFolder()
    {
        string folderName;
        if (!participantID.IsActive())
        {
            Debug.Log("INACTIVE");
            // If it's empty or null (ie Web GL) we will use a datetime ticks UUID 
            folderName = string.Format(@"{0}.txt", DateTime.Now.Ticks);
        }
        else if (string.IsNullOrWhiteSpace(participantID.text))
        {
            // If it's empty or null (ie Web GL) we will use a datetime ticks UUID 
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

    private void InitLogs()
    {

        DateTime currentTime = DateTime.Now;
        string timeStamp = currentTime.ToString("yyyy.MM.dd.HH.mm.ss");

        string choiceDataFileName = "ChoiceData_" + timeStamp + ".txt";
        string rew2ldFileName = "Rew2Ld_" + timeStamp + ".txt";

        pathToChoiceData = Path.Combine(pathToFolder, choiceDataFileName);
        pathToRew2LD = Path.Combine(pathToFolder, rew2ldFileName);

        string choiceDataHeader = "Task Initialised/ Resumed at: " + currentTime + "\n" + "Trial, Leave?(bool) \n";

        string rew2ldHeader ="Task Initialised/ Resumed at: " + currentTime + "\n" +"Rewards (the given patch) to Leave/Stay Decision \n";

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
        taskButton.interactable = isOn;

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

        
        participantID.interactable = isOn;
    }


}