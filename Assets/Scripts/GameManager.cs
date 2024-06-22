using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // GameObjects 
    public Presentation Presentation; // script
    public LSD LSD;

  
    public GameObject presentation; // object 
    public GameObject fixationCross;
    public GameObject canvas;


    // Temp for testing (patches) 
    public float[] patch;
    public float[] defaultPatch;

    // Floats & Vara
    private float reward;
    private int eventNo = 0;
    private float EVENTDISPLAYTIME = 2.0f;

    // Envs 
    private bool envB = true; // in the blue env
    public bool inChoicePhase = false; 

    // Data Persistance 
    private string pathToLogs;


    private void Awake()
    {
        // get timestamp and write as a filename 
        DateTime currentTime = DateTime.Now;
        string fileName = currentTime.ToString("yyyy.MM.dd.HH.mm.ss");

        pathToLogs = Application.persistentDataPath + "/" + fileName + ".txt";
        using (StreamWriter dataOut = File.CreateText(pathToLogs))
        {
            dataOut.WriteLine("Task Initialised at: " + DateTime.Now.ToString());
            dataOut.WriteLine("WHAT ARE THE COLUMN TITLES IDK");
        }

        // read in the patches from Marco's dataset here via an external script acting on GameData object
        // read in the data then read in the patches in that script 
        presentation.SetActive(false);
        canvas.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        patch = new float[] { 0.0f, 0.0f, 0.8f, 0.0f, 0f, 0f, 0.8f, 0f, 0f, 0f, 0.9f, 1.0f, 0.0f };
        defaultPatch = new float[] { 0.0f, 0.0f, 0.8f, 0.0f, 0f, 0f, 0.8f, 0f, 0f, 0f, 0.9f, 1.0f, 0.0f };

        StartTask();
    }

    void StartTask()
    {
        envB = true;
        StartPatch();
    }

    void StartPatch()
    {
        eventNo = 0; 
        StartCoroutine(InterEvent());
    }

    IEnumerator InterEvent()
    {
        fixationCross.SetActive(true);
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
        NextEvent();
    }

    void NextEvent()
    {
        fixationCross.SetActive(false);
        StartCoroutine(ShowPresentation());
    }

    IEnumerator ShowPresentation()
    {
        
        presentation.SetActive(true);
        if (envB)
        {
            Presentation.SetBoxColour(Color.blue);
        }
        else
        {
            Presentation.SetBoxColour(Color.red);
        }

        reward = patch[eventNo];
        Presentation.SetBarHeight(reward);
        yield return new WaitForSeconds(EVENTDISPLAYTIME);
        EndEvent();
    }

    void EndEvent()
    {
        eventNo++;
        presentation.SetActive(false);
        bool endPatch = (eventNo == patch.Length);
        if (endPatch)
        {
            canvas.SetActive(true);
            LSD.ChoicePhase();
        } else
        {
            StartCoroutine(InterEvent());
        }

      
    }

}