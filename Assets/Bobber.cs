using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobber : MonoBehaviour
{
    public float bobSpeed = 1f;     // Speed of the bobbing motion
    public float bobHeight = 1f;    // Height of the bobbing motion

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the vertical offset based on the sine wave
        float yOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        // Apply the vertical offset to the object's position
        transform.position = startPosition + new Vector3(0f, yOffset, 0f);
    }
}
