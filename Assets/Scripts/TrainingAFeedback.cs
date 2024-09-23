using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TrainingAFeedback : MonoBehaviour
{
    //this needs simplified the choice logic has gone ott and can be combined with LSD script 
    public GameManager gameManager;
    private PatchManager patchManager;
    private LSLStream lslStream;

    public GameObject changingButton;
    public GameObject redButton;


    public Color originalColour;

    public float highlightDuration = 1f;

    private void Awake()
    {
        patchManager = gameManager.GetComponent<PatchManager>();
        originalColour = changingButton.GetComponent<Button>().colors.normalColor;

        lslStream = gameManager.GetComponent<LSLStream>();


        //make sure on start all objects in the LSD are inactive 
        DeactivateObjects();
    }

   
    public IEnumerator Choice()
    {
        // Show question mark before displaying the buttons
       
        yield return new WaitForSeconds(0.1f);

        ActivateButtons(true);
#if UNITY_STANDALONE_WIN
        lslStream.TriggerLSLEvent("Now Showing Changing or Red Buttons");
#endif


        Interactable(true);
        AddListeners();
    }

    private void OnChangingChoice()
    {
#if UNITY_STANDALONE_WIN
        lslStream.TriggerLSLEvent("Selected Blue/ Changing");
#endif
        StartCoroutine(HandleButtonClick(changingButton, false));
    }

    private void OnRedChoice()
    {
#if UNITY_STANDALONE_WIN
        lslStream.TriggerLSLEvent("Selected Red/ Default");
#endif
        StartCoroutine(HandleButtonClick(redButton, true));
    }

    private IEnumerator HandleButtonClick(GameObject button, bool red)
    {
        // Make buttons non-interactable
        Interactable(false);
        button.GetComponent<Image>().color = Color.yellow;

        // Wait for the highlight duration
        yield return new WaitForSeconds(highlightDuration);

        // Revert the button color
        button.GetComponent<Image>().color = originalColour;

        DeactivateObjects();

        patchManager.TrainingAChoice(red);
    

    }

    private void DeactivateObjects()
    {
        ActivateButtons(false);
    }

    private void ActivateButtons(bool active)
    {
        changingButton.SetActive(active);
        redButton.SetActive(active);
    }

    private void Interactable(bool active)
    {
        changingButton.GetComponent<Button>().interactable = active;
        redButton.GetComponent<Button>().interactable = active;

    }

    private void AddListeners()
    {
        changingButton.GetComponent<Button>().onClick.AddListener(OnChangingChoice);
        redButton.GetComponent<Button>().onClick.AddListener(OnRedChoice);
    }
}
