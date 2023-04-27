using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Thirdweb;
using TMPro;

/// <summary>
/// Main menu window.
/// </summary>
public class MainMenuUI : WindowWithShowHideAnimators
{
    [SerializeField] Button StartMultiplayerButton;
    [SerializeField] Button BuyAKartButton;
    [SerializeField] Button StartGameButton;
    [SerializeField] Button SettingsButton;
    [SerializeField] Button ResultsButton;
    [SerializeField] Button QuitButton;
    [SerializeField] Window MultiplayerWindow;
    [SerializeField] Window SelectTrackWindow;
    [SerializeField] Window SettingsWindow;
    [SerializeField] Window ResultsWindow;
    protected override void Awake()
    {
        StartMultiplayerButton.onClick.AddListener(StartMultiplayer);
        BuyAKartButton.onClick.AddListener(BuyAKart);
        StartGameButton.onClick.AddListener(StartGame);
        SettingsButton.onClick.AddListener(Settings);
        ResultsButton.onClick.AddListener(Results);
        // QuitButton.onClick.AddListener(Quit);

        base.Awake();
    }

    private void Start()
    {
        StartMultiplayerButton.GetComponentInChildren<TMP_Text>().text = "Racing Requires a Kart!";

        if (Utils.IsWebGLBuild())
            DisableButtons();
    }

    private void StartGame()
    {
        WorldLoading.IsMultiplayer = false;
        WindowsController.Instance.OpenWindow(SelectTrackWindow);
    }

    private void StartMultiplayer()
    {
        WorldLoading.IsMultiplayer = true;
        WindowsController.Instance.OpenWindow(MultiplayerWindow);
    }


    public void DisableButtons()
    {
        StartMultiplayerButton.GetComponent<CustomButton>().interactable = false;
        SettingsButton.GetComponent<CustomButton>().interactable = false;
        ResultsButton.GetComponent<CustomButton>().interactable = false;
    }

    //public void EnableButtons()
    //{
    //    Debug.Log("Enabling Buttons");
    //    StartMultiplayerButton.GetComponentInChildren<TMP_Text>().text = "Lets Party!";
    //    StartMultiplayerButton.GetComponent<CustomButton>().interactable = true;
    //    StartGameButton.GetComponent<CustomButton>().interactable = true;
    //    SettingsButton.GetComponent<CustomButton>().interactable = true;
    //    ResultsButton.GetComponent<CustomButton>().interactable = true;
    //}


    private void BuyAKart()
    {
        string url = "https://opensea.io/collection/partykarts";
        Application.OpenURL(url);
    }

    private void Settings()
    {
        WindowsController.Instance.OpenWindow(SettingsWindow);
    }

    private void Results()
    {
        WindowsController.Instance.OpenWindow(ResultsWindow);
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }
}
