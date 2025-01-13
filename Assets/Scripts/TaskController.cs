using System.Collections;
using UnityEngine;
using System.Linq;


public class TaskController : MonoBehaviour
{
    
    private SessionManager sessionManager;
    private PatchPresenter patchPresenter;

    private bool useBlueEnv = true;

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
        Debug.Log("In RunTask:::" + sessionManager.trialIdx.ToString());
        int[] accumPointsFeedback = { Mathf.FloorToInt(maxTrials / 4), Mathf.FloorToInt(maxTrials / 2), Mathf.FloorToInt((maxTrials * 3) / 4) };

        patchOrder = sessionManager.patchOrder;
        truncationOrder = sessionManager.truncationOrder;

        yield return new WaitForSeconds(1.0f);
        yield return StartCoroutine(sessionManager.Intertrial("Starting Task"));
       
        sessionManager.useBlueEnv = useBlueEnv;

        while (sessionManager.trialIdx < maxTrials)
        {
            // get patch
            sessionManager.patchIdx = patchOrder[sessionManager.trialIdx];
            sessionManager.leave = null;

            rewards = sessionManager.SetPatch(); 
                       
            yield return StartCoroutine(patchPresenter.StartPatch(rewards, sessionManager.useBlueEnv, sessionManager.trialIdx, sessionManager.patchIdx));

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
            sessionManager.WriteOutRewards(rewards);
           

            if (!truncationOrder[sessionManager.trialIdx])
            {
                rewards = sessionManager.SetPatch();
                yield return StartCoroutine(patchPresenter.StartPatch(rewards, sessionManager.useBlueEnv,sessionManager.trialIdx, sessionManager.patchIdx));
            }

            points = sessionManager.GetPoints();
            accumulatedPoints += points;
            yield return StartCoroutine(sessionManager.ShowPointsTrial(sessionManager.trialIdx, points));


            if (accumPointsFeedback.Contains(sessionManager.trialIdx))
            {
                yield return StartCoroutine(sessionManager.ShowAccumulatedPoints(accumulatedPoints));
            }

            sessionManager.trialIdx++;
            sessionManager.WriteState();
        }

        StartCoroutine(sessionManager.EndSession("End of Task"));
    }
    
}


