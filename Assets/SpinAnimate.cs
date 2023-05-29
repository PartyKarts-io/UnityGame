using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAnimate : MonoBehaviour
{
    public float rotationSpeed = 150f; // Adjust the rotation speed as desired

    void Update()
    {
        // Rotate the object around its local Y axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
