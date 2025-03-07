using System.Collections;
using UnityEngine;

public class PatchPresenter : MonoBehaviour
{

    public GameObject eegObject;
    private EegStream eegStream;

    public GameObject boxObject;
    private Box box;

    public GameObject fixation;

    private float[] rewards;
    private bool inBlueEnv;

    private int eventIdx = 0;
    private int trialIdx;
    private int patchIdx;
    private bool? leave;

    private readonly float EVENTDISPLAYTIME = 0.8f; // as per Marco's task

    Color boxBlue = new(0f / 255f, 0f / 255f, 142f / 255f);
    Color boxRed = new(190f / 255f, 0f / 255f, 0f / 255f);

    private void Awake()
    {
       eegStream = eegObject.GetComponent<EegStream>();
       box = boxObject.GetComponent<Box>();
    }

    public IEnumerator StartPatch(float[] rewardsArray, bool? leaveIn, int trialIndex, int patchIndex)
    {
        eventIdx = 0;
        rewards = rewardsArray;
       

        if (leaveIn == true)
        {
            inBlueEnv = false;
        }
        else
        {
            inBlueEnv = true;
        }

        trialIdx = trialIndex;
        patchIdx = patchIndex;
        leave = leaveIn;

        while (eventIdx < rewards.Length)
        {
            yield return StartCoroutine(InterEvent());
        }
    }

    private IEnumerator InterEvent()
    {
        fixation.SetActive(true);

        while (
    !Input.GetKeyDown(KeyCode.Space)
#if UNITY_WEBGL
    && !Input.GetMouseButtonDown(0)
#endif
)
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

        eegStream.LogRewardEvent(inBlueEnv, leave, eventIdx, trialIdx, patchIdx, reward);
        
        yield return new WaitForSeconds(EVENTDISPLAYTIME);
        boxObject.SetActive(false);
        eventIdx++;
    }

}



  



