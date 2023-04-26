using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Renderer))]
public class SetColorForMaskMaterial :MonoBehaviour
{
	/// <summary>
	/// Set color for the instantiating shader.
	/// </summary>
	/// <param name="color">Color preset</param>
	public void SetColor (CarColorPreset color)
	{
		var props = new MaterialPropertyBlock ();
		props.SetColor ("_Color", color.Color);
		props.SetFloat ("_Smoothness", color.Smoothness);

		var renderer = GetComponent<Renderer> ();
		renderer.SetPropertyBlock (props);
	}
}

[Serializable]
public struct CarColorPreset
{
	[SerializeField] Color m_Color;
	[SerializeField, Range(0, 1)] float m_Smoothness;

	public Color Color { get { return m_Color; } }
	public float Smoothness { get { return m_Smoothness; } }
}
