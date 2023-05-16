﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using Michsky.UI.Reach;

/// <summary>
/// To display room information in the room list.
/// </summary>
public class RoomItemUI : MonoBehaviour {

	[SerializeField] Image TrackIcon;
	[SerializeField] Image RegimeIcon;
	[SerializeField] TextMeshProUGUI HostNickNameText;
	[SerializeField] TextMeshProUGUI FeeText;
	[SerializeField] TextMeshProUGUI SelectedTrackText;
	[SerializeField] TextMeshProUGUI PlayersText;
	[SerializeField] SettingsElement OnClickButton;

	public RoomInfo Room { get; private set; }

	public void UpdateInfo (RoomInfo room, Sprite trackIcon, Sprite regimeIcon, string hostNickNameText, string feeText, string selectedTrackText, string playersText, UnityEngine.Events.UnityAction onClickAction)
	{
		Room = room;
		TrackIcon.sprite = trackIcon;
		RegimeIcon.sprite = regimeIcon;
		HostNickNameText.text = hostNickNameText;
		FeeText.text = feeText;
		SelectedTrackText.text = selectedTrackText;
		PlayersText.text = playersText;

		OnClickButton.onClick.RemoveAllListeners ();
		OnClickButton.onClick.AddListener (onClickAction);
	}

}
