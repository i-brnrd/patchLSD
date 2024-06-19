using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{


    // I would just publicly assign the box, the presentation, here as a publicly accessible game object. 

    public GameObject box;
    public GameObject circle; 
    private Vector3 originalScale;

    public float[] patch;
    private float reward;
    private int count;





    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        patch = new float[] { 0.0f, 0.0f, 0.8f, 0.0f, 0f, 0f, 0.8f, 0f, 0f, 0f, 0.9f, 0f, 1.0f };
        originalScale = circle.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            reward = patch[count];
            Vector3 newScale = new Vector3(originalScale.x, originalScale.y * reward, originalScale.z);
            circle.transform.localScale = newScale;
            count++;
        }
    }
}