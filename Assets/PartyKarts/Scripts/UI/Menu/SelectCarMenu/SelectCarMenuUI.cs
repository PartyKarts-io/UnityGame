using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameBalance;
using TMPro;

/// <summary>
/// Select car menu.
/// </summary>
public class SelectCarMenuUI :WindowWithShopLogic
{
	[SerializeField] Button SetNextCarButton;
	[SerializeField] Button SetPrevCarButton;
	[SerializeField] TextMeshProUGUI CarCaptionText;
	[SerializeField] Transform CarPosition;
	[SerializeField] CarParamsUI CarParammsPanel;
	[SerializeField] CarSetColorUI CarSetColorPanel;
	[SerializeField] Button SelectParamsPanelButton;
	[SerializeField] Button SelectColorPanelButton;

	[SerializeField] Transform SelectedButtonBackground;
	[SerializeField] RectTransform ShownPanelPos;
	[SerializeField] RectTransform HiddenPanelPos;
	[SerializeField] float MovePanelsSpeed;

	bool IsMultiplayer;
	int CurrentCarIndex;
	CarPreset SelectedCar;
	GameObject CarInScene;
	Coroutine MovePanelsCoroutine;
	bool SubmitIsPressed = true;
	bool HorizontalIsPressed = true;

	List<CarPreset> Cars { get { return WorldLoading.IsMultiplayer? B.MultiplayerSettings.AvailableCarsForMultiplayer: WorldLoading.AvailableCars; } }
	public Action<CarPreset> OnSelectCarAction { get; set; }

	protected override void Awake ()
	{

		SetNextCarButton.onClick.AddListener (NextCar);
		SetPrevCarButton.onClick.AddListener (PrevCar);

		SelectParamsPanelButton.onClick.AddListener (OnSelectParamsPanel);
		SelectColorPanelButton.onClick.AddListener (OnSelectColorPanel);
	}

	private void OnEnable()
	{
		if (IsMultiplayer != WorldLoading.IsMultiplayer)
		{
			IsMultiplayer = WorldLoading.IsMultiplayer;
			CurrentCarIndex = 0;
		}
		SelectCar (Cars[CurrentCarIndex]);
	}

	private IEnumerator Start ()
	{
		yield return null;
		OnSelectParamsPanel ();
	}

	void Update ()
	{
		if (WindowsController.Instance.CurrentWindow != this)
		{
			return;
		}

		float horizontal = Input.GetAxis("Horizontal");

		if (!Mathf.Approximately(horizontal, 0))
		{
			if (!HorizontalIsPressed)
			{
				if (horizontal > 0)
				{
					NextCar ();
				}
				else
				{
					PrevCar ();
				}
			}
			HorizontalIsPressed = true;
		}
		else
		{
			HorizontalIsPressed = false;
		}

		if (!Mathf.Approximately (Input.GetAxis ("Submit"), 0))
		{
			if (!SubmitIsPressed && SelectButton.interactable)
			{
				SelectButton.onClick.Invoke ();
			}
			SubmitIsPressed = true;
		}
		else
		{
			SubmitIsPressed = false;
		}
	}

	void SelectCar (CarPreset newCar)
	{
		if (CarInScene)
		{
			Destroy (CarInScene.gameObject);
		}

		CarCaptionText.text = newCar.CarCaption;
		SelectedCar = newCar;
		RefreshButtonState (newCar);

		var prefabRef = newCar.CarPrefabForSelectMenu;

		if (prefabRef == null)
		{
			prefabRef = newCar.CarPrefab.gameObject;
		}

		CarInScene = GameObject.Instantiate (prefabRef);
		CarInScene.transform.position = CarPosition.position;
		CarInScene.transform.rotation = CarPosition.rotation;

		CarParammsPanel.SelectCar (newCar);
		CarSetColorPanel.SelectCar (newCar, CarInScene.GetComponent<ISetColor> ());
	}

	protected override void OnSelect ()
	{
		WorldLoading.PlayerCar = SelectedCar;

		if (OnSelectCarAction != null)
		{
			OnSelectCarAction.SafeInvoke (SelectedCar);
		}
		else
		{
			WorldLoading.IsMultiplayer = false;
			LoadingScreenUI.LoadScene (WorldLoading.LoadingTrack.SceneName, WorldLoading.RegimeSettings.RegimeSceneName);
		}
	}

	void NextCar ()
	{
		CurrentCarIndex = MathExtentions.LoopClamp (CurrentCarIndex + 1, 0, (Cars.Count));
		SelectCar (Cars[CurrentCarIndex]);
	}

	void PrevCar ()
	{
		CurrentCarIndex = MathExtentions.LoopClamp (CurrentCarIndex - 1, 0, (Cars.Count));
		SelectCar (Cars[CurrentCarIndex]);
	}

	public override void Open ()
	{
		SubmitIsPressed = true;
		OnSelectCarAction = null;
		CameraInMainMenu.Instance.SetCarSelectMenu (true);
		base.Open ();
	}

	public override void Close ()
	{
		CameraInMainMenu.Instance.SetCarSelectMenu (false);
		base.Close ();
	}

	public void OnSelectParamsPanel ()
	{
		SelectParamsPanelButton.interactable = false;
		SelectColorPanelButton.interactable = true;

		if (MovePanelsCoroutine != null)
		{
			StopCoroutine (MovePanelsCoroutine);
			MovePanelsCoroutine = null;
		}

		MovePanelsCoroutine = StartCoroutine (DoShowPanel (CarParammsPanel.transform, CarSetColorPanel.transform, SelectParamsPanelButton.transform.localPosition));
	}

	public void OnSelectColorPanel ()
	{
		SelectParamsPanelButton.interactable = true;
		SelectColorPanelButton.interactable = false;

		if (MovePanelsCoroutine != null)
		{
			StopCoroutine (MovePanelsCoroutine);
			MovePanelsCoroutine = null;
		}

		MovePanelsCoroutine = StartCoroutine (DoShowPanel (CarSetColorPanel.transform, CarParammsPanel.transform, SelectColorPanelButton.transform.localPosition));
	}

	IEnumerator DoShowPanel (Transform showPanel, Transform hidePanel, Vector3 newSelectedBackgroundPos)
	{
		while (!Mathf.Approximately (showPanel.position.y, ShownPanelPos.position.y) ||
				!Mathf.Approximately (hidePanel.position.y, HiddenPanelPos.position.y) ||
				!Mathf.Approximately (SelectedButtonBackground.transform.position.x, newSelectedBackgroundPos.x)
		)
		{
			yield return null;
			showPanel.position = Vector3.Lerp (showPanel.position, ShownPanelPos.position, Time.deltaTime * MovePanelsSpeed);
			hidePanel.position = Vector3.Lerp (hidePanel.position, HiddenPanelPos.position, Time.deltaTime * MovePanelsSpeed);
			SelectedButtonBackground.localPosition = Vector3.Lerp (SelectedButtonBackground.localPosition, newSelectedBackgroundPos, Time.deltaTime * MovePanelsSpeed);
		}
	}

	private void OnDisable ()
	{
		StopAllCoroutines ();
	}
}
