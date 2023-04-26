using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Car in main menu, for set color.
/// </summary>
public class CarInSelectMenuPrefab :MonoBehaviour, ISetColor
{

	[SerializeField] List<SetColorForMaskMaterial> SetColorObjects = new List<SetColorForMaskMaterial>();

	public void SetColor (CarColorPreset colorPreset)
	{
		SetColorObjects.ForEach (c => c.SetColor (colorPreset));
	}
}
