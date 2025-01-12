using System.Collections;
using UnityEngine;
using System.Linq;


public class TrainingAController : MonoBehaviour
{
    public GameObject trainingAFeedbackScreen;

    private SessionManager sessionManager;
    private PatchPresenter patchPresenter;
    private TrainingAFeedback trainingAFeedback;


    private int trialIdx = 0;

    private float[] redRewards;
    private float[] blueRewards;

    private void Awake()
    {
        sessionManager = GetComponent<SessionManager>();
        patchPresenter = GetComponent<PatchPresenter>();
        trainingAFeedback = trainingAFeedbackScreen.GetComponent<TrainingAFeedback>();
    }


    public IEnumerator RunTrainingA()
    {
        int[] trialsA = { 18, 42, 86, 27, 65, 39, 76, 4, 13, 53 }; //MATLAB trianing 1 based indexing

        while (trialIdx < trialsA.Length)
        {
            if (trialIdx == 0)
            {
                yield return StartCoroutine(sessionManager.Intertrial("Start of Training (A)", 0.5f));
            }
            else
            {
                yield return StartCoroutine(sessionManager.Intertrial());
            }

            sessionManager.leave = true;
            sessionManager.patchIdx = trialsA[trialIdx] - 1; //c# 0 based
            redRewards = sessionManager.SetPatch(); //sets patch  

            yield return StartCoroutine(patchPresenter.StartPatch(redRewards, sessionManager.useBlueEnv, trialIdx, sessionManager.patchIdx));
            yield return StartCoroutine(sessionManager.Intertrial());


            sessionManager.leave = null;
            sessionManager.patchIdx = trialsA[trialIdx] - 1; //c# 0 based
            blueRewards = sessionManager.SetPatch();

            yield return StartCoroutine(patchPresenter.StartPatch(blueRewards, sessionManager.useBlueEnv, trialIdx, sessionManager.patchIdx));

            sessionManager.inChoicePhase = true;
            trainingAFeedbackScreen.SetActive(true);
            yield return StartCoroutine(trainingAFeedback.Choice());

            while (sessionManager.inChoicePhase == true) // wait til the participant returns a choice 
            {
                yield return null;
            }

            // give feedback on their decision
            float redSum = redRewards.Sum();
            float blueSum = blueRewards.Sum();

            if ((redSum > blueSum & (sessionManager.leave ?? false)) || (blueSum > redSum & (!sessionManager.leave ?? false)))
            {
                yield return StartCoroutine(sessionManager.Intertrial("You made the correct decision"));
            }
            else
            {
                yield return StartCoroutine(sessionManager.Intertrial("You made the wrong decision"));
            }

            trialIdx++;
        }
    }

    public void TrainingAChoice(bool red)
    {
        Debug.Log("Training A: leave? " + red.ToString());
        trainingAFeedbackScreen.SetActive(false);
        sessionManager.inChoicePhase = false;
        sessionManager.leave = red;
    }
}

            