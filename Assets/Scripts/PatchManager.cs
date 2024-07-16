using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchManager : MonoBehaviour
{
    // GameData Objects 
    public GameObject gameData;
    private GameData patchData; // Contains lists of float arrays



    public Patch patch;

    private int[,] patchSets;
    private bool[,] isUsed;

    private int nSets = 18;
    private int nPatches = 5; //per set 

    private int availableSets = 5; 


    //these will all be public 

    public void InitPatchIdx()
    {
        // load in the actual patches elsewhere
        // patchSets keeps track of the Row Number of each Patch in the correct index. 
        for (int i = 0; i < nSets; i++)
        {
            for (int j = 0; j < nPatches; j++)
            {
                patchSets[i, j] = i * nSets + j;
                isUsed[i, j] = false;
            }
        }


    }


    public void GetNextPatch()
    {
        // for now make it actually random

    }

    

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{



}

