using System.Collections;
using UnityEngine;


public class TrainingCController : MonoBehaviour
{
    private SessionManager sessionManager;
    private PatchUtilities patchUtils;
    private PatchPresenter patchPresenter;

    private bool useBlueEnv = true;
    private int maxTrials = 18;
    private int trialIdx = 0;

    private int[] patchOrder;

    int points;

    public float[] rewards;

    private void Awake()
    {
        sessionManager = GetComponent<SessionManager>();
        patchUtils = GetComponent<PatchUtilities>();
        patchPresenter = GetComponent<PatchPresenter>();

    }

    public IEnumerator RunTrainingC()
    {
        patchOrder = patchUtils.GeneratePatchOrder(); 
        while (trialIdx < maxTrials)
        {
            if (trialIdx == 0)
            {
                yield return StartCoroutine(sessionManager.Intertrial("Start of Training (C)", 0.5f));
            }
            else
            {
                yield return StartCoroutine(sessionManager.Intertrial());
            }

            sessionManager.patchIdx = patchOrder[trialIdx];

            sessionManager.useBlueEnv = useBlueEnv; // do I need these both? 
            sessionManager.leave = null;
            rewards = sessionManager.SetPatch();
 
            yield return StartCoroutine(patchPresenter.StartPatch(rewards, sessionManager.useBlueEnv, trialIdx, sessionManager.patchIdx));

            // After the patch is complete, decide what to do next
            if (sessionManager.leave == null)
            {
                sessionManager.BeginChoicePhase();
                while (sessionManager.leave == null)
                {
                    yield return null;
                }
            }
            rewards = sessionManager.SetPatch();
            yield return StartCoroutine(patchPresenter.StartPatch(rewards, sessionManager.useBlueEnv, trialIdx, sessionManager.patchIdx));

            points = sessionManager.GetPoints();
            yield return StartCoroutine(sessionManager.ShowPointsTrial(trialIdx, points));

            trialIdx++;


        }
        StartCoroutine(sessionManager.EndSession("End of Training (C)"));
    }



}