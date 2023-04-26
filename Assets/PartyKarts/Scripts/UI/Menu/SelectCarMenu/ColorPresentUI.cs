using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPresentUI :MonoBehaviour
{
	[SerializeField] Image MainColorImage;
	[SerializeField] Image SmoothnessImage;
	[SerializeField] GameObject SelectedObject;
	[SerializeField] float MaxSmoothness = 0.7f;
	[SerializeField] float InSelectScale = 1.2f;
	[SerializeField] float InDeselectScale = 0.8f;

	public Button GetButton { get { return GetComponent<Button> (); } }

	public void InitColor (CarColorPreset color)
	{
		MainColorImage.color = color.Color;
		SmoothnessImage.SetAlpha (color.Smoothness * MaxSmoothness);
	}

	public void SelectColor (bool select)
	{
		SelectedObject.SetActive (select);
		transform.localScale = Vector3.one * (select ? InSelectScale : InDeselectScale);
	}
}
