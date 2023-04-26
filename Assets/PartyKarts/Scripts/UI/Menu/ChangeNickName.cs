using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeNickName : Singleton<ChangeNickName> {

	[SerializeField] GameObject Holder;
	[SerializeField] Button ApplyButton;
	[SerializeField] Button CancelButton;
	[SerializeField] TMP_InputField InputField;

	protected override void AwakeSingleton ()
	{
		if (!PlayerPrefs.HasKey (C.NickName))
		{
			Show ();
		}
		else
		{
			Holder.SetActive (false);
		}
		ApplyButton.onClick.AddListener (OnApplyNickname);
		CancelButton.onClick.AddListener (OnCancel);
	}

	public void Show ()
	{
		InputField.text = PlayerProfile.NickName;
		Holder.SetActive (true);
	}

	void OnApplyNickname ()
	{
		PlayerProfile.NickName = InputField.text;
		Holder.SetActive (false);
	}

	void OnCancel ()
	{
		Holder.SetActive (false);
	}
}
