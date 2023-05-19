using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using GameBalance;
using Thirdweb;
using Unity.VisualScripting;
using System.Numerics;
using System.Threading.Tasks;
using System;
using Michsky.UI.Reach;

/// <summary>
/// To display and control the parameters in the room.
/// </summary>
public class RaceLobbyController : MonoBehaviour, IInRoomCallbacks, IOnEventCallback
{
    [SerializeField] PlayerItemInRoomUI PlayerItemUIRef;

    [SerializeField] PanelManager MainMenuPanel;
    [SerializeField] TMP_Text EntryFeeText;
    [SerializeField] ButtonManager ReadyButton;
    [SerializeField] ButtonManager BackButton;
    [SerializeField] ModeSelector CarSelector;
    [SerializeField] ModeSelector TrackSelector;
    [SerializeField] UIPopup TxnPendingToast;

    [SerializeField] int MinimumPlayersForStart = 2;

    private bool _awaitingTxn = false;

    //Connected players dictionary.
    Dictionary<Player, PlayerItemInRoomUI> Players = new Dictionary<Player, PlayerItemInRoomUI>();

    Room CurrentRoom { get { return PhotonNetwork.CurrentRoom; } }
    bool IsMaster { get { return PhotonNetwork.IsMasterClient; } }
    Player LocalPlayer { get { return PhotonNetwork.LocalPlayer; } }

    bool WaitStartGame;

