using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Thirdweb;
using System.Threading.Tasks;
using Michsky.UI.Reach;
using TMPro;
using System.Threading;

/// <summary>
/// Control the logic of the multiplayer in the main menu.
/// </summary>

public class LobbyManager : MonoBehaviour, ILobbyCallbacks, IMatchmakingCallbacks, IConnectionCallbacks
{

    [SerializeField] PanelManager MainMenuManager;
    [SerializeField] RoomListController RoomListHolder;
    [SerializeField] RaceLobbyController RaceLobbyController;
    [SerializeField] UIPopup LobbyMessageToast;
    [SerializeField] TMP_Text LobbyMessageText;
    [SerializeField] string LeaveRoomMessage;

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
        PhotonNetwork.EnableCloseConnection = true;
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "usw";
        PhotonNetwork.ConnectUsingSettings();
        //System.Action leaveAction = () =>
        //{
        //    LeaveLobby();
        //};


        // TODO - Prompt Are you sure? before leaving this screen

        //CustomBackAction = () =>
        //{
        //    if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        //    {
        //        if (!MessageBox.HasActiveMessageBox)
        //        {
        //            MessageBox.Show(LeaveRoomMessage, leaveAction, null, "Yes", "Cancel");
        //        }
        //    }
        //    else
        //    {
        //        WindowsController.Instance.OnBack(ignoreCustomBackAction: true);
        //    }
        //};
    }

    public void LeaveLobby()
    {
        PhotonNetwork.LeaveRoom();
        MainMenuManager.OpenPanel("Lobby List");
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
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = true;
                PhotonNetwork.CurrentRoom.IsVisible = true;
            }
        }
    }

    public void UpdateHolders()
    {
        if (PhotonNetwork.InRoom)
        {
            MainMenuManager.OpenPanel("Race Lobby");
        }
    }

    public void OnJoinedRoom()
    {
        Debug.Log("On joined room");

        // TODO - Analytics - Log Joined Room
        RaceLobbyController.enabled = true;
        UpdateHolders();
    }

    public void OnLeftRoom()
    {
        Debug.Log("On left room");

        // TODO - Analytics - Log Left Room

        UpdateHolders();

        RaceLobbyController.RemoveAllPlayers();
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
        //LobbyMessageText.text = "Failed to create room. Please try again.";
        //LobbyMessageToast.PlayIn();
        //Thread.Sleep(4000);
        //LobbyMessageToast.PlayOut();
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogErrorFormat("Join room failed, error message: {0}", message);
        //LobbyMessageText.text = "Failed to join room. Please try again.";
        //LobbyMessageToast.PlayIn();
        //Thread.Sleep(4000);
        //LobbyMessageToast.PlayOut();
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
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
