using UnityEngine;
using System;
using System.IO;
using TMPro;
using System.Linq;

public class BehaviouralDataIO : MonoBehaviour
{

    public GameObject sessionManagerObject;
    private SessionManager sessionManager;

    public TMP_InputField participantID;

    // check if these actually need public they may very well not
    public string participantDirName;
    public string pathToParticipantDir;
    public string pathToStateDir;
    public string pathToSessionDir;

    private string timeStamp;


    private void Awake()
    {
        sessionManager = sessionManagerObject.GetComponent<SessionManager>();
    }


    public void WriteOutRewards(bool? leaveDecision, int trialIdx, int patchIdx)
    {
        float[] rewards;
        string dataToWrite;

        // Write Out Rew2LSD
        rewards = sessionManager.SetPatch(null, patchIdx);
        dataToWrite = string.Join(",", rewards.Select(r => r.ToString()).ToArray());
        File.AppendAllText(pathToSessionDir + "/RewardToLSD.txt", dataToWrite + "\n");

        // Write Out LDStay
        rewards = sessionManager.SetPatch(false, patchIdx);
        dataToWrite = string.Join(",", rewards.Select(r => r.ToString()).ToArray());
        File.AppendAllText(pathToSessionDir + "/LDStay.txt", dataToWrite + "\n");

        // Write Out LDLeave
        rewards = sessionManager.SetPatch(true, patchIdx);
        dataToWrite = string.Join(",", rewards.Select(r => r.ToString()).ToArray());
        File.AppendAllText(pathToSessionDir + "/LDLeave.txt", dataToWrite + "\n");

        // Write Out What They Saw Post LSD
        if (!sessionManager.truncationOrder[trialIdx])
        {
            rewards = sessionManager.SetPatch(leaveDecision, patchIdx);
        }
        else
        {
            rewards = new float[] { 0f };
        }
        dataToWrite = string.Join(",", rewards.Select(r => r.ToString()).ToArray());
        File.AppendAllText(pathToSessionDir + "/PostLSD.txt", dataToWrite + "\n");
    }


    // leave stay decision 
    public void WriteOutLSD(bool? leaveDecision, int trialIdx, int patchIdx)
    {
        string[] decisionToWrite = { (trialIdx + 1).ToString(), (patchIdx + 1).ToString(), leaveDecision.ToString() };
        string dataToWrite = string.Join(",", decisionToWrite);
        File.AppendAllText(pathToSessionDir + "/Choice.txt", dataToWrite + "\n");
    }


    public void WriteState(int trialIdx)
    {
        Debug.Log("In Write State" + trialIdx.ToString());
        File.WriteAllText(Path.Combine(pathToStateDir, "state.log"), trialIdx.ToString());
    }

    private void WriteHeaders()
    {
        File.AppendAllText(pathToSessionDir + "/Choice.txt", "Leave Stay Decision \n" + "Trial Index, Patch Index, Leave (1 is Leave, 0 is Stay) \n");
        File.AppendAllText(pathToSessionDir + "/RewardToLSD.txt", "Reward Schedule: Rewards from Patch Start to LSD (in order as presented) \n");
        File.AppendAllText(pathToSessionDir + "/LDStay.txt", "LSD=Stay: Rewards from LSD to End (in order as presented)\n");
        File.AppendAllText(pathToSessionDir + "/LDLeave.txt", "LSD=Leave: Rewards from LSD to End (in order as presented)\n");
        File.AppendAllText(pathToSessionDir + "/PostLSD.txt", "Rewards Presented Post LSD (i.e '0' indicates trial was truncated postLSD) \n");
    }


    public void SetupLogDirectories()
    {
        DateTimeNow(); 
        ParticipantDir();
        InitSessionDir();
        StateDir();
        WriteHeaders();

        Debug.Log("Path to PiD Data " + pathToParticipantDir);
        Debug.Log("Path to State Data " + pathToStateDir);
        Debug.Log("Path to Session Dir " + pathToSessionDir);
        Debug.Log("Path to Session Dir " + participantDirName);


    }

    private void InitSessionDir()
    {
        pathToSessionDir = Path.Combine(pathToParticipantDir, "DATA_" + timeStamp);

        Directory.CreateDirectory(pathToSessionDir);

        File.WriteAllText(pathToSessionDir + "/init.log", "Participant ID:  " + participantDirName);
        File.WriteAllText(pathToSessionDir + "/init.log", "Task Initialised/ Resumed at: " + timeStamp);

    }

    private void DateTimeNow()
    {
        DateTime currentTime = DateTime.Now;
        timeStamp = currentTime.ToString("yyyy_MM_dd_HH_mm_ss");

    }

    private void ParticipantDir()
    {
        // If it's empty; or null (i.e. Web GL) we will use a datetime string. 
        if (!participantID.IsActive() || string.IsNullOrWhiteSpace(participantID.text))
        {
            participantDirName = timeStamp;
        }
        else
        {
            participantDirName = participantID.text;
        }

        pathToParticipantDir = Path.Combine(Application.persistentDataPath, participantDirName);

        if (!Directory.Exists(pathToParticipantDir))
        {
            Directory.CreateDirectory(pathToParticipantDir);
        }

       

    }

    private void StateDir()
    {
        pathToStateDir = Path.Combine(pathToParticipantDir, "STATE");
        if (!Directory.Exists(pathToStateDir))
        {
            Directory.CreateDirectory(pathToStateDir);
        }

    }

}

