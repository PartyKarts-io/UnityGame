using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Bcpg;
using System.Net.NetworkInformation;
using UnityEngine;
using WalletConnectSharp.Unity;
using Thirdweb;
using TMPro;
using Michsky.UI.Reach;
using Photon.Pun;

public class MainMenuManager : MonoBehaviour
{
    ThirdwebSDK sdk;
    [SerializeField] private ButtonManager MultiplayerButton;
    [SerializeField] private GameObject MultiplayerTextReplacement;
    [SerializeField] private PanelManager MainPanelManager;

    // Start is called before the first frame update
    void Start()
    {
        MultiplayerButton.isInteractable = false;
        MultiplayerButton.UpdateUI();
        MultiplayerTextReplacement.SetActive(true);

        sdk = ThirdwebManager.Instance.SDK;
        ThirdwebManager.Instance.walletDisconnectedEvent.AddListener(OnWalletDisconnect);
        ThirdwebManager.Instance.walletConnectedEvent.AddListener(WalletConnected);
        ThirdwebManager.Instance.walletNetworkChangeEvent.AddListener(OnNetworkChange);
        ThirdwebManager.Instance.nftCountLoadedEvent.AddListener(NFTsLoaded);

        if (!Utils.IsWebGLBuild()) return;
    }

    private void ToggleAllButtons(bool shouldEnable)
    {
        if (!Utils.IsWebGLBuild()) return;

        string buttonText = "No NFTs Detected";
        if (ThirdwebManager.Instance.isLoadingNFTBalance)
        {
            buttonText = "Loading your NFTs...";
        } else if (ThirdwebManager.Instance.walletNFTs.Count > 0)
        {
            buttonText = "Lobbies";
        }

        MultiplayerButton.isInteractable = shouldEnable;

        MultiplayerTextReplacement.SetActive(shouldEnable == false);

        MultiplayerButton.buttonText = buttonText;

        MultiplayerButton.UpdateUI();
    }

    public void OnSinglePlayerSelected()
    {
        WorldLoading.IsMultiplayer = false;
    }

    public void OnMultiplayerSelected()
    {
        WorldLoading.IsMultiplayer = true;
    }


    public void OnWalletDisconnect(bool disconnected)
    {
        Debug.Log("Disabling Game");
        ToggleAllButtons(false);
    }

    public void OnNetworkChange(int chainId)
    {
        if (chainId != 137 || chainId != 80001)
            ToggleAllButtons(false);
    }

    public void NFTsLoaded(int count)
    {

        ToggleAllButtons(count > 0);
    }

    public async void WalletConnected(bool connected)
    {
        if (connected)
        {
            string address = await sdk.wallet.GetAddress();
            Debug.Log("Getting NFTs for: " + address);

            PlayerProfile.NickName = address;

            await ThirdwebManager.Instance.GetNFTsForPlayer();
        }
    }

}
