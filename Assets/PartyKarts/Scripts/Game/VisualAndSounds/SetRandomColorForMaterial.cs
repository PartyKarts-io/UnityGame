using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRandomColorForMaterial : MonoBehaviour {

	[SerializeField] List<Color> Colors = new List<Color>();

	public void Awake ()
	{
		if (Colors.Count == 0) return;
		var color = Colors.RandomChoice ();
		var props = new MaterialPropertyBlock ();
		props.SetColor ("_Color", color);

		var renderer = GetComponent<Renderer> ();
		renderer.SetPropertyBlock (props);
	}
}
