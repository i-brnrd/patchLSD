using UnityEngine;

public class EegStream : MonoBehaviour
{
    // Idea here is to gate the LSL code via an eegStreamOn set at start. 
    // (disable for all webGL builds)

    public GameObject gameManagerObject;
    private GameManager gameManager;

    private LSLStream lslStream;

    private bool eegStreamOn = false;

    private void Awake()
    {
        lslStream = GetComponent<LSLStream>();
        gameManager = gameManagerObject.GetComponent<GameManager>();

    }

    // Start & Stop 

    public void StartLSL()
    {
        if (gameManager.eegStreamOn)
        {
            eegStreamOn = true;
            lslStream.StreamStart();
        }
    }


    public void StopLSL()
    {
        if (gameManager.eegStreamOn)
        {
            eegStreamOn = false;
            lslStream.StreamStop();
        }
    }

    // Eeg Logs: 
    // NB: LogMessage is sufficient; as is lslStream.TriggerLSLEvent(message)
    // For ease; specific methods to send eeg messages for each event are defined here. 
 
    public void LogMessage(string message) // core stream log method 
    {
        Debug.Log(message);
        if (gameManager.eegStreamOn && eegStreamOn)
        {
            if (lslStream != null)
            {
                lslStream.TriggerLSLEvent(message);
            }
        }

    }
    public void LogRewardEvent(bool inBlueEnv, bool? leave, int eventIndex, int trialIndex, int patchIndex, float reward)
    {
        int environmentTag;
        int prePostLSDTag;
        int eventNumber;
        int trialNumber;
        int patchNumber;
        int fillLevelPercentage;

        if (inBlueEnv)
        {
            environmentTag = 1;
        }
        else
        {
            environmentTag = 2;
        }

        if (leave == null)
        {
            prePostLSDTag = 1;
        }
        else
        {
            prePostLSDTag= 2;
        }



        fillLevelPercentage = Mathf.RoundToInt(reward * 99);

        eventNumber = eventIndex + 1;  // c# 0 based
        trialNumber = trialIndex + 1;
        patchNumber = patchIndex + 1;
        

        LogMessage($"{environmentTag}{prePostLSDTag}{eventNumber:D2}{trialNumber:D2}{patchNumber:D2}{fillLevelPercentage:D2}");

    }

    public void StartTask()
    {
        LogMessage("Start");
    }

    public void LogChoicePhaseBegins()
    {
        LogMessage("7");
    }

    public void LogButtonsAppear()
    {
        LogMessage("8");
    }

    public void LogLeaveDecision()
    {
        LogMessage("4000");
    }

    public void LogStayDecision()
    {
        LogMessage("3000");
    }

    public void LogPoints(int points)
    {
        LogMessage(points.ToString());
    }

}      