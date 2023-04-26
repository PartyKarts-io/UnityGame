using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBalance;

/// <summary>
/// Set car color ui.
/// TODO Add more costum parameters.
/// </summary>
public class CarSetColorUI :MonoBehaviour
{

	[SerializeField] ColorPresentUI ColorPresentRef;
	[SerializeField] float OffsetBetweenPresets = 40;
	[SerializeField] int MaxColors = 7;

	CarPreset CurrentCar;
	ISetColor CarOnScene;

	List<ColorPresentUI> Colors = new List<ColorPresentUI>();

	void Awake ()
	{
		ColorPresentRef.SetActive (false);
	}

	public void SelectCar (CarPreset newCar, ISetColor carOnScene)
	{
		CurrentCar = newCar;
		CarOnScene = carOnScene;
		UpdateColorForCurrentCar ();
	}

	void UpdateColorForCurrentCar ()
	{

		foreach (var color in Colors)
		{
			color.SelectColor (false);
			color.SetActive (false);
		}
		for (int i = 0; i < CurrentCar.AvailibleColors.Count; i++)
		{
			if (i + 1 > MaxColors)
			{
				Debug.LogErrorFormat ("In carPreset({0}) Available colors are greater than the maximum count (Max count is {1})", CurrentCar, MaxColors);   //TODO Add infinity select wheel.
				break;
			}
			ColorPresentUI color;
			if (Colors.Count <= i)
			{
				color = Instantiate (ColorPresentRef, ColorPresentRef.transform.parent);
				var rect = color.transform as RectTransform;
				var newPos = rect.anchoredPosition;
				newPos.x += (OffsetBetweenPresets + rect.sizeDelta.x) * i;
				rect.anchoredPosition = newPos;
				Colors.Add (color);
			}
			else
			{
				color = Colors[i];
			}
			color.SetActive (true);
			color.SelectColor (false);

			var newColor = CurrentCar.AvailibleColors[i];

			color.InitColor (newColor);

			var button = color.GetButton;
			button.onClick.RemoveAllListeners ();
			button.onClick.AddListener (() =>
			{

				Colors.ForEach (c => c.SelectColor (false));
				PlayerProfile.SetCarColor (CurrentCar, newColor);
				color.SelectColor (true);
				CarOnScene.SetColor (newColor);

			});

		}
		int index = PlayerProfile.GetCarColorIndex (CurrentCar);
		Colors[index].SelectColor (true);
		CarOnScene.SetColor (CurrentCar.AvailibleColors[index]);
	}
}
