using UnityEngine;

public class EegStream : MonoBehaviour
{
    // Idea here is to gate the LSL code via an eegFlag set at start. 
    // (disable for all webGL builds)

    private LSLStream lslStream;
    private GameManager gameManager;

    private bool eegStreamOn = false;

    private void Awake()
    {
        lslStream = GetComponent<LSLStream>();
        gameManager = GetComponent<GameManager>();

    }

    // Start & Stop 

    public void StartLSL()
    {
        if (gameManager.eegFlag)
        {
            eegStreamOn = true;
            lslStream.StreamStart();
        }
    }


    public void StopLSL()
    {
        if (gameManager.eegFlag)
        {
            eegStreamOn = false;
            lslStream.StreamStop();
        }
    }

    // Logs.
    // NB: LogMessage is sufficient; as is lslStream.TriggerLSLEvent(message)
    // Event-named methods definded here for ease; & eventual porting to Unity Event System? Whatever that is. 

    public void LogMessage(string message) // core stream log method 
    {
        Debug.Log(message);
        if (gameManager.eegFlag && eegStreamOn)
        {
            if (lslStream != null)
            {
                lslStream.TriggerLSLEvent(message);
            }
        }

    }
    public void LogRewardEvent(bool inBlueEnv, int eventIndex, int trialIndex, int patchIndex, float reward)
    {
        int environmentTag;
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

        fillLevelPercentage = Mathf.RoundToInt(reward * 99);

        eventNumber = eventIndex + 1;  // c# 0 based
        trialNumber = trialIndex + 1;
        patchNumber = patchIndex + 1;
        

        LogMessage($"{environmentTag}{eventNumber:D2}{trialNumber:D2}{patchNumber:D2}{fillLevelPercentage:D2}");

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