#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// For random rotate and scale child objects (Created for trees and rocks).
/// </summary>
public class RandomRotateScaleChilds :MonoBehaviour
{

	[SerializeField] Vector3 MinEulerAngles = new Vector3(0, -180, 0);
	[SerializeField] Vector3 MaxEulerAngles = new Vector3(0, 180, 0);

	[SerializeField] float MinScale = 0.9f;
	[SerializeField] float MaxScale = 1.1f;

	[ContextMenu ("UpdateRandom")]
	public void UpdateRandomScaleAndRotation ()
	{
		Vector3 newEulerAngles = Vector3.one;
		Vector3 newScale = Vector3.one;
		System.Random random = new System.Random ();
		for (int i = 0; i < transform.childCount; i++)
		{

			newEulerAngles.x = random.Next (MinEulerAngles.x, MaxEulerAngles.x);
			newEulerAngles.y = random.Next (MinEulerAngles.y, MaxEulerAngles.y);
			newEulerAngles.z = random.Next (MinEulerAngles.z, MaxEulerAngles.z);

			var scale = random.Next (MinScale, MaxScale);
			newScale.x = scale;
			newScale.y = scale;
			newScale.z = scale;

			var child = transform.GetChild (i);
			child.localEulerAngles = newEulerAngles;
			child.localScale = newScale;
		}
	}
}

public static class FloatRandom
{

	public static float Next (this System.Random random, float minValue, float maxValue)
	{
		float value = maxValue - minValue;
		value *= (float)random.NextDouble ();
		value += minValue;
		return value;
	}

}

#endif
