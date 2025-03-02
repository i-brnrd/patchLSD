using System.Collections.Generic;
using UnityEngine;

public class RewardData : MonoBehaviour

{
    public TextAsset rew2ldCSV;
    public TextAsset ldStayCSV;
    public TextAsset ldGoCSV;

    public List<float[]> rew2ld;
    public List<float[]> ldStay;
    public List<float[]> ldGo;

    private void Awake()
    {
        rew2ld = LoadFromCSV(rew2ldCSV);
        ldStay = LoadFromCSV(ldStayCSV);
        ldGo = LoadFromCSV(ldGoCSV);
    }

    private List<float[]> LoadFromCSV(TextAsset csvFile)
    {
        List<float[]> floatData = new List<float[]>();

        // Split the file into lines
        string[] lines = csvFile.ToString().Split('\n');

        for (int lineIndex = 0; lineIndex < lines.Length -1; lineIndex++)
        {
            string line = lines[lineIndex];

            // Split each line into values
            string[] values = line.Trim().Split(',');
            float[] floatLine = new float[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                if (float.TryParse(values[i], out float floatValue))
                {
                    floatLine[i] = floatValue * (1.0f/0.35f); //max @ full 
                    if (floatLine[i] > 1.0f)
                    {
                        floatLine[i] = 1.0f;
                    }
                }
                else
                {
                    Debug.Log(i);
                    Debug.Log(lineIndex);
                    Debug.LogError("Failed to parse value: " + values[i]);
                }
            }

            // Add the line's float array to the main list
            floatData.Add(floatLine);
        }

        return floatData;
    }



}



