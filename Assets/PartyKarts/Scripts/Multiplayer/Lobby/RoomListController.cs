using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Thirdweb;
using Org.BouncyCastle.Bcpg;
using System.Numerics;
using System.Threading.Tasks;
using System;
using System.Text;
using GameBalance;
using Michsky.UI.Reach;

/// <summary>
/// List of available rooms and the ability to create a new one.
/// </summary>
public class RoomListController : MonoBehaviour
{
    [SerializeField] RoomItemController RoomItemUIRef;
    [SerializeField] ButtonManager CreateRoomButton;
    [SerializeField] Michsky.UI.Reach.Dropdown FeeSelector;
    [SerializeField] PanelManager MenuPanelManager;
    [SerializeField] ModeSelector ServerSelector;
    [SerializeField] UIPopup TxnPendingToast;

    [Space(10)]
    [SerializeField] Image PingIndicatorImage;
    [SerializeField] string AutoText = "(Auto) ";

    ThirdwebSDK sdk;

    List<RoomItemController> RoomItems = new List<RoomItemController>();

    List<string> Tokens = new List<string>();

    private int raceFee = 0;
    private bool awaitingTransaction = false;
    Hashtable selectedTrack = new();

    float Timer = 0;
    bool IsMaster { get { return PhotonNetwork.IsMasterClient; } }
    Room CurrentRoom { get { return PhotonNetwork.CurrentRoom; } }

    private void Start()
    {
        FeeSelector.onValueChanged.AddListener(SetRaceFee);

        RoomItemUIRef.SetActive(false);
        CreateRoomButton.isInteractable = false;
        sdk = ThirdwebManager.Instance.SDK;
    }

    void OnEnable()
    {
        Timer = 0;
    }

    async void Update()
    {
        bool photonIsConnected = PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InLobby;
        bool walletIsConnected = await ThirdwebManager.Instance.SDK.wallet.IsConnected();
        bool enableButtons = !awaitingTransaction && photonIsConnected && (Utils.IsWebGLBuild() ? walletIsConnected : true);

        // TOGGLE THIS FOR LOCAL DEV
        CreateRoomButton.Interactable(enableButtons);

        var firstOption = ServerSelector.items[0];
        if (photonIsConnected && ServerSelector.currentModeIndex == 0)
        {
            var server = B.MultiplayerSettings.Servers.FirstOrDefault();
        }

        if (Timer <= 0 && photonIsConnected)
        {
            UpdatePing();
            Timer = B.MultiplayerSettings.PingUpdateSettings;
        }
        Timer -= Time.deltaTime;
    }

    public void SetRaceFee(int fee)
    {
        // sets the race fee to the Qty of tokens required based on the dropdown selection
        switch (fee)
        {
            case 0:
                raceFee = 0;
                break;
            case 1:
                raceFee = 5000;
                break;
            case 2:
                raceFee = 10000;
                break;
            case 3:
                raceFee = 20000;
                break;
            case 4:
                raceFee = 50000;
                break;
            case 5:
                raceFee = 100000;
                break;
            case 6:
                raceFee = 150000;
                break;
            case 7:
                raceFee = 250000;
                break;
            case 8:
                raceFee = 500000;
                break;
            default:
                raceFee = 0;
                break;
        }

        Debug.Log($"Race Fee Selected: {raceFee}");
    }

    async void CreateRoom()
    {
        string raceId = System.Guid.NewGuid().ToString();
        CreateRoomButton.SetText("Awaiting wallet confirmation...");

        string trackName = (string)selectedTrack[C.TrackName];
        BigInteger bigintFee = BigInteger.Parse($"{raceFee}"); // convert race fee option to wei (10^16 wei = 0.01 MATIC)

        //set custom properties.
        Hashtable customProperties = new Hashtable();
        customProperties.Add(C.EntryFee, bigintFee.ToString());
        customProperties.Add(C.RoomCreator, PhotonNetwork.NickName);
        customProperties.Add(C.TrackName, trackName);

        //set custom properties for display in room list.
        string[] customPropertiesForLobby = new string[3];
        customPropertiesForLobby[0] = C.RoomCreator;
        customPropertiesForLobby[1] = C.TrackName;
        customPropertiesForLobby[2] = C.EntryFee;

        byte maxPlayers = B.MultiplayerSettings.MaxPlayersInRoom;

        RoomOptions options = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = maxPlayers,
            CustomRoomProperties = customProperties,
            CustomRoomPropertiesForLobby = customPropertiesForLobby,
        };

