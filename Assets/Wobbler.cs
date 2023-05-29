using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobbler : MonoBehaviour
{
    public float wobbleSpeed = 1f;     // Speed of the wobbling motion
    public float wobbleAmount = 1f;    // Amount of the wobbling motion
    public bool otherDirection = false;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the horizontal offset based on the sine wave
        float xOffset = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;

        // Apply the horizontal offset to the object's position
        transform.position = startPosition + (otherDirection ? new Vector3(0f, 0f, xOffset) :  new Vector3(xOffset, 0f, 0f));
    }
}
