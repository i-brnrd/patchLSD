using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LSD : MonoBehaviour
{
    public GameManager gameManager;
    private PatchManager patchManager;
    private LSLStream lslStream;

    public GameObject questionMark;
    public GameObject leaveButton;
    public GameObject stayButton;

    private Vector3 leftButtonPos;
    private Vector3 rightButtonPos;

    public Color originalColour;

    public float highlightDuration = 2f;

    private void Awake()
    {
        patchManager = gameManager.GetComponent<PatchManager>();
        leftButtonPos = leaveButton.transform.localPosition; //grabbing positions 
        rightButtonPos = stayButton.transform.localPosition;
        originalColour = leaveButton.GetComponent<Button>().colors.normalColor;

        lslStream = gameManager.GetComponent<LSLStream>();


        //make sure on start all objects in the LSD are inactive 
        DeactivateLSDObjects();
    }

    public void ChoicePhase()
    {
        StartCoroutine(Choice());
    }

    private IEnumerator Choice()
    {
        // Show question mark before displaying the buttons
        yield return StartCoroutine(QMark());
        bool isLeft = Random.value < 0.5f;

        ActivateButtons(true);
#if UNITY_STANDALONE_WIN
        lslStream.TriggerLSLEvent("8");
#endif
       
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
        Interactable(true);
        AddListeners();
    }

    private void OnLeaveChoice()
    {
        StartCoroutine(HandleButtonClick(leaveButton, true));
    }

    private void OnStayChoice()
    {
        StartCoroutine(HandleButtonClick(stayButton, false));
    }

    private IEnumerator HandleButtonClick(GameObject button, bool isLeave)
    {
        // Make buttons non-interactable
        Interactable(false);
        button.GetComponent<Image>().color = Color.yellow;

        // Wait for the highlight duration
        yield return new WaitForSeconds(highlightDuration);

        // Revert the button color
        button.GetComponent<Image>().color = originalColour;

        if (isLeave)
        {
            patchManager.ClickedLeaveLSD();
        } else
        {
            patchManager.ClickedStayLSD();
        }

        DeactivateLSDObjects();
    }

    private IEnumerator QMark()
    {
        questionMark.SetActive(true);
        yield return new WaitForSeconds(2);
        questionMark.SetActive(false);
    }

    private void DeactivateLSDObjects()
    {
        questionMark.SetActive(false);
        ActivateButtons(false);
    }

    private void ActivateButtons(bool active)
    {
        leaveButton.SetActive(active);
        stayButton.SetActive(active);
    }

    private void Interactable(bool active)
    {
        leaveButton.GetComponent<Button>().interactable = active;
        stayButton.GetComponent<Button>().interactable = active;
       
    }

    private void AddListeners()
    {
        leaveButton.GetComponent<Button>().onClick.AddListener(OnLeaveChoice);
        stayButton.GetComponent<Button>().onClick.AddListener(OnStayChoice);
    }
}
