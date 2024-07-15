using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour

{
    public TextAsset rew2ldCSV;
    public TextAsset ldStayCSV;
    public TextAsset ldGoCSV;

    public List<float[]> rew2ld;

    void Start()
    {
        rew2ld = LoadFromCSV(rew2ldCSV); 
    }

    private List<float[]> LoadFromCSV(TextAsset csvFile)
    {
        List<float[]> floatData = new List<float[]>();

        // Split the file into lines
        string[] lines = csvFile.ToString().Split('\n');

        foreach (var line in lines)
        {
            // Split each line into values
            string[] values = line.Trim().Split(',');
            float[] floatLine = new float[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                if (float.TryParse(values[i], out float floatValue))
                {
                    floatLine[i] = floatValue;
                }
                else
                {
                    Debug.LogError("Failed to parse value: " + values[i]);
                }
            }

            // Add the line's float array to the main list
            floatData.Add(floatLine);
        }

        return floatData;
    }


}



