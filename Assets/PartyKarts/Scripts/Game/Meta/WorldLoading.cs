using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBalance;

/// <summary>
/// To select boot options from the main menu
/// </summary>
public class WorldLoading :MonoBehaviour
{
	public static string BotName = "Bot";						//TODO Add bots names.
	public static CarPreset PlayerCar;
	public static TrackPreset LoadingTrack;
	public static bool IsMultiplayer = false;

	public static string PlayerName { get { return PlayerProfile.NickName; } }
	public static bool HasLoadingParams { get { return PlayerCar != null; } }
	public static int LapsCount { get { return HasLoadingParams ? LoadingTrack.LapsCount: 3; } }
	public static int AIsCount { get { return GameOptions.EnableAI? LoadingTrack.AIsCount: 0; } }
	public static List<CarPreset> AvailableCars { get { return RegimeSettings.AvailableCars; } }
	public static RegimeSettings RegimeSettings { get { return LoadingTrack != null ? LoadingTrack.RegimeSettings: RegimeForDebug?? B.GameSettings.DefaultRegimeSettings; } }
	public static CarColorPreset SelectedColor { get { return PlayerProfile.GetCarColor (PlayerCar); } }

	public static RegimeSettings RegimeForDebug { get; set; }

}
