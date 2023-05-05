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
using Unity.VisualScripting.Antlr3.Runtime;

/// <summary>
/// List of available rooms and the ability to create a new one.
/// </summary>
public class RoomListUI : MonoBehaviour
{
    [SerializeField] RoomItemUI RoomItemUIRef;
    [SerializeField] Button ConnectToRandomRoomButton;
    [SerializeField] Button CreateRoomButton;
    [SerializeField] Button CancelFeeSelectionButton;
    [SerializeField] Button ConfirmFeeButton;
    [SerializeField] TMP_Dropdown ServerList;
    [SerializeField] Window FeeSelectionMenu;
    [SerializeField] TMP_Dropdown FeeSelector;
    [SerializeField] SelectTrackUI SelectTrackUI;

    [Space(10)]
    [SerializeField] TextMeshProUGUI PingText;
    [SerializeField] Image PingIndicatorImage;
    [SerializeField] string AutoText = "(Auto) ";

    ThirdwebSDK sdk;

    List<RoomItemUI> RoomItems = new List<RoomItemUI>();

    List<string> Tokens = new List<string>();

    private int raceFee = 0;
    private bool awaitingTransaction = false;
    Hashtable selectedTrack = new();

    float Timer = 0;
    bool IsMaster { get { return PhotonNetwork.IsMasterClient; } }
    Room CurrentRoom { get { return PhotonNetwork.CurrentRoom; } }

    private void Start()
    {
        ConnectToRandomRoomButton.onClick.AddListener(ConnectToRandomRoom);
        CreateRoomButton.onClick.AddListener(ShowFeeSelector);
        ConfirmFeeButton.GetComponent<CustomButton>().interactable = true;
        ConfirmFeeButton.onClick.AddListener(ShowTrackSelector);
        CancelFeeSelectionButton.onClick.AddListener(HideFeeSelector);
        FeeSelector.onValueChanged.AddListener(SetRaceFee);

        RoomItemUIRef.SetActive(false);
        CreateRoomButton.interactable = false;
        sdk = ThirdwebManager.Instance.SDK;
    }

    void OnEnable()
    {
        Timer = 0;
        ServerList.Select();
        UpdateServerList();
    }

    async void Update()
    {
        bool photonIsConnected = PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InLobby;
        bool walletIsConnected = await ThirdwebManager.Instance.SDK.wallet.IsConnected();
        bool enableButtons = !awaitingTransaction && photonIsConnected && (Utils.IsWebGLBuild() ? walletIsConnected : true);

        // TOGGLE THIS FOR LOCAL DEV
        CreateRoomButton.interactable = enableButtons;
        ConnectToRandomRoomButton.interactable = enableButtons;

        var firstOption = ServerList.options[0];
        if (photonIsConnected && firstOption.text == AutoText && ServerList.value == 0)
        {
            var server = B.MultiplayerSettings.Servers.FirstOrDefault(s => s.ServerToken == PhotonNetwork.NetworkingClient.CloudRegion);
            UpdateServerList(server.ServerCaption);
        }

        if (Timer <= 0 && photonIsConnected)
        {
            UpdatePing();
            Timer = B.MultiplayerSettings.PingUpdateSettings;
        }
        Timer -= Time.deltaTime;
    }

    void ConnectToRandomRoom()
    {
        Hashtable customProperties = new Hashtable();
        customProperties.Add(C.RandomRoom, true);
        PhotonNetwork.JoinRandomRoom(customProperties, 0, MatchmakingMode.RandomMatching, null, null);
    }

    public void CreateRandomRoom()
    {

        //set custom properties.
        Hashtable customProperties = new Hashtable();
        customProperties.Add(C.RandomRoom, true);

        string[] customPropertiesForLobby = new string[1];
        customPropertiesForLobby[0] = C.RandomRoom;

        RoomOptions options = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = B.MultiplayerSettings.MaxPlayersInRoom,
            CustomRoomProperties = customProperties,
            CustomRoomPropertiesForLobby = customPropertiesForLobby,
        };

