using System;
using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Bcpg;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Thirdweb;
using TMPro;

/// <summary>
/// Windows controller to control the game windows.
/// </summary>
public class WindowsController : Singleton<WindowsController>
{

    [SerializeField] Window MainWindow;
    [SerializeField] GameObject MenuHolder;
    [SerializeField] GameObject BackButton;
    [SerializeField] Button StartMultiplayerButton;

    List<Window> WindowsHistory = new List<Window>();
    bool HasNewWindowInFrame;
    public Window CurrentWindow { get; private set; }
    public bool HasWindowsHistory { get { return WindowsHistory.Count > 0; } }
    CustomButton[] uiButtons;
    ThirdwebSDK sdk;

    protected override void AwakeSingleton()
    {
        GameOptions.CurrentQuality = 3; // sets game quality to High by default
        var windows = GameObject.FindObjectsOfType<Window>();
        foreach (var window in windows)
        {
            window.SetActive(false);
        }
    }

    private void Start()
    {
        if (MainWindow != null)
        {
            MainWindow.Open();
            CurrentWindow = MainWindow;
        }
        sdk = ThirdwebManager.Instance.SDK;
        ThirdwebManager.Instance.walletDisconnectedEvent.AddListener(OnWalletDisconnect);
        ThirdwebManager.Instance.walletConnectedEvent.AddListener(WalletConnected);
        ThirdwebManager.Instance.walletNetworkChangeEvent.AddListener(OnNetworkChange);
        ThirdwebManager.Instance.nftsLoadedEvent.AddListener(NFTsLoaded);
    }

    private void Update()
    {
        if (HasNewWindowInFrame)
        {
            HasNewWindowInFrame = false;
        }
        else if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button1)) && CurrentWindow != null && CurrentWindow != MainWindow)
        {
            OnBack();
        }

        BackButton.SetActive(HasWindowsHistory);
    }


    private void ToggleAllButtons(bool shouldEnable)
    {
        uiButtons = GameObject.FindObjectsOfType<CustomButton>();

        string buttonText = "Racing Requires a Kart!";
        if (!ThirdwebManager.Instance.isLoadingNFTBalance)
        {
            buttonText = "Loading your NFTs...";
        }

        StartMultiplayerButton.GetComponentInChildren<TMP_Text>().text = shouldEnable ? "Let's Party!" : buttonText;

        // Loop through the components and do something with them
        foreach (CustomButton button in uiButtons)
        {
            if (button.gameObject.name == "Button_Practice")
            {
                button.interactable = true;
            }
            else if (button.gameObject.name != "Buy a Kart")
            {
                button.interactable = !Utils.IsWebGLBuild() ? true : shouldEnable;
            }
            else
            {
                button.interactable = true;
            }
        }
    }

    public void NFTsLoaded(List<NFT> nfts)
    {
        if (nfts.Count > 0)
        {
            StartMultiplayerButton.GetComponentInChildren<TMP_Text>().text = "Lets Party!";
        }
        ToggleAllButtons(nfts.Count > 0);
    }

    public async void WalletConnected(bool connected)
    {
        if (connected)
        {
            string address = await sdk.wallet.GetAddress();
            Debug.Log("Getting NFTs for: " + address);

            PlayerProfile.NickName = address;

            StartMultiplayerButton.GetComponentInChildren<TMP_Text>().text = "Loading your NFTs...";

            await ThirdwebManager.Instance.GetNFTsForPlayer();
        }
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

    public void OpenWindow(Window window)
    {
        if (CurrentWindow == window)
            return;

        CloseCurrent();

        WindowsHistory.Add(window);
        CurrentWindow = window;
        CurrentWindow.Open();
        HasNewWindowInFrame = true;
    }

    public void OnBack(bool ignoreCustomBackAction = false)
    {
        if (!ignoreCustomBackAction && CurrentWindow != null && CurrentWindow.CustomBackAction != null)
        {
            CurrentWindow.CustomBackAction.SafeInvoke();
            return;
        }

        if (WindowsHistory.Count > 0)
        {
            WindowsHistory.RemoveAt(WindowsHistory.Count - 1);
        }

        CloseCurrent();

        if (WindowsHistory.Count > 0)
        {
            CurrentWindow = WindowsHistory[WindowsHistory.Count - 1];
        }
        else
        {
            CurrentWindow = MainWindow;
        }

        if (CurrentWindow != null)
        {
            CurrentWindow.Open();
        }
    }

    private void CloseCurrent()
    {
        if (CurrentWindow != null)
        {
            CurrentWindow.Close();
            CurrentWindow = null;
        }
    }
}
