using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

/// <summary>
/// Base class game controller.
/// </summary>
public class GameController :MonoBehaviourPunCallbacks, IOnEventCallback
{
	[SerializeField] GameObject CountdownObject;
	[SerializeField] float CountdownTime = 3;
	[SerializeField] float DellayCountdownShowHide = 1;
	[SerializeField] List<Transform> CarPositions = new List<Transform>();
	[SerializeField] PositioningSystem m_PositioningSystem;

	[SerializeField] GameObject EndGameTimerHolder;
	[SerializeField, TextArea(1, 2)] string EndGameTextPrefix = "The first player finished.\nThe game will end in {0} seconds";

	[Space(10)]
	[SerializeField] GameBalance.RegimeSettings RegimeForDebug;

	public static GameController Instance;
	public static BaseRaceEntity RaceEntity;
	public static bool RaceIsStarted { get { return Instance.m_RaceIsStarted; } }
	public static CarController PlayerCar { get { return Instance.m_PlayerCar; } }
	public static List<CarController> AllCars { get { return Instance.m_AllCars; } }
	public static bool InGameScene { get { return Instance != null; } }
    public static bool InMainMenuScene { get { return SceneManager.GetActiveScene ().name == B.GameSettings.MainMenuSceneName; } }
	public static bool InPause { get { return Mathf.Approximately (Time.timeScale, 0); } }
	public static bool RaceIsEnded { get { return Instance.m_GameIsEnded; } }

	public PositioningSystem PositioningSystem { get { return m_PositioningSystem; } }

	bool m_GameIsEnded;
	bool m_RaceIsStarted;

	public Action RatingOfPlayersChanged;
	public Action OnEndGameAction;
	public Action OnStartRaceAction;

	public Action FixedUpdateAction;

	List<CarController> m_AllCars = new List<CarController>();
	CarController m_PlayerCar;

