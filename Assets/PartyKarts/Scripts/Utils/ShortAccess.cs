using UnityEngine;
using GameBalance;
using System.Numerics;

/// <summary> 
/// Fast access to settings.
/// B - Balance.
/// </summary>
public static class B
{

	static Settings _Settings;

	static Settings Settings
	{
		get
		{
			if (_Settings == null)
			{
				_Settings = Resources.Load<Settings> ("Settings");
			}
			return _Settings;
		}
	}

	public static GameSettings GameSettings { get { return Settings.GameSettings; } }
	public static GraphicsSettings GraphicsSettings { get { return Settings.GraphicsSettings; } }
	public static LayerSettings LayerSettings { get { return Settings.LayerSettings; } }
	public static SoundSettings SoundSettings { get { return Settings.SoundSettings; } }
	public static ResourcesSettings ResourcesSettings { get { return Settings.ResourcesSettings; } }
	public static MultiplayerSettings MultiplayerSettings { get { return Settings.MultiplayerSettings; } }
	public static float SpeedInHourMultiplier { get { return C.KPHMult; } }

}

/// <summary> 
/// Constants used in game
/// C - Constants
/// </summary>
/// 
public static class C
{
	//App constants
	public const string RoomId = "RoomId";
	public const string VersionPrefix = "Demo version";

	//Tag constants
	public const string ResetDriftTag = "ResetDriftCollision";
	public const string ResetCarTriggerTag = "ResetCarTrigger";

	//Pref constants
	public const string MutePrefName = "Mute";
	public const string EnableAIName = "EnableAI";
	public const string QualityPrefName = "Quality";
	public const string ControlPrefName = "Control";
	public const string CameraIndexPrefName = "CameraIndex";
	public const string ArrowControlPrefName = "ArrowControl";
	public const string SteerControlPrefName = "SteerControl";
	public const string AccelerometrControlPrefName = "AccelerometrControl";

	public const string TotalScorePrefName = "TotalScore";
	public const string BestScorePrefName = "BestScore";
	public const string TimeInRacePrefName = "TimeInRace";
	public const string BestRaceTimePrefName = "RaceTime";
	public const string BestLapTimePrefName = "LapTime";
	public const string TotalDistance = "TotalDistance";
	public const string Money = "Money";
	public const string Bought = "Bought";
	public const string Comleted = "Comleted";

	//Animation constants
	public const string ShowTrigger = "Show";
	public const string HideTrigger = "Hide";

	//CarParams constants
	public const string CarColorIndex = "cci";
	public const float MPHMult = 2.23693629f;
	public const float KPHMult = 3.6f;

	//Multiplayer constants
	public const string EntryFee = "ef";
	public const string NickName = "nn";
	public const string CarName = "cn";
	public const string IsReady = "ir";
	public const string IsLoaded = "il";
	public const string RoomCreator = "rc";
	public const string TrackName = "tn";
	public const string ServerToken = "st";
	public const string PlayerPing = "pt";
	public const string RandomRoom = "rr";

    public static BigInteger ONE_TEST_ETHER = new BigInteger(10000000000000000);
	public static BigInteger ONE_ETHER = new BigInteger(10000000000000000);

}

/// <summary>
/// Photon events codes
/// PE - Photon Event.
/// </summary>

public static class PE
{
	public const byte StartGame = 0;
	public const byte FinishRace = 1;
}
