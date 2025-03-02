using System.Collections;
using UnityEngine;
using System.Linq;


public class TrainingAController : MonoBehaviour
{
    public GameObject sessionManagerObject;
    private SessionManager sessionManager;

    public GameObject patchObject;
    private PatchPresenter patchPresenter;

    public GameObject trainingAFeedbackScreen;
    private TrainingAFeedback trainingAFeedback;


    private int trialIndex;
    private int patchIndex;
    private bool inChoicePhase;
    private bool? leave = true;

    private float[] redRewards;
    private float[] blueRewards;

    private void Awake()
    {
        sessionManager = sessionManagerObject.GetComponent<SessionManager>();
        patchPresenter = patchObject.GetComponent<PatchPresenter>();
        trainingAFeedback = trainingAFeedbackScreen.GetComponent<TrainingAFeedback>();
    }


    public IEnumerator RunTrainingA()
    {
        int[] trialsA = { 17, 41, 85, 26, 64, 38, 75, 3, 12, 52 }; // selected to give a variety. 

        trialIndex = 0;

        while (trialIndex < trialsA.Length)
        {
            if (trialIndex == 0)
            {
                yield return StartCoroutine(sessionManager.Intertrial("Start of Training A", 0.5f));
            }
            else
            {
                yield return StartCoroutine(sessionManager.Intertrial());
            }
            leave = true;
            patchIndex = trialsA[trialIndex];

            redRewards = sessionManager.SetPatch(leave,patchIndex); //sets patch  

            yield return StartCoroutine(patchPresenter.StartPatch(redRewards, leave, trialIndex, patchIndex));
            yield return StartCoroutine(sessionManager.Intertrial());


            leave = false;
            patchIndex = trialsA[trialIndex];
            blueRewards = sessionManager.SetPatch(leave, patchIndex);

            yield return StartCoroutine(patchPresenter.StartPatch(blueRewards, leave, trialIndex, patchIndex));

            leave = null; // resetting leave to null 
            inChoicePhase = true;
            trainingAFeedbackScreen.SetActive(true);

            yield return StartCoroutine(trainingAFeedback.Choice());

            while (inChoicePhase == true) // wait til the participant returns a choice 
            {
                yield return null;
            }

            // give feedback on their decision
            float redSum = redRewards.Sum();
            float blueSum = blueRewards.Sum();

            if ((redSum > blueSum & (leave ?? false)) || (blueSum > redSum & (!leave ?? false)))
            {
                yield return StartCoroutine(sessionManager.Intertrial("You made the correct decision"));
            }
            else
            {
                yield return StartCoroutine(sessionManager.Intertrial("You made the wrong decision"));
            }

            trialIndex++;
        }
        StartCoroutine(sessionManager.EndTrainingSession("Training A Complete"));
    }

    public void TrainingAChoice(bool choseLeave)
    {
        trainingAFeedbackScreen.SetActive(false);
        inChoicePhase = false;
        leave = choseLeave;
    }
}

            