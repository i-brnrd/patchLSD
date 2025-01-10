using System.Collections;
using UnityEngine;
using System.Linq;


public class TaskController : MonoBehaviour
{
    
    private PatchManager sessionManager;
    private PatchUtilities trialSelectUtils;
    private Patch patchPresenter;

    private bool useBlueEnv = true;
    private int maxTrials = 90;

    private int count = 0;
    
    int points;
    int accumulatedPoints;
    bool shouldTruncate = true;

    public float[] rewards;

    // randomise the truncations here remember
    // sld rewards be public? 

    private void Awake()
    {
        sessionManager = GetComponent<PatchManager>();
        trialSelectUtils = GetComponent<PatchUtilities>();
        patchPresenter = GetComponent<Patch>();

    }
 
    public IEnumerator ExecuteTask()
    {
        int[] accumPointsFeedback = { Mathf.FloorToInt(maxTrials / 4), Mathf.FloorToInt(maxTrials / 2), Mathf.FloorToInt((maxTrials * 3) / 4) };

        yield return StartCoroutine(sessionManager.Intertrial("Starting Task"));
        Debug.Log("start wait WHAT IS GOING ON ");
        yield return new WaitForSeconds(1.0f);
        Debug.Log("stop wait");

        sessionManager.useBlueEnv = useBlueEnv;

        while (count < maxTrials)
        {
            sessionManager.leave = null;
            sessionManager.trial = trialSelectUtils.GetTrial();
            rewards = sessionManager.SetPatchArg(sessionManager.leave); 
                       
            yield return StartCoroutine(patchPresenter.StartPatch(rewards, sessionManager.useBlueEnv, sessionManager.trial));

            if (sessionManager.leave == null)
            {
                sessionManager.BeginChoicePhase();
                while (sessionManager.leave == null)
                {
                    yield return null;
                }
            }

            // write out via session manager 
            sessionManager.WriteOutData();


            shouldTruncate = count % 3 != 0; // call out 

            if (!shouldTruncate)
            {
                rewards = sessionManager.SetPatchArg(sessionManager.leave);
                yield return StartCoroutine(patchPresenter.StartPatch(rewards, sessionManager.useBlueEnv, sessionManager.trial));
            }

            points = sessionManager.GetPoints();
            accumulatedPoints += points;
            yield return StartCoroutine(sessionManager.ShowPointsTrial(count, points));


            if (accumPointsFeedback.Contains(count))
            {
                yield return StartCoroutine(sessionManager.ShowAccumulatedPoints(accumulatedPoints));
            }

            count++;
        }

        StartCoroutine(sessionManager.EndSession("End of Task"));
    }
    
}


