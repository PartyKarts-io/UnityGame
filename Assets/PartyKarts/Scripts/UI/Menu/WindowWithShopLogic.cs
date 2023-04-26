using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// Logic for unlocking content in the main menu.
/// To unlock tracks and cars.
/// </summary>

public class WindowWithShopLogic :WindowWithShowHideAnimators
{
	[Header("Shop logic")]
	[SerializeField] GameObject LockedHolder;

	[SerializeField] GameObject CompleteTrackHolder;
	[SerializeField] TextMeshProUGUI CompleteTrackText;

	[SerializeField] ButtonState UnlockedState;
	[SerializeField] ButtonState MoneyLockState;
	[SerializeField] ButtonState TrackLockState;

	[SerializeField] protected Button SelectButton;
	[SerializeField] TextMeshProUGUI ButtonText;

	protected virtual void OnSelect () { }

	protected void RefreshButtonState (LockedContent lockContent)
	{
		CompleteTrackHolder.SetActive (false);

		if (lockContent.IsUnlocked)
		{
			LockedHolder.SetActive (false);
			SetButtonState (UnlockedState, OnSelect);
			SelectButton.interactable = true;
			return;
		}

		LockedHolder.SetActive (true);

		if (lockContent.GetUnlockType == LockedContent.UnlockType.UnlockByMoney)
		{
			SelectButton.interactable = lockContent.CanUnlock;
			UnityAction clickAction = () =>
			{
				if (lockContent.TryUnlock ())
				{
					RefreshButtonState (lockContent);
				}
			};
			SetButtonState (MoneyLockState, clickAction);
			return;
		}

		CompleteTrackHolder.SetActive (true);
		CompleteTrackText.text = "You don't own this NFT";
		SetButtonState (TrackLockState);
		SelectButton.interactable = false;
	}

	void SetButtonState (ButtonState state, UnityAction onClickAction = null)
	{
		SelectButton.onClick.RemoveAllListeners ();
		if (onClickAction != null)
		{
			SelectButton.onClick.AddListener (onClickAction);
		}

		SelectButton.colors = state.ColorBlock;
		ButtonText.text = state.ButtonStr;
	}
}
