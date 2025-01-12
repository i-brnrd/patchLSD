using UnityEngine;
using System.Collections.Generic;

public class PatchUtilities : MonoBehaviour
{
    private int[,] groups; // 18x5 array (0..89)
    private bool[] usedLines = new bool[90];
    private bool[] usedGroups = new bool[18];
    private int lastGroup = -1;

    private void Awake()
    {
        // Fill groups with [group, line] => 0..89
        groups = new int[18, 5];
        int lineIdx = 0;
        for (int group = 0; group < 18; group++)
        {
            for (int line = 0; line < 5; line++)
            {
                groups[group, line] = lineIdx;
                lineIdx++;
            }
        }
    }

    public int[] GeneratePatchOrder()
    {
        // Prepare a local array to store the result
        int[] trialOrder = new int[90];

        // Reset your internal states before generation
        ResetUsedLines();
        ResetUsedGroups();
        lastGroup = -1;

        // Generate each trial in sequence
        for (int i = 0; i < 90; i++)
        {
            // If all groups used, reset them
            if (AllGroupsUsed())
                ResetUsedGroups();

            // Pick a valid group
            int selectedGroup;
            do
            {
                selectedGroup = Random.Range(0, 18);
            }
            while (selectedGroup == lastGroup || usedGroups[selectedGroup]);

            // Pick a random line from that group
            int selectedLine;
            do
            {
                selectedLine = groups[selectedGroup, Random.Range(0, 5)];
            }
            while (usedLines[selectedLine]);

            // Mark them as used
            usedLines[selectedLine] = true;
            usedGroups[selectedGroup] = true;
            lastGroup = selectedGroup;

            // Store the result
            trialOrder[i] = selectedLine;
        }

        return trialOrder;
    }

    private void ResetUsedLines()
    {
        for (int i = 0; i < usedLines.Length; i++)
        {
            usedLines[i] = false;
        }
    }

    private void ResetUsedGroups()
    {
        for (int i = 0; i < usedGroups.Length; i++)
        {
            usedGroups[i] = false;
        }
    }

    private bool AllGroupsUsed()
    {
        for (int i = 0; i < 18; i++)
        {
            if (!usedGroups[i]) return false;
        }
        return true;
    }


    public bool[] GenerateTruncations()
    {
        // Generate 18 blocks of 5 items (total 90).
        bool[] truncations = new bool[90];

        // Fill in the array in “blocks” of 3 'true', 2 'false', then shuffle. 
        int index = 0;
        for (int block = 0; block < 18; block++)
        {
            // Build a local list of 5: 3 true, 2 false
            var blockBools = new List<bool> { true, true, true, false, false };

            // Shuffle this list
            for (int i = 0; i < blockBools.Count; i++)
            {
                int randomIndex = Random.Range(i, blockBools.Count);
                // Swap the current element with the randomly chosen one
                bool temp = blockBools[i];
                blockBools[i] = blockBools[randomIndex];
                blockBools[randomIndex] = temp;
            }

            // Copy the shuffled block into the main array
            for (int i = 0; i < 5; i++)
            {
                truncations[index] = blockBools[i];
                index++;
            }
        }

        return truncations;
    }
}