    void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);

        //Initialized all buttons.
        PlayerItemUIRef.SetActive(false);
        ReadyButton.Interactable(false);
        TrackSelector.Interactable(false);
        CarSelector.Interactable(true);
        ReadyButton.onClick.AddListener(OnReadyClick);
        ReadyButton.UpdateUI();

        if (CurrentRoom == null)
        {
            MainMenuPanel.OpenPanel("Lobby List");
            return;
        }

        var entryFee = CurrentRoom.CustomProperties[C.EntryFee].ToString();
        EntryFeeText.text = $"{entryFee} $KART";

        TrackSelector.Interactable(IsMaster);
        OnRoomPropertiesUpdate(CurrentRoom.CustomProperties);

        foreach (var playerKV in Players)
        {
            Destroy(playerKV.Value.gameObject);
        }

        Players.Clear();

        foreach (var playerKV in CurrentRoom.Players)
        {
            TryUpdateOrCreatePlayerItem(playerKV.Value);
        }

        var localPlayer = PhotonNetwork.LocalPlayer;


        List<NFT> nfts = ThirdwebManager.Instance.walletNFTs;
        var selectedCar = WorldLoading.AvailableCars.FirstOrDefault(c => c.name == nfts[0].metadata.name);
        WorldLoading.PlayerCar = selectedCar;

        CarSelector.SelectMode(WorldLoading.AvailableCars.IndexOf(selectedCar));

        LocalPlayer.SetCustomProperties(C.CarName, selectedCar.CarCaption);
        LocalPlayer.SetCustomProperties(C.CarName, selectedCar.CarCaption, C.CarColorIndex, PlayerProfile.GetCarColorIndex(selectedCar), C.IsReady, false);

        //Choosing the first track when create the room.
        if (PhotonNetwork.IsMasterClient && (CurrentRoom.CustomProperties == null || !CurrentRoom.CustomProperties.ContainsKey(C.TrackName)))
        {
            var hashtable = new Hashtable();
            hashtable.Add(C.TrackName, B.GameSettings.Tracks.First().name);
            CurrentRoom.SetCustomProperties(hashtable);
        }

        localPlayer.SetCustomProperties(C.IsReady, false);
    }

    public virtual void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnSelectCar(string selectedCarName)
    {
        CarPreset car = B.MultiplayerSettings.AvailableCarsForMultiplayer.Find(c => c.name == selectedCarName);
        WorldLoading.PlayerCar = car;

        Debug.Log($"Selected Car: {car.name}");

        LocalPlayer.SetCustomProperties(C.CarName, car.CarCaption, C.CarColorIndex, PlayerProfile.GetCarColorIndex(car));
    }

    public void OnSelectTrack(string selectedTrackName)
    {
        TrackPreset track = B.MultiplayerSettings.AvailableTracksForMultiplayer.Find(t => t.name == selectedTrackName);
        var hashtable = new Hashtable();
        hashtable.Add(C.TrackName, track.name);

        Debug.Log($"Selected Track: {track.name}");

        if (IsMaster)
        {
            CurrentRoom.SetCustomProperties(hashtable);
        }
    }


   public async void OnReadyClick()
    {
        if (!Utils.IsWebGLBuild())
        {
            ReadyUp();
        }
        else
        {
            ReadyButton.Interactable(false);
            ReadyButton.UpdateUI();

            BackButton.Interactable(false);
            BackButton.UpdateUI();

            if (!(bool)LocalPlayer.CustomProperties[C.IsReady])
            {
                bool approveTxnResult = await ApproveKartToken(PhotonNetwork.CurrentRoom);
                if (approveTxnResult)
                {
                    TransactionResult joinTxnResult = await JoinLobbyTransaction(PhotonNetwork.CurrentRoom);

                    if (joinTxnResult.isSuccessful())
                    {
                        ReadyUp();
                    }
                    else
                    {
                        BackButton.Interactable(true);
                        Debug.LogError(joinTxnResult);
                    }
                }
                else
                {
                    BackButton.Interactable(true);
                    Debug.LogError("Error approving Race Fee");
                }
                BackButton.UpdateUI();
            }
        }
    }

    private void ReadyUp()
    {
        BackButton.Interactable(false);
        ReadyButton.Interactable(false);
        TrackSelector.Interactable(false);
        CarSelector.Interactable(false);
        ReadyButton.UpdateUI();
        BackButton.UpdateUI();

        // THIS IS WHERE YOU SET THE CAR COLOR
        LocalPlayer.SetCustomProperties(C.IsReady, true, C.CarColorIndex, PlayerProfile.GetCarColorIndex(WorldLoading.PlayerCar));
    }

    private async Task<TransactionResult> JoinLobbyTransaction(RoomInfo room)
    {
        try
        {
            _awaitingTxn = true;
            TxnPendingToast.PlayIn();

            Contract contract = ThirdwebManager.Instance.pkRaceContract;
            Debug.Log("Join Lobby: TRANSACTION INITIATED");

            TransactionResult txnResult = await contract.Write(
                    "joinRaceLobby",
                    room.Name
            );

            TxnPendingToast.PlayOut();

            _awaitingTxn = false;

            return txnResult;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            TxnPendingToast.PlayOut();
            _awaitingTxn = false;

            return null;
        }
    }

    private async Task<bool> ApproveKartToken(RoomInfo room)
    {
        _awaitingTxn = true;
        string connectedWalletAddress = await ThirdwebManager.Instance.SDK.wallet.GetAddress();
        string spenderAddress = ThirdwebManager.Instance.PK_RACE_CONTRACT_ADDRESS;
        try
        {
            Contract contract = ThirdwebManager.Instance.pkTokenContract;
            var fee = room.CustomProperties[C.EntryFee].ToString();
            BigInteger raceFee = ThirdwebManager.Instance.ConvertEtherToWei($"{fee}");

            Debug.Log($"Checking Allowance for owner {connectedWalletAddress} and spender {spenderAddress}");

            string allowance = await contract.Read<string>(
                    "allowance",
                   connectedWalletAddress,
                   spenderAddress
                );

            BigInteger bigIntAllowance = BigInteger.Parse(allowance);

            if (bigIntAllowance.CompareTo(raceFee) == -1)
            {
                Debug.Log($"Allowance is insufficient, calling increaseAllowance for Race Fee: {raceFee}");
                TxnPendingToast.PlayIn();

                TransactionResult txnResult = await contract.Write("increaseAllowance", spenderAddress, $"{raceFee}");

                _awaitingTxn = false;
                TxnPendingToast.PlayOut();

                return txnResult.isSuccessful();
            }
            else
            {
                return true;
            }

        }
        catch (Exception e)
        {
            TxnPendingToast.PlayOut();

            _awaitingTxn = false;
            Debug.LogError(e);
            return false;
        }
    }

    private async Task<TransactionResult> LeaveLobbyTransaction()
    {
        try
        {
            _awaitingTxn = true;

            Contract contract = ThirdwebManager.Instance.pkRaceContract;
            Debug.Log("Leave Lobby: TRANSACTION INITIATED");

            TransactionResult txnResult = await contract.Write(
                    "leaveRaceLobby",
                    PhotonNetwork.CurrentRoom.Name
            );

            _awaitingTxn = false;

            return txnResult;
        }
        catch (Exception e)
        {
            _awaitingTxn = false;

            Debug.LogError(e);
            return null;
        }

    }

    async void LeaveContractLobby()
    {
        TransactionResult leaveLobbyResult = await LeaveLobbyTransaction();

        if (leaveLobbyResult.isSuccessful())
        {
            Debug.Log("Left Race Lobby");
        }
        else
        {
            // show error message popup or something
            Debug.LogError(leaveLobbyResult);
        }
    }

    //Updating a player when changing any property.
    void TryUpdateOrCreatePlayerItem(Player targetPlayer)
    {
        PlayerItemInRoomUI playerItem = null;

        //Get or create PlayerItem.
        if (!Players.TryGetValue(targetPlayer, out playerItem))
        {
            playerItem = Instantiate(PlayerItemUIRef, PlayerItemUIRef.transform.parent);
            Players.Add(targetPlayer, playerItem);
        }

        var customProps = targetPlayer.CustomProperties;

        //SetActive(false) if player without any property.
        if (customProps == null ||
            !customProps.ContainsKey(C.CarName) ||
            !customProps.ContainsKey(C.IsReady)
            )
        {
            playerItem.SetActive(false);
            return;
        }

        var idReady = (bool)customProps[C.IsReady];

        System.Action kickAction = null;
        if (PhotonNetwork.IsMasterClient)
        {
            kickAction = () =>
            {
                PhotonNetwork.CloseConnection(targetPlayer);
            };
        }

        playerItem.SetActive(true);
        playerItem.UpdateProperties(targetPlayer, kickAction);

        if (targetPlayer.IsLocal)
        {
            if (_awaitingTxn || !idReady)
            {
                ReadyButton.Interactable(true);
            }
            else
            {
                ReadyButton.Interactable(false);
            }
            ReadyButton.UpdateUI();

        }

        //We inform all players about the start of the game.
        if (IsMaster && !WaitStartGame &&
            CurrentRoom.PlayerCount >= MinimumPlayersForStart &&
            CurrentRoom.Players.All
            (p =>
                p.Value.CustomProperties.ContainsKey(C.IsReady) &&
                (bool)p.Value.CustomProperties[C.IsReady]
            ))
        {
            PhotonNetwork.RaiseEvent(PE.StartGame, null, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            WaitStartGame = true;
        }
    }

    //Clear player items.
    public void RemoveAllPlayers()
    {
        foreach (var player in Players)
        {
            Destroy(player.Value.gameObject);
        }
        Players.Clear();
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.LogFormat("New master is player [{0}]", newMasterClient.NickName);
        // TrackSelector.Interactable(IsMaster);

        foreach (var player in CurrentRoom.Players)
        {
            TryUpdateOrCreatePlayerItem(player.Value);
        }
        UpdateCustomProperties();
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        TryUpdateOrCreatePlayerItem(newPlayer);
        UpdateCustomProperties();
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (Players.ContainsKey(otherPlayer))
        {
            Destroy(Players[otherPlayer].gameObject);
            Players.Remove(otherPlayer);
        }

        UpdateCustomProperties();
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        TryUpdateOrCreatePlayerItem(targetPlayer);
    }

    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(C.TrackName))
        {
            var trackName = (string)propertiesThatChanged[C.TrackName];

            int index = B.GameSettings.Tracks.FindIndex(t => t.name == trackName);

            // TrackSelector.SelectMode(index > -1 ? index : 0);
        }
        else if (propertiesThatChanged.ContainsKey(C.EntryFee))
        {
            var entryFee = (string)propertiesThatChanged[C.EntryFee];
            EntryFeeText.SetText($"{entryFee} $KART");
        }
    }

    void UpdateCustomProperties()
    {
        if (IsMaster)
        {
            var customProperties = new Hashtable();
            customProperties.Add(C.RoomCreator, PlayerProfile.NickName);
            CurrentRoom.SetCustomProperties(customProperties);
        }
    }

    //We get information about the start and start loading the level.
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == PE.StartGame)
        {
            WorldLoading.IsMultiplayer = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(C.IsReady, false, C.IsLoaded, false, C.TrackName, "");

            if (!CurrentRoom.CustomProperties.ContainsKey(C.TrackName))
            {
                return;
            }

            var trackName = (string)CurrentRoom.CustomProperties[C.TrackName];

            var track = B.GameSettings.Tracks.FirstOrDefault(t => t.name == trackName);
            WorldLoading.LoadingTrack = track;

            LoadingScreenUI.LoadScene(track.SceneName, track.RegimeSettings.RegimeSceneName);

            if (IsMaster)
            {
                CurrentRoom.IsOpen = false;
                CurrentRoom.IsVisible = false;
            }

        }
    }
}
