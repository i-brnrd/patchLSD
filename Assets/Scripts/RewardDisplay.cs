using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardDisplay : MonoBehaviour
{
    public GameManager gameManager;

    public float heightFactor = 0.9f; // would get this from gameManager uytre
    // Presentation Objects
    private GameObject boxContents;
    private GameObject contentsMask;

    private Vector3 originalScale;
    private Vector3 originalPosition;


    private void Awake()
    {
        boxContents = gameObject.transform.Find("Contents").gameObject;
        //contentsMask = boxContents.transform.Find("ContentsMask").gameObject;

       // Debug.Log(contentsMask.name);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Store the original scale
        originalScale = boxContents.transform.localScale;
        originalPosition = contentsMask.transform.position;
        Debug.Log(originalPosition);
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            Vector3 newScale = new Vector3(originalScale.x, originalScale.y * heightFactor, originalScale.z);
            Debug.Log("New Scale" + newScale.ToString());
      
            float heightDifference = newScale.y - originalScale.y;
            Debug.Log("Height Difference" + heightDifference.ToString());
            // rescale and reposition surely! 
            contentsMask.transform.localScale = newScale;

            Debug.Log(contentsMask.transform.position.y);

            // reposition to where though? what is wrong with me 
            
            contentsMask.transform.position = new Vector3(originalPosition.x, originalPosition.y - heightDifference / 2, originalPosition.z);
            Debug.Log(contentsMask.transform.position.y);

            heightFactor = heightFactor - 0.1f;
        }
    }

}

