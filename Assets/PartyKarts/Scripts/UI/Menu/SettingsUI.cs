using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Settings window. TODO Add window for others platforms.
/// </summary>
public class SettingsUI :WindowWithShowHideAnimators
{

	[SerializeField] AudioClip ClickClip;

	[SerializeField] TMP_Dropdown QualityDropDown;

	[SerializeField] Button ArrowsControlButton;
	[SerializeField] Button SteerWheelControlButton;
	[SerializeField] Button AccelerometrControlButton;

	[SerializeField] Toggle MuteSoundToogle;
	[SerializeField] Toggle EnableAiToogle;

	[SerializeField] Button ChangeNickNameButton;

	void Start ()
	{
		StartQualityDropDown ();
		StartControlButtons ();
		StartSoundSettings ();
		StartOther ();
	}

	#region Quality

	void StartQualityDropDown ()
	{
		var options = new List<TMP_Dropdown.OptionData> ();
		foreach (var opName in QualitySettings.names)
		{
			options.Add (new TMP_Dropdown.OptionData (opName));
		}

		QualityDropDown.ClearOptions ();
		QualityDropDown.AddOptions (options);

		QualityDropDown.value = GameOptions.CurrentQuality;

		QualityDropDown.onValueChanged.RemoveAllListeners ();
		QualityDropDown.onValueChanged.AddListener (SetQuality);
		QualityDropDown.onValueChanged.AddListener ((value) => { SoundControllerInUI.PlayAudioClip (ClickClip); });
	}

	void SetQuality (int newValue)
	{
		GameOptions.CurrentQuality = newValue;
	}

	#endregion //Quality

	#region Control

	public static event System.Action OnControlChanged;

	void StartControlButtons ()
	{
		ArrowsControlButton.onClick.AddListener (OnArrowsControl);
		SteerWheelControlButton.onClick.AddListener (OnSteerWheelControl);
		AccelerometrControlButton.onClick.AddListener (OnAccelerometrControl);
		UpdateButtons ();
	}

	void UpdateButtons ()
	{
		ArrowsControlButton.interactable = GameOptions.CurrentControl != ControlType.Arrows;
		SteerWheelControlButton.interactable = GameOptions.CurrentControl != ControlType.SteerWheel;
		AccelerometrControlButton.interactable = GameOptions.CurrentControl != ControlType.Accelerometr;
	}

	void OnArrowsControl ()
	{
		GameOptions.CurrentControl = ControlType.Arrows;
		UpdateButtons ();
	}

	void OnSteerWheelControl ()
	{
		GameOptions.CurrentControl = ControlType.SteerWheel;
		UpdateButtons ();
	}

	void OnAccelerometrControl ()
	{
		GameOptions.CurrentControl = ControlType.Accelerometr;
		UpdateButtons ();
	}

	#endregion //Control

	#region Sound

	void StartSoundSettings ()
	{
		MuteSoundToogle.isOn = GameOptions.SoundIsMute;
		MuteSoundToogle.onValueChanged.RemoveAllListeners ();
		MuteSoundToogle.onValueChanged.AddListener (OnChangeMute);
		MuteSoundToogle.onValueChanged.AddListener ((value) => { SoundControllerInUI.PlayAudioClip (ClickClip); });
	}

	void OnChangeMute (bool value)
	{
		GameOptions.SoundIsMute = value;
	}

	#endregion //Sound

	#region Other

	void StartOther ()
	{
		if (EnableAiToogle)
		{
			EnableAiToogle.isOn = GameOptions.EnableAI;
			EnableAiToogle.onValueChanged.RemoveAllListeners ();
			EnableAiToogle.onValueChanged.AddListener(OnChangeAI);
		}

		if (ChangeNickNameButton != null && ChangeNickName.Instance != null)
		{
			ChangeNickNameButton.onClick.AddListener (ChangeNickName.Instance.Show);
		}
	}

	void OnChangeAI (bool value)
	{
		GameOptions.EnableAI = value;
	}

	#endregion //Other
}
