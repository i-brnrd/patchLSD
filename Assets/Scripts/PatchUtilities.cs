using System.Linq;
using UnityEngine;

public class PatchUtilities : MonoBehaviour
{
    private int[,] groups; // 18x5 array to represent the groups
    private bool[] usedLines = new bool[90]; // mask to track lines 
    private bool[] usedGroups = new bool[18]; // groups 
    private int lastGroup = -1; 
                               
    private void Awake()
    {
        // Initialize the 18x5 group array
        groups = new int[18, 5];

        //  fill arr with line numbers (0-89, 5 lines per group)
        int lineIdx = 0;
        for (int group = 0; group < 18; group++)
        {
            for (int line = 0; line < 5; line++)
            {
                groups[group, line] = lineIdx;
                lineIdx++;
            }
        }

        ResetUsedGroups();
    }

    private void ResetUsedGroups()
    {
        for (int i = 0; i < 18; i++)
        {
            usedGroups[i] = false;
        }
    }

    // Returns the next random trial that isn't from the same group as the previous one
    public int GetTrial()

    {

        // Check if all groups have been used
        bool allGroupsUsed = true;
        for (int i = 0; i < 18; i++)
        {
            if (!usedGroups[i])
            {
                allGroupsUsed = false;
                break;
            }
        }

        // If all used, reset 
        if (allGroupsUsed)
        {
            ResetUsedGroups();
        }
        int selectedGroup;
        // Select a random group NOT same as last one OR se;ected in this round 
        do
        {
            selectedGroup = Random.Range(0, 18);
        } while (selectedGroup == lastGroup || usedGroups[selectedGroup]);

        // Select a random UNUSED line from the selected group
        int selectedLine;
        do
        {
            selectedLine = groups[selectedGroup, Random.Range(0, 5)];
        } while (usedLines[selectedLine]);

        usedLines[selectedLine] = true;
        usedGroups[selectedGroup] = true;
        lastGroup = selectedGroup;
        return selectedLine;
    }
}