        PhotonNetwork.CreateRoom(System.Guid.NewGuid().ToString(), options);
    }

    void ShowFeeSelector()
    {
        FeeSelectionMenu.SetActive(true);
    }

    void HideFeeSelector()
    {
        raceFee = 0;
        FeeSelectionMenu.SetActive(false);
    }

    public void SetRaceFee(int fee)
    {
        switch (fee)
        {
            case 0:
                raceFee = 0;
                break;
            case 1:
                raceFee = 10;
                break;
            case 2:
                raceFee = 50;
                break;
            case 3:
                raceFee = 100;
                break;
            case 4:
                raceFee = 500;
                break;
            case 5:
                raceFee = 1000;
                break;
            default:
                raceFee = 5000;
                break;
        }
    }

    public void ShowTrackSelector()
    {
        FeeSelectionMenu.SetActive(false);
        WindowsController.Instance.OpenWindow(SelectTrackUI);
        SelectTrackUI.OnSelectTrackAction = OnSelectTrack;
    }

    void OnSelectTrack(TrackPreset trackPreset)
    {
        selectedTrack.Add(C.TrackName, trackPreset.name);
        WindowsController.Instance.OnBack();
        CreateRoom();
    }

    async void CreateRoom()
    {
        string raceId = System.Guid.NewGuid().ToString();

        TMP_Text buttonText = CreateRoomButton.GetComponentInChildren<TMP_Text>();
        buttonText.text = "Awaiting wallet confirmation...";

        string trackName = (string)selectedTrack[C.TrackName];
        BigInteger raceFee = BigInteger.Parse($"{this.raceFee}"); // convert race fee option to wei (10^16 wei = 0.01 MATIC)

        //set custom properties.
        Hashtable customProperties = new Hashtable();
        customProperties.Add(C.EntryFee, raceFee.ToString());
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

            buttonText.text = "Create Room";
            if (newRaceResult.isSuccessful())
            {
                Debug.Log("Creating Race Lobby with ID: " + raceId);

                PhotonNetwork.CreateRoom(raceId, options);
            }
            else
            {
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

        try
        {
            Contract contract = ThirdwebManager.Instance.pkRaceContract;
            Debug.Log("TRANSACTION INITIATED");

            TransactionResult txnResult = await contract.Write(
                    "createRaceLobby",
                    raceFee,
                    raceId,
                    trackName,
                    maxRacers,
                    isVisible,
                    isOpen
            );

            awaitingTransaction = false;

            return txnResult;
        }
        catch (Exception e)
        {
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
            RoomItemUI lobbyItem = RoomItems.FirstOrDefault(r => r.Room != null && r.Room.Name == room.Name);

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

    private void JoinRoom(RoomItemUI room)
    {
        PhotonNetwork.JoinRoom(room.Room.Name);
    }


    void UpdateServerList(string autoRegion = "")
    {
        Tokens = new List<string>();

        ServerList.ClearOptions();
        var options = new List<TMP_Dropdown.OptionData>();

        options.Add(new TMP_Dropdown.OptionData(AutoText + autoRegion));
        Tokens.Add("");

        foreach (var server in B.MultiplayerSettings.Servers)
        {
            options.Add(new TMP_Dropdown.OptionData(server.ServerCaption));
            Tokens.Add(server.ServerToken);
        }

        ServerList.AddOptions(options);
        ServerList.value = Tokens.FindIndex(t => t == PlayerProfile.ServerToken);
        ServerList.onValueChanged.AddListener((int value) =>
        {
            PlayerProfile.ServerToken = Tokens[value];
            LobbyManager.ConnectToServer();
            Timer = 0;
        });
    }

    void UpdatePing()
    {
        var ping = PhotonNetwork.GetPing();
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
