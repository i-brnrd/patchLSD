using System.Collections;
using UnityEngine;


public class TrainingBController : MonoBehaviour
{

    private SessionManager sessionManager;
    private PatchPresenter patchPresenter;

    private float[] rewards;

    private void Awake()
    {
        sessionManager = GetComponent<SessionManager>();
        patchPresenter = GetComponent<PatchPresenter>();
    }

    public IEnumerator RunTrainingB()
    {
        int[] trialsB = { 7, 41, 81 };

        int totalSets = 3;
        int setIdx = 0;

        while (setIdx < totalSets)
        {
            yield return StartCoroutine(sessionManager.Intertrial($"Start of Training (B) Set {setIdx + 1}"));

            for (int i = 0; i < trialsB.Length; i++)
            {
                if (i != 0)
                {
                    yield return StartCoroutine(sessionManager.Intertrial("End of Patch", 0.5f));  // Regular intertrial
                }

                sessionManager.patchIdx = trialsB[i] - 1; //c# 1 based indexing

                sessionManager.leave = null;
                rewards = sessionManager.SetPatch();

                yield return StartCoroutine(patchPresenter.StartPatch(rewards, sessionManager.useBlueEnv, i, sessionManager.patchIdx));

                sessionManager.leave = false;
                rewards = sessionManager.SetPatch();

                yield return StartCoroutine(patchPresenter.StartPatch(rewards, sessionManager.useBlueEnv, i, sessionManager.patchIdx));

            }

            if (setIdx < totalSets - 1)
            {
                yield return StartCoroutine(sessionManager.Intertrial($"Set {setIdx + 1} Complete"));
            }

            setIdx++; // Increment the set counter
        }
        StartCoroutine(sessionManager.EndSession("End of Training (B)"));
    }
}

