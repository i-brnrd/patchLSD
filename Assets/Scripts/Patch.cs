using System.Collections;
using UnityEngine;

public class Patch : MonoBehaviour
{
    public GameObject BoxObj;
    private Box box;
    public GameObject fixationCross;

    private float[] rewards;
    private bool blueEnv;
    private int eventCount = 0;

    private readonly float EVENTDISPLAYTIME = 0.8f; // as per Marco's task
    Color myBlue = new Color(0f / 255f, 0f / 255f, 142f / 255f);
    Color myRed = new Color(190f / 255f, 0f / 255f, 0f / 255f);

    private void Awake()
    {
       box  = BoxObj.GetComponent<Box>();
    }

    public IEnumerator StartPatch(float[] patchArray, bool envB)
    {
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
        BoxObj.SetActive(false);
        eventCount++;
    }

}
