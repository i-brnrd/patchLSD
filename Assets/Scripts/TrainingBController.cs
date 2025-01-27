using System.Collections;
using UnityEngine;


public class TrainingBController : MonoBehaviour
{

    private SessionManager sessionManager;
    private PatchPresenter patchPresenter;

    private int trialIndex;
    private int patchIndex;
    private bool? leave;

    private float[] rewards;

    private void Awake()
    {
        sessionManager = GetComponent<SessionManager>();
        patchPresenter = GetComponent<PatchPresenter>();
    }

    public IEnumerator RunTrainingB()
    {
        trialIndex = 0;
        int[] trialsB = { 7, 41, 81 };

        int totalSets = 3;
        int setIdx = 0;

        while (setIdx < totalSets)
        {
            yield return StartCoroutine(sessionManager.Intertrial($"Start of Training B: Set {setIdx + 1}"));

            for (int i = 0; i < trialsB.Length; i++)
            {
                if (i != 0)
                {
                    yield return StartCoroutine(sessionManager.Intertrial("End of Patch", 0.5f));  // Regular intertrial
                }

                patchIndex = trialsB[i] - 1; //c# 1 based indexing

                leave = null;
                rewards = sessionManager.SetPatch(leave, patchIndex);

                yield return StartCoroutine(patchPresenter.StartPatch(rewards, leave, trialIndex, patchIndex));

                leave = false;
                rewards = sessionManager.SetPatch(leave, patchIndex);

                yield return StartCoroutine(patchPresenter.StartPatch(rewards, leave, trialIndex, patchIndex));

            }

            if (setIdx < totalSets - 1)
            {
                yield return StartCoroutine(sessionManager.Intertrial($"Set {setIdx + 1} Complete"));
            }

            setIdx++; // Increment the set counter
        }
        StartCoroutine(sessionManager.EndSession("End of Training B"));
    }
}

