using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Main menu in game.
/// </summary>
public class InGameMenu :MonoBehaviour
{

	[SerializeField] Transform BlockerImage;                //For block all buttons in game UI.
	[SerializeField] Button SetNextCameraButton;            //Set next camera button available from the game UI.
	[SerializeField] Button RestartCarButton;               //Reset car button available from the game UI.
	[SerializeField] Button PauseButton;					//Pause button available from the game UI.
	[SerializeField] Button ContinueButton;                 //Main button for countinue game after close winodw.
	[SerializeField] Button RestartButton;                  //Reset current scene button.
	[SerializeField] Button SettingsButton;                 //Open settings button.
	[SerializeField] Button ExitButton;                     //Exit to main menu button.

	[SerializeField] Window InGameMainMenu;                 //Link to main menu Window in game scene.
	[SerializeField] Window InGameSettings;                 //Link to settings Window in game scene.

	WindowsController Windows { get { return WindowsController.Instance; } }

	private void Start ()
	{
		BlockerImage.SetActive (false);
		SetNextCameraButton.onClick.AddListener (() => CameraController.Instance.SetNextCamera ());
		RestartCarButton.onClick.AddListener (() => GameController.PlayerCar.ResetPosition ());
		PauseButton.onClick.AddListener (Show);
		ContinueButton.onClick.AddListener (() => Windows.OnBack());
		SettingsButton.onClick.AddListener (Settings);
		ExitButton.onClick.AddListener (Exit);
		InGameMainMenu.OnDisableAction += OnDisableMainMenu;

		if (WorldLoading.IsMultiplayer)
		{
			RestartButton.interactable = false;
		}
		else
		{
			RestartButton.onClick.AddListener (RestartScene);
		}
	}

	private void Update ()
	{
		if (Windows.CurrentWindow == null && !InGameMainMenu.gameObject.activeInHierarchy && !GameController.RaceIsEnded && 
			(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown (KeyCode.Joystick1Button7)))
		{
			Show ();
			ContinueButton.Select ();
		}
	}

	private void OnDestroy ()
	{
		StopAllCoroutines ();
		InGameMainMenu.OnDisableAction -= OnDisableMainMenu;
	}

	void SetTimeScale (float scale)
	{
		if (!WorldLoading.IsMultiplayer)
		{
			Time.timeScale = scale;
			GameOptions.UpdateAudioMixer ();
		}
	}

	void Show ()
	{
		BlockerImage.SetActive (true);
		Windows.OpenWindow (InGameMainMenu);
		SetTimeScale (0);
	}

	void OnDisableMainMenu ()
	{
		if (Windows.HasWindowsHistory)
			return;

		BlockerImage.SetActive (false);
		SetTimeScale (1);
	}


	void RestartScene ()
	{
		SetTimeScale (1);
		LoadingScreenUI.ReloadCurrentScene();
	}

	void Settings ()
	{
		Windows.OpenWindow (InGameSettings);
	}

	void Exit ()
	{
		SetTimeScale (1);
		GameController.LeaveRoom();
		LoadingScreenUI.LoadScene (B.GameSettings.MainMenuSceneName);
	}
}
