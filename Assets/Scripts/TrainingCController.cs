using System.Collections;
using UnityEngine;


public class TrainingCController : MonoBehaviour
{
    private SessionManager sessionManager;
    private PatchUtilities patchUtils;
    private PatchPresenter patchPresenter;

    
    private int maxTrials = 18;

    private int trialIndex;
    private int patchIndex;
    private bool? leave;

    private int[] patchOrder;

    private int points;

    public float[] rewards;

    private void Awake()
    {
        sessionManager = GetComponent<SessionManager>();
        patchUtils = GetComponent<PatchUtilities>();
        patchPresenter = GetComponent<PatchPresenter>();

    }

    public IEnumerator RunTrainingC()
    {
        patchOrder = patchUtils.GeneratePatchOrder(); //generate a random order 

        trialIndex = 0;

        while (trialIndex < maxTrials)
        {
            if (trialIndex== 0)
            {
                yield return StartCoroutine(sessionManager.Intertrial("Start of Training C", 0.5f));
            }
            else
            {
                yield return StartCoroutine(sessionManager.Intertrial());
            }

            patchIndex = patchOrder[trialIndex];
            leave = null;
            rewards = sessionManager.SetPatch(leave,patchIndex);

            yield return StartCoroutine(patchPresenter.StartPatch(rewards, leave, trialIndex, patchIndex));

            // After the patch is complete, decide what to do next
            if (leave == null)
            {
                sessionManager.BeginChoicePhase();
                while (sessionManager.leaveDecision == null)
                {
                    yield return null;
                }
                leave = sessionManager.leaveDecision;
                sessionManager.leaveDecision = null;
            }

            rewards = sessionManager.SetPatch(leave, patchIndex);
            yield return StartCoroutine(patchPresenter.StartPatch(rewards, leave, trialIndex, patchIndex));

            points = sessionManager.GetPoints(leave, patchIndex);
            yield return StartCoroutine(sessionManager.ShowPointsTrial(trialIndex, points));

            trialIndex++;


        }
        StartCoroutine(sessionManager.EndSession("End of Training C"));
    }



}