using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Bcpg;
using System.Net.NetworkInformation;
using UnityEngine;
using WalletConnectSharp.Unity;
using Thirdweb;
using TMPro;
using Michsky.UI.Reach;

public class MainMenuManager : MonoBehaviour
{
    ThirdwebSDK sdk;
    [SerializeField] private ButtonManager MultiplayerButton;

    // Start is called before the first frame update
    void Start()
    {
        sdk = ThirdwebManager.Instance.SDK;
        ThirdwebManager.Instance.walletDisconnectedEvent.AddListener(OnWalletDisconnect);
        ThirdwebManager.Instance.walletConnectedEvent.AddListener(WalletConnected);
        ThirdwebManager.Instance.walletNetworkChangeEvent.AddListener(OnNetworkChange);
        ThirdwebManager.Instance.nftsLoadedEvent.AddListener(NFTsLoaded);

        MultiplayerButton.Interactable(false);
        MultiplayerButton.buttonText = "Wallet Not Connected";
        MultiplayerButton.UpdateUI();
    }

    private void ToggleAllButtons(bool shouldEnable)
    {
        //uiButtons = GameObject.FindObjectsOfType<CustomButton>();

        string buttonText = "No NFTs Detected";
        if (ThirdwebManager.Instance.isLoadingNFTBalance)
        {
            buttonText = "Loading your NFTs...";
        } else if (ThirdwebManager.Instance.walletNFTs.Count > 0)
        {
            buttonText = "Race Online";
        }

        MultiplayerButton.isInteractable = shouldEnable;
        MultiplayerButton.buttonText = buttonText;
        MultiplayerButton.UpdateUI();
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

    public void NFTsLoaded(List<NFT> nfts)
    {

        ToggleAllButtons(nfts.Count > 0);
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
