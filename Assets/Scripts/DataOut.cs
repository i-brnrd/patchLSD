using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DataOut : MonoBehaviour
{
    // Start is called before the first frame update
    // Idea here is to gate the LSL code into Win Only for now.

    private LSLStream lslStream;

    private void Awake()
    {
        lslStream = GetComponent<LSLStream>();

    }
    public void LogMessage(string message)
    {
        Debug.Log(message);

#if UNITY_STANDALONE_WIN
        lslStream.TriggerLSLEvent(message);
#endif
    }

}
