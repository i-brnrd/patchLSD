using System.Collections;
using UnityEngine;

public class Patch : MonoBehaviour
{
    public GameManager gameManager;

    public GameObject BoxObj;
    private Box box;
    public GameObject fixationCross;

    private float[] rewards;
    private bool blueEnv;
    private int eventCount = 0;
    private readonly float EVENTDISPLAYTIME = 2.0f;

    // Start is called before the first frame update
    private void Awake()
    {
       box  = BoxObj.GetComponent<Box>();
    }

    public void StartPatch(float[] patchArray, bool envB)
    {
        eventCount = 0;
        rewards = patchArray;
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
            box.SetBoxColour(Color.blue);
        }
        else
        {
            box.SetBoxColour(Color.red);
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
        if (endPatch)
        {
            EndPatch();
        }
        else
        {
            StartCoroutine(InterEvent());
        }

    }

    private void EndPatch()
    {
        if (gameManager.leave == null)
        {
            gameManager.BeginChoicePhase();
        } else
        {
            gameManager.NextTrial();
        }
    }

}
