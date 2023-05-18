using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Michsky.UI.Reach;

/// <summary>
/// To display data about the player in the room.
/// </summary>
public class PlayerItemInRoomUI : MonoBehaviour {

	[SerializeField] LobbyPlayer LobbyPlayer;
	Player TargetPlayer;

	void Update ()
	{
		if (!TargetPlayer.IsLocal) return;
	}


	public void UpdateProperties (Player targetPlayer, System.Action kickAction)
	{
		TargetPlayer = targetPlayer;

		var customProperties = targetPlayer.CustomProperties;

		if ((bool)customProperties[C.IsReady])
		{
			LobbyPlayer.SetReady();
		} else
		{
			LobbyPlayer.SetNotReady();
		}

		LobbyPlayer.SetPlayerName(targetPlayer.TruncatedName);
		LobbyPlayer.SetAdditionalText((string)customProperties[C.CarName]);
	}
}
