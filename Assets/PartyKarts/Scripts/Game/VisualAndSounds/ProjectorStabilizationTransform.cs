using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectorStabilizationTransform : MonoBehaviour {

	CarController Car;
	void Start () {
		Car = GetComponentInParent<CarController> ();
		if (Car == null)
		{
			Debug.LogError ("CarController not found in parent");
			Destroy (gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		var carDirection = Car.transform.TransformDirection(Vector3.forward);
		carDirection.y = 0;

		transform.rotation = Quaternion.LookRotation (carDirection, Vector3.up);
	}
}
