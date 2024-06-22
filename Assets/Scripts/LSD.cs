using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSD : MonoBehaviour
{

    private GameObject questionMark;
    private GameObject leaveButton;
    private GameObject stayButton;

    private Vector3 leftButtonPos;
    private Vector3 rightButtonPos;


    private void Awake()
    {
        leaveButton = gameObject.transform.Find("LeaveButton").gameObject;
        stayButton = gameObject.transform.Find("StayButton").gameObject;
        questionMark = gameObject.transform.Find("ChoicePhase").gameObject;

        leftButtonPos = leaveButton.transform.localPosition;
        rightButtonPos = stayButton.transform.localPosition;

        //make sure on start all objects in the LSD are inactive 
        DeactivateLSDObjects();
    }

    public void ChoicePhase()
    {
        StartCoroutine(Choice());
    }

    private IEnumerator Choice()
    {
        questionMark.SetActive(true);
        yield return new WaitForSeconds(2);
        questionMark.SetActive(false);


        bool isLeft = Random.value < 0.5f;
        leaveButton.SetActive(true);
        stayButton.SetActive(true);

        if (isLeft)
        {
            leaveButton.transform.localPosition = leftButtonPos;
            stayButton.transform.localPosition = rightButtonPos;
        }
        else
        {
            leaveButton.transform.localPosition = rightButtonPos;
            stayButton.transform.localPosition = leftButtonPos;
        }

    }

    private void DeactivateLSDObjects()
    {
        questionMark.SetActive(false);
        leaveButton.SetActive(false);
        stayButton.SetActive(false);
    }


}
