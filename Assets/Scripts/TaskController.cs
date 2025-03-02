using System.Collections;
using UnityEngine;
using System.Linq;


public class TaskController : MonoBehaviour
{

    public GameObject sessionManagerObject;
    private SessionManager sessionManager;

    public GameObject patchObject;
    private PatchPresenter patchPresenter;

    public GameObject gameData;
    private BehaviouralDataIO behaviouralData;

    private int maxTrials = 2;//private int maxTrials = 90;
    int patchIndex;
    bool? leave;
 
    int points;
    int accumulatedPoints;

    private float[] rewards;

    private void Awake()
    {
        sessionManager = sessionManagerObject.GetComponent<SessionManager>();
        patchPresenter = patchObject.GetComponent<PatchPresenter>();
        behaviouralData = gameData.GetComponent<BehaviouralDataIO>();
    }


    public IEnumerator RunTask(int trialIndex, int [] patchOrder, bool [] truncationOrder)
    {

        int[] accumPointsFeedback = { Mathf.FloorToInt(maxTrials / 4), Mathf.FloorToInt(maxTrials / 2), Mathf.FloorToInt((maxTrials * 3) / 4) };

        yield return new WaitForSeconds(1.0f);
        yield return StartCoroutine(sessionManager.Intertrial("Starting Task"));
       
        while (trialIndex < maxTrials)
        {
            // get patch
            patchIndex = patchOrder[trialIndex];
            leave = null;

            rewards = sessionManager.SetPatch(leave, patchIndex);
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

            // write out decision via session manager 
            behaviouralData.WriteOutLSD(leave, trialIndex, patchIndex);

            if (!truncationOrder[trialIndex])
            {
                rewards = sessionManager.SetPatch(leave, patchIndex);
                yield return StartCoroutine(patchPresenter.StartPatch(rewards, leave, trialIndex, patchIndex));
            }

            behaviouralData.WriteOutRewards(leave, trialIndex, patchIndex);


            points = sessionManager.GetPoints(leave, patchIndex);
            accumulatedPoints += points;

            trialIndex++;
            behaviouralData.WriteState(trialIndex);

            yield return StartCoroutine(sessionManager.ShowPointsTrial((trialIndex-1), points));

           

            if (accumPointsFeedback.Contains(trialIndex))
            {
                yield return StartCoroutine(sessionManager.ShowAccumulatedPoints(accumulatedPoints));
            }

        }

        sessionManager.EndTask();
    }
    
}


