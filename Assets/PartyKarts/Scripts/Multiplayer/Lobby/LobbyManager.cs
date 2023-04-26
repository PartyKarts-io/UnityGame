using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Thirdweb;
using System.Threading.Tasks;

/// <summary>
/// Control the logic of the multiplayer in the main menu.
/// </summary>

public class LobbyManager : WindowWithShowHideAnimators, ILobbyCallbacks, IMatchmakingCallbacks, IConnectionCallbacks
{

    [SerializeField] RoomListUI RoomListHolder;
    [SerializeField] InRoomUI InRoomHolder;
    [SerializeField] InRoomUI InRandomRoomHolder;
    private const string BASE_LEAVE_ROOM_MESSAGE = "Leave Lobby? You may withdraw your entry fee but you must pay gas for this transaction.";
    [SerializeField] string LeaveRoomMessage = BASE_LEAVE_ROOM_MESSAGE;
    CustomButton[] uiButtons;

    private bool _awaitingTransaction = false;

    public bool InRoom
    {
        get
        {
            return PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom;
        }
    }


    static public void ConnectToServer()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Disconnect();
        }

        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = PlayerProfile.ServerToken;
        PhotonNetwork.ConnectUsingSettings();
    }

    void Start()
    {
        uiButtons = GameObject.FindObjectsOfType<CustomButton>();

        //Initialize custom back action.

        PhotonNetwork.EnableCloseConnection = true;

        System.Action leaveAction = () =>
        {
            LeaveLobby();
        };

        CustomBackAction = () =>
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                if (!MessageBox.HasActiveMessageBox)
                {
                    MessageBox.Show(LeaveRoomMessage, leaveAction, null, "Yes", "Cancel");
                }
            }
            else
            {
                WindowsController.Instance.OnBack(ignoreCustomBackAction: true);
            }
        };
    }

    void LeaveLobby()
    {
        PhotonNetwork.LeaveRoom();
    }

    void OnEnable()
    {
        if (PhotonNetwork.NickName != PlayerProfile.NickName)
        {
            PhotonNetwork.NickName = PlayerProfile.NickName;
        }

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            ConnectToServer();
        }
        else if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        PhotonNetwork.AddCallbackTarget(this);
        UpdateHolders();
    }

    void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void CheckCurrentConnection()
    {
        if (InRoom)
        {
            WindowsController.Instance.OpenWindow(this);

            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = true;
                PhotonNetwork.CurrentRoom.IsVisible = true;
            }
        }
    }

    public void UpdateHolders()
    {
        bool inRoom = PhotonNetwork.InRoom;
        bool inRandomRoom = inRoom && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(C.RandomRoom);

        RoomListHolder.SetActive(!inRoom);
        InRoomHolder.SetActive(inRoom && !inRandomRoom);
        InRandomRoomHolder.SetActive(inRandomRoom);
    }

    public void OnJoinedRoom()
    {
        Debug.Log("On joined room");

        UpdateHolders();
    }

    public void OnLeftRoom()
    {
        Debug.Log("On left room");

        UpdateHolders();

        InRoomHolder.RemoveAllPlayers();
    }

    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (RoomListHolder.gameObject.activeInHierarchy)
        {
            RoomListHolder.OnRoomListUpdate(roomList);
        }
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogErrorFormat("Create room failed, error message: {0}", message);
        MessageBox.Show(message);
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogErrorFormat("Join room failed, error message: {0}", message);
        MessageBox.Show(message);
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        //Create room if not found available
        if (returnCode == 32760)
        {
            RoomListHolder.CreateRandomRoom();
        }
        else
        {
            Debug.LogErrorFormat("Join random room failed, error message: {0}", message);
            MessageBox.Show(message);
        }
    }

    #region Callbacks for log

    public void OnCreatedRoom()
    {
        Debug.Log("Room is created");
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        PhotonNetwork.JoinLobby();
    }

    public void OnJoinedLobby()
    {
        Debug.Log("On joined lobby");
    }

    public void OnLeftLobby()
    {
        Debug.Log("On left lobby");
    }

    public void OnConnected()
    {
        Debug.Log("Connected to Photon");
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        B.MultiplayerSettings.ShowDisconnectCause(cause);
        RoomListHolder.OnDisconnected();
    }

    #endregion //Callbacks for log

    #region EmptyCallbacks

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
    }

    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
    }

    #endregion //EmptyCallbacks
}
