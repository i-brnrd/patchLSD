using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    // Presentation Objects
    private SpriteRenderer boxSprite;
    private GameObject contentsMask;
    private GameObject environment;

    private Vector3 originalScale;

    // setup refs to the environment (colour) & contents (gold bars) 
    private void Awake()
    {
        environment = gameObject.transform.Find("Environment").gameObject;
        contentsMask = gameObject.transform.Find("Contents").gameObject;

        boxSprite = environment.GetComponent<SpriteRenderer>();
        originalScale = contentsMask.transform.localScale;
    }

    public void SetBoxColour(Color colour)
    {
        boxSprite.color = colour;
    }

    public void SetBarHeight(float height)
    {
        contentsMask.transform.localScale = new Vector3 (originalScale.x, originalScale.y * height, originalScale.z);
    }
}

