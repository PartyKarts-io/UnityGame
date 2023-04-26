using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBalance;
using Thirdweb;
using System.Linq;

/// <summary>
/// For save and load player results. TODO Add save/load file.
/// </summary>
public static class PlayerProfile
{
	public static string NickName
	{
		get
		{
			if (!PlayerPrefs.HasKey (C.NickName))
			{
				PlayerPrefs.SetString (C.NickName, string.Format ("Player {0}", Random.Range (0, 99999)));
			}
			return PlayerPrefs.GetString (C.NickName);
		}
		set
		{
			if (!string.IsNullOrEmpty (value))
			{
				PlayerPrefs.SetString (C.NickName, value);
			}
		}
	}

    public static string TruncatedName
    {
        get
        {
            if (NickName.Length <= 8)
            {
                return NickName;
            }
            else
            {
                return NickName.Substring(0, 4) + "..." + NickName.Substring(NickName.Length - 4);
            }
        }
    }

    public static string ServerToken
	{
		get
		{
			if (!PlayerPrefs.HasKey (C.ServerToken))
			{
				return string.Empty;
			}
			return PlayerPrefs.GetString (C.ServerToken);
		}
		set
		{
			PlayerPrefs.SetString (C.ServerToken, value);
		}
	}

	public static int TotalScore
	{
		get
		{
			return PlayerPrefs.GetInt (C.TotalScorePrefName, 0);
		}
		set
		{
			PlayerPrefs.SetInt (C.TotalScorePrefName, value);
		}
	}

	public static float RaceTime
	{
		get
		{
			return PlayerPrefs.GetFloat (C.TimeInRacePrefName, 0);
		}
		set
		{
			PlayerPrefs.SetFloat (C.TimeInRacePrefName, value);
		}
	}


	public static float TotalDistance
	{
		get
		{
			return PlayerPrefs.GetFloat (C.TotalDistance, 0);
		}
		set
		{
			PlayerPrefs.SetFloat (C.TotalDistance, value);
		}
	}

	public static void SetRaceTimeForTrack (TrackPreset track, float time)
	{
		PlayerPrefs.SetFloat (string.Format ("{0}_{1}", C.BestRaceTimePrefName, track.name), time);
	}

	public static float GetRaceTimeForTrack (TrackPreset track)
	{
		return PlayerPrefs.GetFloat (string.Format ("{0}_{1}", C.BestRaceTimePrefName, track.name));
	}

	public static void SetBestLapForTrack (TrackPreset track, float time)
	{
		PlayerPrefs.SetFloat (string.Format ("{0}_{1}", C.BestLapTimePrefName, track.name), time);
	}

	public static float GetBestLapForTrack (TrackPreset track)
	{
		return PlayerPrefs.GetFloat (string.Format ("{0}_{1}", C.BestLapTimePrefName, track.name));
	}

	#region Unlocked logic

	// Unlocked and money prefs logic
	// !!!ATTENTION!!! the data is not protected.
	// TODO add save protected logic.
	public static int Money
	{
		get
		{
			return PlayerPrefs.GetInt (C.Money);
		}
		set
		{
			if (Money != value)
			{
				OnMoneyChanged.SafeInvoke (value);
				PlayerPrefs.SetInt (C.Money, value);
			}
		}

	}

	public static System.Action<int> OnMoneyChanged;

	public static void SetObjectAsBought (LockedContent obj)
	{
		PlayerPrefs.SetInt (string.Format("{0}_{1}", obj.name, C.Bought), 0);
	}

	public static bool ObjectIsBought (LockedContent obj)
	{
		return PlayerPrefs.HasKey (string.Format ("{0}_{1}", obj.name, C.Bought));
	}

	public static bool OwnsCorrectNFT(LockedContent obj)
	{
        // TODO - Check connected Wallet to see if the player owns the NFT for the correct car;
        // ThirdwebManager.Instance.SDK.wallet.
        List<NFT> nfts = ThirdwebManager.Instance.walletNFTs;
        return nfts.Any(nft => nft.metadata.name == obj.name);
	}

	public static void SetTrackAsComplited (TrackPreset track)
	{
		PlayerPrefs.SetInt (string.Format ("{0}_{1}", track.name, C.Comleted), 0);
	}

	public static bool TrackIsComplited (TrackPreset track)
	{
		return PlayerPrefs.HasKey (string.Format ("{0}_{1}", track.name, C.Comleted));
	}

	#endregion //Unlocked logic

	#region Best score

	public static int BestScore
	{
		get
		{
			return PlayerPrefs.GetInt (C.BestScorePrefName, 0);
		}
		set
		{
			PlayerPrefs.SetInt (C.BestScorePrefName, value);
		}
	}

	public static string BestScoreCar
	{
		get
		{
			return PlayerPrefs.GetString (C.BestScorePrefName + "_car", "--");
		}
		set
		{
			PlayerPrefs.SetString (C.BestScorePrefName + "_car", value);
		}
	}

	#endregion //Best score

	#region CarParams

	public static void SetCarColor (CarPreset car, CarColorPreset color)
	{
		PlayerPrefs.SetInt (string.Format ("{0}_{1}", C.CarColorIndex, car.name), car.AvailibleColors.IndexOf (color));
	}

	public static int GetCarColorIndex (CarPreset car)
	{
		return PlayerPrefs.GetInt (string.Format ("{0}_{1}", C.CarColorIndex, car.name), 0);
	}

	public static CarColorPreset GetCarColor (CarPreset car)
	{
		return car.AvailibleColors[GetCarColorIndex (car)];
	}

	#endregion //CarParams
}
