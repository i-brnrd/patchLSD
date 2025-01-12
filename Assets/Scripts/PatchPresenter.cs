using System.Collections;
using UnityEngine;

public class PatchPresenter : MonoBehaviour
{
    public GameObject boxObject;
    public GameObject fixation;

    private EegStream eegStream;
    private Box box;

    private float[] rewards;
    private bool inBlueEnv;

    private int eventIdx = 0;
    private int trialIdx;
    private int patchIdx;

    private readonly float EVENTDISPLAYTIME = 0.8f; // as per Marco's task

    Color boxBlue = new(0f / 255f, 0f / 255f, 142f / 255f);
    Color boxRed = new(190f / 255f, 0f / 255f, 0f / 255f);

    private void Awake()
    {
       box  = boxObject.GetComponent<Box>();
       eegStream = GetComponent<EegStream>();
    }

    public IEnumerator StartPatch(float[] rewardsArray, bool useBlueEnv, int trialIndex, int patchIndex)
    {
        eventIdx = 0;
        rewards = rewardsArray;
        inBlueEnv = useBlueEnv;

        trialIdx = trialIndex;
        patchIdx = patchIndex;

        while (eventIdx < rewards.Length)
        {
            yield return StartCoroutine(InterEvent());
        }
    }

    private IEnumerator InterEvent()
    {
        fixation.SetActive(true);
        while (!Input.GetKeyDown(KeyCode.Space) && !Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        fixation.SetActive(false);
        yield return StartCoroutine(ShowBox());
    }

    private IEnumerator ShowBox()
    {
        float reward;
        
        boxObject.SetActive(true);

        if (inBlueEnv)
        {
            box.SetBoxColour(boxBlue);
        }
        else
        {
            box.SetBoxColour(boxRed);
        }

        reward = rewards[eventIdx];
        box.SetBarHeight(reward);

        eegStream.LogRewardEvent(inBlueEnv, eventIdx, trialIdx, patchIdx, reward);
        
        yield return new WaitForSeconds(EVENTDISPLAYTIME);
        boxObject.SetActive(false);
        eventIdx++;
    }

}



  



