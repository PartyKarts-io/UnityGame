using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using GameBalance;

/// <summary>
/// Select track menu. TODO Add Lock/Unlock tracks logic.
/// </summary>
public class SelectTrackUI :WindowWithShopLogic
{
	[SerializeField] Button NextTrackButton;
	[SerializeField] Button PrevTrackButton;
	[SerializeField] LineRenderer TrackPath;

	[Header("Path settings")]
	[SerializeField] float MaxRadius = 1.5f;
	[SerializeField] Window SelectCarkWindow;
	[SerializeField] float PathStep = 10;

	[Header("Track info")]
	[SerializeField] Image TrackIcon;
	[SerializeField] TextMeshProUGUI TrackNameText;
	[SerializeField] TextMeshProUGUI LapCountText;
	[SerializeField] TextMeshProUGUI RegimeText;
	[SerializeField] Image RegimeImage;

	[Header("Animation")]
	[SerializeField] Vector2 LeftPoint = new Vector2(- 3840, 0);
	[SerializeField] Vector2 RightPoint = new Vector2(3840, 0);
	[SerializeField] float SpeedAnimation = 960;

	bool IsMultiplayer;
	TrackPreset CurrentTrackPreset;
	int CurrentTrackIndex = 0;
	bool SubmitIsPressed = true;
	bool HorizontalIsPressed = true;

	const string LapCaption = "Lap";
	const string LapsCaption = "Laps";

	public Action<TrackPreset> OnSelectTrackAction { get; set; }

	List<TrackPreset> Tracks { get { return WorldLoading.IsMultiplayer ? B.MultiplayerSettings.AvailableTracksForMultiplayer : B.GameSettings.Tracks; } }

	// Use this for initialization
	void Start () {
		NextTrackButton.onClick.AddListener (NextTrack);
		PrevTrackButton.onClick.AddListener (PrevTrack);
	}

	void OnEnable ()
	{
		if (IsMultiplayer != WorldLoading.IsMultiplayer) {
			IsMultiplayer = WorldLoading.IsMultiplayer;
			CurrentTrackIndex = 0;
		}
		SelectTrack (Tracks[CurrentTrackIndex]);
	}

	public override void Open ()
	{
		SubmitIsPressed = true;
		OnSelectTrackAction = null;
		base.Open ();
	}

	void Update ()
	{
		if (WindowsController.Instance.CurrentWindow != this)
		{
			return;
		}

		float horizontal = Input.GetAxis("Horizontal");

		if (!Mathf.Approximately (horizontal, 0))
		{
			if (!HorizontalIsPressed)
			{
				if (horizontal > 0)
				{
					NextTrack ();
				}
				else
				{
					PrevTrack ();
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

	protected override void OnSelect ()
	{
		if (OnSelectTrackAction != null)
		{
			OnSelectTrackAction.SafeInvoke (CurrentTrackPreset);
		}
		else
		{
			WorldLoading.LoadingTrack = CurrentTrackPreset;
			WindowsController.Instance.OpenWindow (SelectCarkWindow);
		}
	}

	void NextTrack ()
	{
		CurrentTrackIndex = MathExtentions.LoopClamp (CurrentTrackIndex + 1, 0, B.GameSettings.Tracks.Count);
		SelectTrack (Tracks[CurrentTrackIndex]);
	}

	void PrevTrack ()
	{
		CurrentTrackIndex = MathExtentions.LoopClamp (CurrentTrackIndex - 1, 0, B.GameSettings.Tracks.Count);
		SelectTrack (Tracks[CurrentTrackIndex]);
	}

	void SelectTrack (GameBalance.TrackPreset track)
	{
		CurrentTrackPreset = track;
		RefreshButtonState (CurrentTrackPreset);

		TrackIcon.sprite = track.TrackIcon;
		TrackNameText.text = track.TrackName;
		LapCountText.text = string.Format("{0} {1}",track.LapsCount.ToString (), (track.LapsCount == 1? LapCaption: LapsCaption));
		RegimeText.text = track.RegimeSettings.RegimeCaption;
		RegimeImage.sprite = track.RegimeSettings.RegimeImage;

		var path = track.GameController.PositioningSystem.GetPathForVisual;
		path.Awake ();
		TrackPath.positionCount = (int)(path.Length / PathStep);

		float distance = 0;
		var allPosints = new List<Vector3> ();

		for (int i = 0; i < TrackPath.positionCount && distance < path.Length; i++)
		{
			var pathPoint = path.GetRoutePoint (distance);
			distance += PathStep;
			var position = new Vector3 (pathPoint.position.x, pathPoint.position.z, pathPoint.position.y);
			allPosints.Add (position);
		}

		float maxX = allPosints.Max (p => p.x.Abs ());
		float maxY = allPosints.Max (p => p.y.Abs ());

		float scale = Mathf.Max (maxX, maxY);
		scale = MaxRadius / scale;

		float minX = allPosints.Min (p => p.x);
		float minY = allPosints.Min (p => p.y);

		Vector3 offset = new Vector3 (minX, minY, 0);

		for (int i = 0; i < TrackPath.positionCount; i++)
		{
			TrackPath.SetPosition (i, (allPosints[i] - offset) * scale);
		}
	}
}
