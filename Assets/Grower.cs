using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grower : MonoBehaviour
{
    public float swellSpeed = 3f;       // Speed of the swelling motion
    public float swellAmount = 0.3f;    // Amount of the swelling motion

    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        // Calculate the scale based on a sine wave
        float scaleOffset = Mathf.Sin(Time.time * swellSpeed) * swellAmount;
        Vector3 newScale = initialScale + new Vector3(scaleOffset, scaleOffset, scaleOffset);

        // Apply the new scale to the object
        transform.localScale = newScale;
    }
}
