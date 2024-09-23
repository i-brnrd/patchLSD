using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;

public class LSLStream : MonoBehaviour
{
    string StreamName = "LSL4Unity.Isla.Marco";
    string StreamType = "Markers";
    private StreamOutlet outlet;
    private string[] sample = { "" };

    public void TriggerLSLEvent(string message )
    {
        Debug.Log("Log: " + message);
        // Send event via LSL when button is clicked
        if (outlet != null)
        {
            sample[0] = message;
            outlet.push_sample(sample);
            //Debug.Log("Sent LSL event: " + sample[0]);
        }
        else
        {
            Debug.LogWarning("No stream started");
        }
    }

    public void StreamStart()
    {
        Debug.Log("Trying to start LSL Stream");

        // Create and start the LSL stream
        if (outlet == null)
        {
            var hash = new Hash128();
            hash.Append(StreamName);
            hash.Append(StreamType);
            hash.Append(gameObject.GetInstanceID());

            StreamInfo streamInfo = new StreamInfo(StreamName, StreamType, 1, LSL.LSL.IRREGULAR_RATE, channel_format_t.cf_string, hash.ToString());
            outlet = new StreamOutlet(streamInfo);

            Debug.Log("Stream started: " + StreamName);
        }
        else
        {
            Debug.LogWarning("Stream already running!");
        }
    }

    public void StreamStop()
    {
        Debug.Log("Trying to stop the LSL Stream");

        // Stop the stream
        if (outlet != null)
        {
            outlet = null; // Deref
            Debug.Log("Stream stopped.");
        }
        else
        {
            Debug.LogWarning("No stream running");
        }
    }
}