	void Awake ()
	{
		if (!WorldLoading.HasLoadingParams)
		{
			WorldLoading.RegimeForDebug = RegimeForDebug;
			LoadingScreenUI.LoadScene (RegimeForDebug.RegimeSceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
		}
		Instance = this;
		OnEndGameAction += () => m_GameIsEnded = true;

		StartCoroutine (StartRaceCoroutine ());

		//Find all cars in current game.
		foreach (var car in GameObject.FindObjectsOfType<CarController> ())
		{
			if (car.GetComponent<UserControl> () != null)
			{
				if (m_PlayerCar != null)
				{
					Debug.LogErrorFormat ("CarControllers with UserControl script count > 1");
				}
				else
				{
					m_PlayerCar = car;
				}
			}
			m_AllCars.Add (car);
		}

		CarPositions.ForEach (p => p.SetActive (false));

		//Destroy All AudioListeners
		foreach (var car in m_AllCars)
		{
			var audioListener = car.GetComponent<AudioListener> ();
			if (audioListener != null)
			{
				Destroy (audioListener);
			}
		}

		//Multiplayer awake
		if (WorldLoading.IsMultiplayer)
		{
			m_AllCars.ForEach (c => Destroy(c.gameObject));
			m_AllCars.Clear ();

			InitRaceEntity ();

			StartCoroutine (InstantiateMultiplayerCar ());
			return;
		}

		//For debug load scene
		if (!WorldLoading.HasLoadingParams)
		{
			if (AllCars.All (c => c.GetComponent<UserControl> () == null))
			{
				var car = AllCars.First ();
				var userControl = car.gameObject.AddComponent<UserControl> ();
				m_PlayerCar = car;

				if (car.GetComponent<DriftAIControl> () != null)
				{
					userControl.enabled = false;
				}
			}

			if (m_PlayerCar != null)
			{
                m_PlayerCar.gameObject.AddComponent<AudioListener> ();
			}
			else
			{
				Debug.LogErrorFormat ("[Debug Scene] PlayerCar not found ");
			}

            AllCars.ForEach (c => SetDriftConfigForCar (c));

			InitRaceEntity ();
			return;
		}

		//Destroy all cars in scene if load from world loading.
		m_AllCars.ForEach (c => Destroy (c.gameObject));
		m_AllCars.Clear ();

		//Initialize player car
		m_PlayerCar = GameObject.Instantiate (WorldLoading.PlayerCar.CarPrefab);

        if (m_PlayerCar.GetComponent<UserControl> () == null)
		{
			m_PlayerCar.gameObject.AddComponent<UserControl> ();
		}

		m_PlayerCar.SetColor (WorldLoading.SelectedColor);
		m_AllCars.Add (m_PlayerCar);

		//Initialize AI cars
		for (int i = 0; i < WorldLoading.AIsCount; i++)
		{
			var carPreset = WorldLoading.AvailableCars.RandomChoice();
			var car = GameObject.Instantiate (carPreset.CarPrefab);

            car.gameObject.AddComponent<DriftAIControl> ();

			var userControl = car.GetComponent<UserControl> ();
			if (userControl != null)
			{
				Destroy (userControl);
			}

			car.SetColor (carPreset.GetRandomColor ());

			m_AllCars.Add (car);
		}

		//Set random start positions.
		for (int i = 0; i < m_AllCars.Count; i++)
		{
			int j = UnityEngine.Random.Range (0, m_AllCars.Count);
			var temp = m_AllCars[i];
			m_AllCars[i] = m_AllCars[j];
			m_AllCars[j] = temp;
		}

		m_PlayerCar.gameObject.AddComponent<AudioListener> ();

		if (m_AllCars.Count > CarPositions.Count)
		{
			Debug.LogErrorFormat ("CarPositions less loaded cars count: CarPositions: {0}, Loaded cars: {1}", CarPositions.Count, m_AllCars.Count);
			return;
		}

		for (int i = 0; i < m_AllCars.Count; i++)
		{
			m_AllCars[i].transform.position = CarPositions[i].position;
			m_AllCars[i].transform.rotation = CarPositions[i].rotation;
            SetDriftConfigForCar (m_AllCars[i]);
        }

        InitRaceEntity ();
	}

    void SetDriftConfigForCar (CarController car)
    {
        car.SetCarDriftConfig (WorldLoading.RegimeSettings.CarDriftConfig);
        car.GetFrontLeftWheel.UpdateFrictionConfig (WorldLoading.RegimeSettings.FrontWheelsConfig);
        car.GetFrontRightWheel.UpdateFrictionConfig (WorldLoading.RegimeSettings.FrontWheelsConfig);
        car.GetRearLeftWheel.UpdateFrictionConfig (WorldLoading.RegimeSettings.RearWheelsConfig);
        car.GetRearRightWheel.UpdateFrictionConfig (WorldLoading.RegimeSettings.RearWheelsConfig);
    }

	void InitRaceEntity ()
	{
		if (WorldLoading.RegimeSettings is GameBalance.DriftRegimeSettings)
		{
			RaceEntity = new DriftRaceEntity (this);
		}
		else
		{
			RaceEntity = new RaceEntity (this);
		}
	}

	/// <summary>
	/// Delay start the race and the inclusion of the countdown.
	/// </summary>
	IEnumerator StartRaceCoroutine ()
	{

		if (WorldLoading.IsMultiplayer)
		{
			PhotonNetwork.LocalPlayer.SetCustomProperties (C.IsLoaded, true, C.IsReady, false);
		}

		while (!LoadingScreenUI.IsLoaded)
		{
			yield return null;
		}

		var countDownObject = Instantiate(CountdownObject);
		countDownObject.SetActive (true);

		yield return new WaitForSeconds (CountdownTime);

		OnStartRaceAction.SafeInvoke ();
		m_RaceIsStarted = true;

		yield return new WaitForSeconds (DellayCountdownShowHide);

		CountdownObject.SetActive (false);
	}

	#region Mulriplayer

	Coroutine FinishTimerCoroutine;
	List<MultiplayerCarController> MultiplayerCars = new List<MultiplayerCarController>();

	//Instantiate multiplayer car.
	IEnumerator InstantiateMultiplayerCar ()
	{
		//Wait other players
		while (!LoadingScreenUI.IsLoaded)
		{
			yield return null;
		}

		//Find start pos.
		Vector3 pos = CarPositions[0].position;
		Quaternion rot = CarPositions[0].rotation;
		int index = 0;

		var orderedPlayers = PhotonNetwork.CurrentRoom.Players.OrderBy(p => p.Key);
		foreach (var player in orderedPlayers)
		{
			if (player.Value == PhotonNetwork.LocalPlayer)
			{
				pos = CarPositions[index].position;
				rot = CarPositions[index].rotation;
				break;
			}
			index++;
		}

		PhotonNetwork.Instantiate (WorldLoading.PlayerCar.CarPrefab.name, pos, rot);
	}

	/// <summary>
	/// To add a multiplayer car to the game controller.
	/// </summary>
	public virtual void AddMultiplayerCar (MultiplayerCarController multiplayerController)
	{
        SetDriftConfigForCar (multiplayerController.Car);
        AllCars.Add (multiplayerController.Car);
		MultiplayerCars.Add (multiplayerController);
		if (multiplayerController.IsMine)
		{
			m_PlayerCar = multiplayerController.Car;
		}
		RaceEntity.AddMultiplayerCar (multiplayerController);
	}

	/// <summary>
	/// Send a message about the finish of the race to other players.
	/// </summary>
	public static void SendFinishEvent ()
	{
		PhotonNetwork.RaiseEvent (PE.FinishRace, null, new RaiseEventOptions () { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
	}

	/// <summary>
	/// Receive a race finish message from other players.
	/// </summary>
	public void OnEvent (EventData photonEvent)
	{
		if (photonEvent.Code == PE.FinishRace)
		{
			if (FinishTimerCoroutine == null)
			{
				FinishTimerCoroutine = StartCoroutine (StartFinishRaceTimer ());
			}
			var players = PhotonNetwork.CurrentRoom.Players;
			var finishedCar = MultiplayerCars.FirstOrDefault (c => players.ContainsKey(photonEvent.Sender) && c.PhotonView.Owner == players[photonEvent.Sender]);
			if (finishedCar != null)
			{
				finishedCar.Car.PositioningCar.ForceFinish ();
			}
		}
	}

	IEnumerator StartFinishRaceTimer ()
	{
		var timer = B.MultiplayerSettings.SecondsToEndGame;
		var endGameTimerHolder = Instantiate(EndGameTimerHolder);
		endGameTimerHolder.SetActive (true);
		var endGameTimerText = endGameTimerHolder.GetComponentInChildren<TextMeshProUGUI>();
		while (timer >= 0 && AllCars.Any (c => c != null && !c.PositioningCar.IsFinished))
		{
			endGameTimerText.text = string.Format (EndGameTextPrefix, timer);
			timer--;
			yield return new WaitForSeconds (1);
		}

		if (!PlayerCar.PositioningCar.IsFinished)
		{
			PlayerCar.PositioningCar.OnFinishRaceAction.SafeInvoke ();
			SendFinishEvent ();
		}

		while (AllCars.Any (c => c != null && !c.PositioningCar.IsFinished))
		{
			yield return null;
		}

		endGameTimerHolder.SetActive (false);
		OnEndGameAction.SafeInvoke ();
	}

	public static void LeaveRoom ()
	{
		if (WorldLoading.IsMultiplayer)
		{
			PhotonNetwork.LeaveRoom ();
		}
	}

	public override void OnDisconnected (DisconnectCause cause)
	{
		System.Action onCloseAction = () =>
		{
			LoadingScreenUI.LoadScene (B.GameSettings.MainMenuSceneName);
		};
		B.MultiplayerSettings.ShowDisconnectCause (cause);
	}

	#endregion //Mulriplayer

	void Update () { }
	void FixedUpdate () 
	{
		FixedUpdateAction.SafeInvoke ();
	}

}
