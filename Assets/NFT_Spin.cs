using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class NFT_Spin : MonoBehaviour
{
    public float speed = 90.0f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(UnityEngine.Vector3.up, speed * Time.deltaTime);
    }
}
