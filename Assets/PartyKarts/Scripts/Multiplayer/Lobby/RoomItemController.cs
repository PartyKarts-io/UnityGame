using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using Michsky.UI.Reach;
using Photon.Pun;

/// <summary>
/// To display room information in the room list.
/// </summary>
public class RoomItemController : MonoBehaviour {

	[SerializeField] Image TrackIcon;
	[SerializeField] Image RegimeIcon;
	[SerializeField] TextMeshProUGUI HostNickNameText;
	[SerializeField] TextMeshProUGUI FeeText;
	[SerializeField] TextMeshProUGUI SelectedTrackText;
	[SerializeField] TextMeshProUGUI PlayersText;
    [SerializeField] SettingsElement LobbyListItem;

    public RoomInfo Room { get; private set; }

	public void UpdateInfo (RoomInfo room, Sprite trackIcon, Sprite regimeIcon, string hostNickNameText, string feeText, string selectedTrackText, string playersText)
	{
		Room = room;
		TrackIcon.sprite = trackIcon;
		RegimeIcon.sprite = regimeIcon;
		HostNickNameText.text = TruncateAddress(hostNickNameText);
		FeeText.text = feeText;
		SelectedTrackText.text = selectedTrackText;
		PlayersText.text = playersText;

		LobbyListItem.onClick.AddListener(() => {
            PhotonNetwork.JoinRoom(room.Name);
        });
    }

    public string TruncateAddress(string address)
    {
        if (address.Length <= 10) // Check if address length is less than or equal to 10
            return address; // Return the address as is if it's already in truncated format

        string truncatedAddress = address.Substring(0, 4) + "...." + address[^4..]; // Truncate the address

        return truncatedAddress;
    }

}
