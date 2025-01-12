using System.Collections;
using UnityEngine;
using System.Linq;


public class TaskController : MonoBehaviour
{
    
    private SessionManager sessionManager;
    private PatchPresenter patchPresenter;

    private bool useBlueEnv = true;

    private int trialIdx = 0;
    private int maxTrials = 90;

    private int[] patchOrder;
    private bool[] truncationOrder;

    int points;
    int accumulatedPoints;

    public float[] rewards;



    private void Awake()
    {
        sessionManager = GetComponent<SessionManager>();
        patchPresenter = GetComponent<PatchPresenter>();
    }

    // get in the 
 
    public IEnumerator RunTask()
    {
        int[] accumPointsFeedback = { Mathf.FloorToInt(maxTrials / 4), Mathf.FloorToInt(maxTrials / 2), Mathf.FloorToInt((maxTrials * 3) / 4) };

        patchOrder = sessionManager.patchOrder;
        truncationOrder = sessionManager.truncationOrder;

        yield return new WaitForSeconds(1.0f);
        yield return StartCoroutine(sessionManager.Intertrial("Starting Task"));
       
        sessionManager.useBlueEnv = useBlueEnv;

        while (trialIdx < maxTrials)
        {
            // get patch
            sessionManager.patchIdx = patchOrder[trialIdx];
            sessionManager.leave = null;

            rewards = sessionManager.SetPatch(); 
                       
            yield return StartCoroutine(patchPresenter.StartPatch(rewards, sessionManager.useBlueEnv, trialIdx, sessionManager.patchIdx));

            if (sessionManager.leave == null)
            {
                sessionManager.BeginChoicePhase();
                while (sessionManager.leave == null)
                {
                    yield return null;
                }
            }

            // write out decision via session manager 
            sessionManager.WriteOutLSD();

            if (!truncationOrder[trialIdx])
            {
                rewards = sessionManager.SetPatch();
                yield return StartCoroutine(patchPresenter.StartPatch(rewards, sessionManager.useBlueEnv,trialIdx, sessionManager.patchIdx));
            }

            points = sessionManager.GetPoints();
            accumulatedPoints += points;
            yield return StartCoroutine(sessionManager.ShowPointsTrial(trialIdx, points));


            if (accumPointsFeedback.Contains(trialIdx))
            {
                yield return StartCoroutine(sessionManager.ShowAccumulatedPoints(accumulatedPoints));
            }

            trialIdx++;
        }

        StartCoroutine(sessionManager.EndSession("End of Task"));
    }
    
}


