using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used on objects that should be turned off if shadows are on.
/// </summary>
public class HideObjectIfShadowsEnabled :MonoBehaviour
{

	void Awake ()
	{
		GameOptions.OnQualityChanged += OnQualityChanged;
		OnQualityChanged ();
	}

	private void OnDestroy ()
	{
		GameOptions.OnQualityChanged -= OnQualityChanged;
	}

	void OnQualityChanged ()
	{
		gameObject.SetActive (QualitySettings.shadows == ShadowQuality.Disable);
	}
}
