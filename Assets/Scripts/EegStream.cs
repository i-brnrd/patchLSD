using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EegStream : MonoBehaviour
{
    // Idea here is to gate the LSL code via an eegFlag
    // Which I intend to switch to false for all webGL builds. 

    private LSLStream lslStream;
    private GameManager gameManager;

    private void Awake()
    {
        lslStream = GetComponent<LSLStream>();
        gameManager = GetComponent<GameManager>();

    }

    public void StartLSL()
    {
        if (gameManager.eegFlag)
        {
            lslStream.StreamStart();
        }
    }


    public void StopLSL()
    {
        if (gameManager.eegFlag)
        {
            lslStream.StreamStop();
        }
    }

    public void LogMessage(string message) // core stream log method 
    {
        Debug.Log(message);
        if (gameManager.eegFlag)
        {
            if (lslStream != null)
            {
                lslStream.TriggerLSLEvent(message);
            }
        }

    }

}