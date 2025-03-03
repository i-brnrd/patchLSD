using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Manager Objects with Associated Scripts
    public GameObject sessionManagerObj;
    private SessionManager sessionManager;

    public GameObject gameDataObject;
    private BehaviouralDataIO behaviouralIO;

    // Canvas Objects (screens managed by GameManager)
    public GameObject startScreen;
    public GameObject pauseScreen;

    
    // Mode Toggles (on StartScreen) 
    public Toggle trainingToggle;
    public Toggle playToggle;

    // GameObjects 
    public GameObject trainingInputs;

    public Button taskButton;
    public TMP_Text taskText;
    public Toggle eegToggle;
    public TMP_InputField participantID;
    public Toggle resumeToggle;

    public bool isPaused = false;

    public bool eegStreamOn = false;
    public bool resumeParticipant = false;

    private void Awake()
    {

        sessionManager = sessionManagerObj.GetComponent<SessionManager>();
        behaviouralIO = gameDataObject.GetComponent<BehaviouralDataIO>();
        
        // For a WebGL build; hide EEG/resume toggles and participant ID input; & move the start task button. 
#if UNITY_WEBGL
        WebGLSetup();
#endif

    }


    private void WebGLSetup()
    {
        // Hide EEG/resume toggles and participant ID input; & move the start task button. 
        eegToggle.gameObject.SetActive(false);
        resumeToggle.gameObject.SetActive(false);
        participantID.gameObject.SetActive(false);
        taskText.gameObject.SetActive(false);
        taskButton.transform.localPosition = taskButton.transform.localPosition + new Vector3(0, 250, 0);
    }


    private void Update() //No pause screen on Esc in WebGL 
    {
#if !UNITY_WEBGL
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = true;
            pauseScreen.SetActive(true);
        }
#endif
    }

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

        behaviouralIO.SetupLogDirectories();
        startScreen.SetActive(false);
        sessionManager.StartTask();
    }

    public void EndSession()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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