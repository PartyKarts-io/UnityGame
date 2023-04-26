using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// For save and load all game options (Quality, control type, camera and others).
/// </summary>
public static class GameOptions
{

	/// <summary>
	/// Load parameters.
	/// </summary>
	[RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.AfterSceneLoad)]
	static void OnLoadScene ()
	{
		UpdateAudioMixer ();
		CurrentQuality = CurrentQuality;
	}

	#region Control

	public static event System.Action<ControlType> OnControlChanged;

	/// <summary>
	/// Control type for mobile.
	/// </summary>
	public static ControlType CurrentControl
	{
		get
		{
			return (ControlType)PlayerPrefs.GetInt (C.ControlPrefName, 0);
		}
		set
		{
			PlayerPrefs.SetInt (C.ControlPrefName, (int)value);
			OnControlChanged.SafeInvoke (value);
		}
	}

	/// <summary>
	/// Last changed camera.
	/// </summary>
	public static int ActiveCameraIndex
	{
		get
		{
			return PlayerPrefs.GetInt (C.CameraIndexPrefName, 0);
		}
		set
		{
			PlayerPrefs.SetInt (C.CameraIndexPrefName, value);
		}
	}

	#endregion //Control

	#region Quality

	public static event System.Action OnQualityChanged;

	/// <summary>
	/// Last changed quality.
	/// </summary>
	public static int CurrentQuality
	{
		get
		{
			int value;
			if (PlayerPrefs.HasKey (C.QualityPrefName))
			{
				value = PlayerPrefs.GetInt (C.QualityPrefName);
			}
			else
			{
				value = QualitySettings.GetQualityLevel ();
			}
			return value;
		}
		set
		{
			if (QualitySettings.GetQualityLevel () != value)
			{
				QualitySettings.SetQualityLevel (value);
				PlayerPrefs.SetInt (C.QualityPrefName, value);
				OnQualityChanged.SafeInvoke ();
			}
			Application.targetFrameRate = B.GraphicsSettings.TargetFPS;
			Shader.globalMaximumLOD = (value + 2) * 100;
		}
	}

	#endregion //Quality

	#region Audio

	/// <summary>
	/// Last changed sound mute flag.
	/// </summary>
	public static bool SoundIsMute
	{
		get
		{
			return PlayerPrefs.GetInt (C.MutePrefName, 0) == 1;
		}
		set
		{
			PlayerPrefs.SetInt (C.MutePrefName, value ? 1 : 0);
			UpdateAudioMixer ();
		}
	}

	/// <summary>
	/// Set actual snapshot.
	/// </summary>
	public static void UpdateAudioMixer ()
	{

		var snapshot = B.SoundSettings.StandartSnapshot;

		if (SoundIsMute)
		{
			snapshot = B.SoundSettings.MuteSnapshot;
		}
		else
		{
			if (Mathf.Approximately (Time.timeScale, 0))
			{
				snapshot = B.SoundSettings.PauseSnapshot;
			}
		}

		snapshot.TransitionTo (0.5f);
	}

	#endregion //Audio

	#region Other

	public static bool EnableAI
	{
		get
		{
			return PlayerPrefs.GetInt (C.EnableAIName, 1) == 1;
		}
		set
		{
			PlayerPrefs.SetInt (C.EnableAIName, value ? 1 : 0);
		}
	}

	#endregion //Other
}
