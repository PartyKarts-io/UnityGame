using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

/// <summary>
/// Loading screen. Loaded with any loading scene.
/// </summary>
public class LoadingScreenUI :MonoBehaviour
{

	[SerializeField] TextMeshProUGUI LoadedProcentText;
	[SerializeField] Image BackGroundInage;
	[SerializeField] float HideObjectsTime = 0.5f;

	IEnumerator LoadSceneCoroutine (AsyncOperation asyncOperation, float startProcent = 0, float procentMultiplier = 1f, bool needDestroyObject = true, System.Action onCompleteAction = null)
	{
		while (!asyncOperation.isDone)
		{
			var procent = startProcent * 100 + Mathf.RoundToInt ((asyncOperation.progress) * 100 * procentMultiplier);
			LoadedProcentText.text = string.Format ("Loading: {0}%", procent);
			if (asyncOperation.progress >= 0.9f)
			{
				asyncOperation.allowSceneActivation = true;
			}
			yield return null;
		}

		if (needDestroyObject)
		{
			//Wait other players
			if (WorldLoading.IsMultiplayer && SceneManager.sceneCount > 1)
			{
				LoadedProcentText.text = "Wait other players...";
				while (PhotonNetwork.CurrentRoom.Players.Any (p => !p.Value.CustomProperties.ContainsKey(C.IsLoaded) || !(bool)p.Value.CustomProperties[C.IsLoaded]))
				{
					yield return null;
				}
				yield return new WaitForSeconds (B.MultiplayerSettings.WaitOtherPlayersTime);
			}

			IsLoaded = true;

			//Hide loading screen animation
			float timer = HideObjectsTime;
			while (timer > 0)
			{
				float newAlpha = timer / HideObjectsTime;
				BackGroundInage.SetAlpha (newAlpha);
				LoadedProcentText.SetAlpha (newAlpha);
				timer -= Time.deltaTime;
				yield return null;
			}

			Destroy (gameObject);
			Instance = null;
		}

		onCompleteAction.SafeInvoke ();
	}

	#region Static region

	public static bool IsLoaded { get; private set; }

	static LoadingScreenUI Instance;

	static string CurrentLevelName;
	static string CurrentRegimeSceneName;

	public static void LoadScene (string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
	{
		if (Instance != null)
		{
			Debug.LogError ("Loading in process!");
			return;
		}

		IsLoaded = false;
		
		Instance = GameObject.Instantiate (B.ResourcesSettings.LoadingScreenUI);
		DontDestroyOnLoad (Instance);

		var asyncOperation = SceneManager.LoadSceneAsync (sceneName, mode);
		asyncOperation.allowSceneActivation = false;
		Instance.StartCoroutine (Instance.LoadSceneCoroutine (asyncOperation));
	}

	public static void LoadScene (string sceneName, string regimeSceneName)
	{
		if (Instance != null)
		{
			Debug.LogError ("Loading in process!");
			return;
		}

		IsLoaded = false;
		
		Instance = GameObject.Instantiate (B.ResourcesSettings.LoadingScreenUI);
		DontDestroyOnLoad (Instance);

		CurrentLevelName = sceneName;
		CurrentRegimeSceneName = regimeSceneName;

		var asyncOperation = SceneManager.LoadSceneAsync (sceneName, LoadSceneMode.Single);
		asyncOperation.allowSceneActivation = false;

		System.Action completeAction = () =>
		{
			asyncOperation = SceneManager.LoadSceneAsync (regimeSceneName, LoadSceneMode.Additive);
			asyncOperation.allowSceneActivation = false;

			Instance.StartCoroutine (Instance.LoadSceneCoroutine (asyncOperation, startProcent: 0.7f, procentMultiplier: 0.3f, needDestroyObject: true));
		};

		Instance.StartCoroutine (Instance.LoadSceneCoroutine (asyncOperation, startProcent: 0, procentMultiplier: 0.7f, needDestroyObject: false, onCompleteAction: completeAction));
	}

	public static void ReloadCurrentScene ()
	{
		LoadScene (CurrentLevelName, CurrentRegimeSceneName);
	}

	#endregion //Static region
}
