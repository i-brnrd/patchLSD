using System.Collections;
using UnityEngine;

public class Patch : MonoBehaviour
{
    private PatchManager patchManager;

    public GameObject BoxObj;
    private Box box;
    public GameObject fixationCross;

    private float[] rewards;
    private bool blueEnv;
    private int eventCount = 0;

    private readonly float EVENTDISPLAYTIME = 0.8f; // as per Marco's task
    Color myBlue = new Color(0f / 255f, 0f / 255f, 142f / 255f);
    Color myRed = new Color(190f / 255f, 0f / 255f, 0f / 255f);

    // Start is called before the first frame update
    private void Awake()
    {
       box  = BoxObj.GetComponent<Box>();
       patchManager= GetComponent<PatchManager>();
    }

    public void StartPatch(float[] patchArray, bool envB)
    {
        eventCount = 0;
        rewards = patchArray;
        Debug.Log(rewards.Length);
        blueEnv = envB;
        StartCoroutine(InterEvent());
    }
   
    private IEnumerator InterEvent()
    {
        fixationCross.SetActive(true);
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }

        NextEvent();
    }

    private void NextEvent()
    {
        fixationCross.SetActive(false);
        StartCoroutine(ShowBox());
    }

    private IEnumerator ShowBox()
    {
        float reward;
        BoxObj.SetActive(true);
        if (blueEnv)
        {
            box.SetBoxColour(myBlue);
        }
        else
        {
            box.SetBoxColour(myRed);
        }

        reward = rewards[eventCount];
        box.SetBarHeight(reward);
        yield return new WaitForSeconds(EVENTDISPLAYTIME);
        EndEvent();
    }

    private void EndEvent()
    {
        eventCount++;
        BoxObj.SetActive(false);
        bool endPatch = (eventCount == rewards.Length);
        Debug.Log(rewards.Length);

        if (endPatch)
        {
            EndPatch();
        }
        else
        {
            StartCoroutine(InterEvent());
        }

    }

 

}