        if (!Utils.IsWebGLBuild())
        {
            PhotonNetwork.CreateRoom(raceId, options);
        }
        else
        {
            TransactionResult newRaceResult = await CreateRoomTransaction(raceId, trackName, maxPlayers, true, true);

            CreateRoomButton.SetText("Create Room");

            if (newRaceResult.isSuccessful())
            {
                // TODO - Analytics - Log Create Room 
                PhotonNetwork.CreateRoom(raceId, options);
            }
            else
            {
                // TODO - Analytics - Log Create Room Failed

                // show error message popup or something
                Debug.LogError(newRaceResult);
            }
        }
    }

    private async Task<TransactionResult> CreateRoomTransaction(
        string raceId,
        string trackName,
        uint maxRacers,
        bool isVisible,
        bool isOpen
    )
    {
        awaitingTransaction = true;
        TxnPendingToast.PlayIn();
        try
        {
            Contract contract = ThirdwebManager.Instance.pkRaceContract;
            Debug.Log("TRANSACTION INITIATED");

            TransactionResult txnResult = await contract.Write(
                    "createRaceLobby",
                    $"{ThirdwebManager.Instance.ConvertEtherToWei(raceFee.ToString())}",
                    raceId,
                    trackName,
                    maxRacers,
                    isVisible,
                    isOpen
            );

            TxnPendingToast.PlayOut();
            awaitingTransaction = false;

            return txnResult;
        }
        catch (Exception e)
        {
            TxnPendingToast.PlayOut();
            awaitingTransaction = false;
            Debug.LogError(e);
            return null;
        }
    }


    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var roomItem in RoomItems)
        {
            if (roomItem != null && (roomItem.Room == null || roomItem.Room.RemovedFromList))
            {
                Destroy(roomItem.gameObject);
            }
        }

        RoomItems.RemoveAll(i => i == null);

        foreach (RoomInfo room in roomList)
        {
            if (room.CustomProperties != null && room.CustomProperties.ContainsKey(C.RandomRoom))
            {
                continue;
            }

            //Get or create room item.
            RoomItemController lobbyItem = RoomItems.FirstOrDefault(r => r.Room != null && r.Room.Name == room.Name);

            if (lobbyItem == null)
            {
                lobbyItem = Instantiate(RoomItemUIRef, RoomItemUIRef.transform.parent);
                RoomItems.Add(lobbyItem);
            }



            //Check custom properties.
            if (room.CustomProperties == null ||
                room.PlayerCount == 0 ||
                !room.IsOpen ||
                !room.CustomProperties.ContainsKey(C.RoomCreator) ||
                !room.CustomProperties.ContainsKey(C.TrackName)
            )
            {
                lobbyItem.SetActive(false);
                continue;
            }

            var fee = (string)room.CustomProperties[C.EntryFee];
            var trackName = (string)room.CustomProperties[C.TrackName];
            var track = B.MultiplayerSettings.AvailableTracksForMultiplayer.Find(t => t.name == trackName);

            string creatorName = (string)room.CustomProperties[C.RoomCreator];

            lobbyItem.UpdateInfo(
                room,
                track.TrackIcon,
                track.RegimeSettings.RegimeImage,
                creatorName,
                $"{fee} WEI" ?? "NO FEE",
                track.TrackName,
                string.Format("{0}/{1}", room.PlayerCount, room.MaxPlayers),
                () => JoinRoom(lobbyItem)

            );

            lobbyItem.SetActive(true);
        }
    }

    private void JoinRoom(RoomItemController room)
    {
        PhotonNetwork.JoinRoom(room.Room.Name);
    }

    public void SelectServer()
    {
        var selection = ServerSelector.items[ServerSelector.currentModeIndex];
        PlayerProfile.ServerToken = selection.titleKey;
        LobbyManager.ConnectToServer();
        Timer = 0;
    }

    void UpdatePing()
    {
        var ping = PhotonNetwork.GetPing();
        //PingText.text = ping.ToString();

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

    public void OnDisconnected()
    {
        if (RoomItems != null)
        {
            foreach (var roomItem in RoomItems)
            {
                if (roomItem != null)
                {
                    Destroy(roomItem.gameObject);
                }
            }
        }
    }

    public static string StringToHex(string value)
    {
        StringBuilder sb = new StringBuilder();

        foreach (char c in value)
        {
            sb.Append(Convert.ToInt32(c).ToString("x"));
        }

        return sb.ToString();
    }
}
