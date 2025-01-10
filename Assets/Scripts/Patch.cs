using System.Collections;
using UnityEngine;

public class Patch : MonoBehaviour
{
    public GameObject BoxObj;
    private Box box;
    public GameObject fixationCross;
    private LSLStream lslStream;

    private float[] rewards;
    private bool blueEnv;
    private int eventCount = 0;
    private int curve;
    private bool training;

    private readonly float EVENTDISPLAYTIME = 0.8f; // as per Marco's task
    Color myBlue = new Color(0f / 255f, 0f / 255f, 142f / 255f);
    Color myRed = new Color(190f / 255f, 0f / 255f, 0f / 255f);

    private void Awake()
    {
       box  = BoxObj.GetComponent<Box>();
       lslStream = GetComponent<LSLStream>();
    }

    public IEnumerator StartPatch(float[] patchArray, bool envB, int reward_curve)
    {
        training = false;
        eventCount = 0;
        rewards = patchArray;
        
        blueEnv = envB;

        curve = reward_curve;

        while (eventCount < rewards.Length)
        {
            yield return StartCoroutine(InterEvent());
        }
    }

    public IEnumerator StartPatchTraining(float[] patchArray, bool envB)
    {
        training = true;
        eventCount = 0;
        rewards = patchArray;

        blueEnv = envB;

        while (eventCount < rewards.Length)
        {
            yield return StartCoroutine(InterEvent());
        }
    }

    private IEnumerator InterEvent()
    {
        fixationCross.SetActive(true);
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }

        fixationCross.SetActive(false);
        yield return StartCoroutine(ShowBox());
    }

    private IEnumerator ShowBox()
    {
        float reward;
        int envTag;
        int fillLevelTag;

        int step;
        

        BoxObj.SetActive(true);
        if (blueEnv)
        {
            envTag = 1;
            box.SetBoxColour(myBlue);
        }
        else
        {
            envTag = 2;
            box.SetBoxColour(myRed);
        }

        step = eventCount + 1;
        reward = rewards[eventCount];
        fillLevelTag = Mathf.RoundToInt(reward * 99);
        box.SetBarHeight(reward);
        if (!training) {
            //Debug.Log($"{envTag}{step:D2}{curve:D2}{fillLevelTag:D2}");
#if UNITY_STANDALONE_WIN

            lslStream.TriggerLSLEvent($"{envTag}{step:D2}{curve:D2}{fillLevelTag:D2}");
#endif
    }
        yield return new WaitForSeconds(EVENTDISPLAYTIME);
        BoxObj.SetActive(false);
        eventCount++;
    }

}



  



