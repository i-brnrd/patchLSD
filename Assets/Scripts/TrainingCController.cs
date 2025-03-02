using System.Collections;
using UnityEngine;


public class TrainingCController : MonoBehaviour
{
    public GameObject sessionManagerObject;
    private SessionManager sessionManager;

    public GameObject patchObject;
    private PatchPresenter patchPresenter;
    private PatchUtilities patchUtils;
    
    private int maxTrials = 18; //only want to run through 18 (one of each).

    private int trialIndex;
    private int patchIndex;
    private bool? leave;

    private int[] patchOrder;

    private int points;

    public float[] rewards;

    private void Awake()
    {
        sessionManager = sessionManagerObject.GetComponent<SessionManager>();
        patchUtils = patchObject.GetComponent<PatchUtilities>();
        patchPresenter = patchObject.GetComponent<PatchPresenter>();

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
        StartCoroutine(sessionManager.EndTrainingSession("Training C Complete"));
    }



}