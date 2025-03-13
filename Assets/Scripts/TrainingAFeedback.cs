using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TrainingAFeedback : MonoBehaviour
{
    //this needs simplified the choice logic has gone ott and can be combined with LSD script 
    public GameObject trainingObject;
    private TrainingAController trainingAController;
    
    public GameObject changingButton;
    public GameObject redButton;

    public Color originalColour;

    public float highlightDuration = 1f;

    private void Awake()
    {
        trainingAController = trainingObject.GetComponent<TrainingAController>();
        originalColour = changingButton.GetComponent<Button>().colors.normalColor;
        //make sure on start all objects in the LSD are inactive 
        ActivateButtons(false);
    }

   
    public IEnumerator Choice()
    {
        // Show question mark before displaying the buttons
        yield return new WaitForSeconds(0.1f);

        ActivateButtons(true);
        Interactable(true);
        AddListeners();
    }

    private void OnChangingChoice()
    {
        StartCoroutine(HandleButtonClick(changingButton, false));
    }

    private void OnRedChoice()
    {
        StartCoroutine(HandleButtonClick(redButton, true));
    }

    private IEnumerator HandleButtonClick(GameObject button, bool leave)
    {
        // Make buttons non-interactable
        Interactable(false);
        button.GetComponent<Image>().color = Color.yellow;

        // Wait for the highlight duration
        yield return new WaitForSeconds(highlightDuration);

        // Revert the button color
        button.GetComponent<Image>().color = originalColour;

        ActivateButtons(false);

        trainingAController.TrainingAChoice(leave);
    

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
