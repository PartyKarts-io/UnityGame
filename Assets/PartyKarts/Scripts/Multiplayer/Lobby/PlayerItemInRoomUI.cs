using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// To display data about the player in the room.
/// </summary>
public class PlayerItemInRoomUI : MonoBehaviour {

	[SerializeField] TextMeshProUGUI PlayerNickNameText;
	[SerializeField] TextMeshProUGUI PlayerCarText;
	[SerializeField] Button KickButton;

	[SerializeField] GameObject ReadyObject;
	[SerializeField] GameObject NotReadyObject;
	[SerializeField] Image PingIndicatorImage;
	[SerializeField] TextMeshProUGUI PingText;

	//Vote tracks settings
	[SerializeField] GameObject SelectedTrackHolder;
	[SerializeField] Image TrackIconImage;
	[SerializeField] Image RegimeIconImage;
	[SerializeField] TextMeshProUGUI TrackNameText;

	float Timer;
	Player TargetPlayer;
	string trackName;

	void Update ()
	{
		if (!TargetPlayer.IsLocal) return;
		
		if (Timer <= 0)
		{
			UpdatePing ();
			Timer = B.MultiplayerSettings.PingUpdateSettings;
		}
		Timer -= Time.deltaTime;
	}

	void UpdatePing ()
	{
		Hashtable customProperties = new Hashtable();
		customProperties[C.PlayerPing] = PhotonNetwork.GetPing ();
		TargetPlayer.SetCustomProperties (customProperties);
	}

	public void UpdateProperties (Player targetPlayer, System.Action kickAction)
	{
		TargetPlayer = targetPlayer;

		var customProperties = targetPlayer.CustomProperties;

		ReadyObject.SetActive ((bool)customProperties[C.IsReady]);
		NotReadyObject.SetActive (!(bool)customProperties[C.IsReady]);

		PlayerNickNameText.text = targetPlayer.TruncatedName;

		PlayerCarText.text = (string)customProperties[C.CarName];

		if (kickAction == null)
		{
			KickButton.SetActive (false);
		}
		else
		{
			KickButton.interactable = !TargetPlayer.IsMasterClient;
			KickButton.SetActive (true);
			KickButton.onClick.RemoveAllListeners ();
			KickButton.onClick.AddListener (() => kickAction.SafeInvoke ());
		}

		if (SelectedTrackHolder != null) 
		{
			trackName = customProperties.ContainsKey (C.TrackName) ? (string)customProperties[C.TrackName] : string.Empty;

			if (!string.IsNullOrEmpty(trackName)) 
			{
				var track = B.MultiplayerSettings.AvailableTracksForMultiplayer.Find(t => t.name == trackName);
				SelectedTrackHolder.SetActive (true);

				TrackIconImage.sprite = track.TrackIcon;
				RegimeIconImage.sprite = track.RegimeSettings.RegimeImage;
				TrackNameText.text = string.Format ("{0}: {1}", track.TrackName, track.RegimeSettings.RegimeCaption);
			}
			else 
			{
				SelectedTrackHolder.SetActive (false);
			}
		}

		if (customProperties.ContainsKey (C.PlayerPing))
		{
			PingIndicatorImage.SetActive (true);
			int ping = (int)customProperties[C.PlayerPing];
			PingText.text = ping.ToString();

			if (ping <= B.MultiplayerSettings.VeryGoodPing)
			{
				PingIndicatorImage.sprite = B.MultiplayerSettings.VeryGoodPingSprite;
			}
			else if (ping <= B.MultiplayerSettings.GoodPing)
			{
				PingIndicatorImage.sprite = B.MultiplayerSettings.GoodPingSprite;
			}
			else if (ping <= B.MultiplayerSettings.MediumPing)
			{
				PingIndicatorImage.sprite = B.MultiplayerSettings.MediumPingSprite;
			}
			else
			{
				PingIndicatorImage.sprite = B.MultiplayerSettings.BadPingSprite;
			}
		}
		else
		{
			PingIndicatorImage.SetActive (false);
		}
	}
}
