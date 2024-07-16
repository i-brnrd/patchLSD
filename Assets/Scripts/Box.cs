using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    // Presentation Objects
    private SpriteRenderer box;
    private GameObject contentsMask;
    private GameObject environment;

    private Vector3 originalScale;

    // setup refs to the environment (colour) & contents (gold bars) 
    private void Awake()
    {
        contentsMask = gameObject.transform.Find("Contents").gameObject;
        environment = gameObject.transform.Find("Environment").gameObject;
        box = environment.GetComponent<SpriteRenderer>();
        originalScale = contentsMask.transform.localScale;
    }

    public void SetBoxColour(Color colour)
    {
        box.color = colour;
    }

    public void SetBarHeight(float height)
    {
        contentsMask.transform.localScale = new Vector3 (originalScale.x, originalScale.y * height, originalScale.z);
    }
}

