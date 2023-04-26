using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageBox : MonoBehaviour {

	[SerializeField] TextMeshProUGUI MessageText;
	[SerializeField] Button ApplyButton;
	[SerializeField] Button CancelButton;
	[SerializeField] TextMeshProUGUI ApplyButtonText;
	[SerializeField] TextMeshProUGUI CancelButtonText;

	Action ApplyAction;
	Action CancelAction;

	public static bool HasActiveMessageBox { get { return ActiveMessageBox != null; } }
	static MessageBox ActiveMessageBox;

	void Awake ()
	{
		if (!Application.isPlaying)
		{
			DestroyImmediate (gameObject);
		}

		ApplyButton.onClick.AddListener (OnApply);
		CancelButton.onClick.AddListener (OnCancel);
	}

	public static void Show (string message)
	{
		ActiveMessageBox = Instantiate (B.ResourcesSettings.MessageBox);
		ActiveMessageBox.Init (message);
	}

	public static void Show (string message, Action applyAction, Action cancelAction, string applyButtonText = "Apply", string cancelButtonText = "OK")
	{
		ActiveMessageBox = Instantiate (B.ResourcesSettings.MessageBox);
		ActiveMessageBox.Init (message, applyAction, cancelAction, applyButtonText, cancelButtonText);
	}

	void Init (string message, Action applyAction = null,  Action cancelAction = null, string applyButtonText = "Apply", string cancelButtonText = "OK")
	{
		MessageText.text = message;
		ApplyAction = applyAction;
		CancelAction = cancelAction;

		ApplyButtonText.text = applyButtonText;
		CancelButtonText.text = cancelButtonText;

		ApplyButton.SetActive (applyAction != null);

		CancelButton.Select ();
	}

	public void OnApply ()
	{
		ApplyAction.SafeInvoke ();
		Destroy (gameObject);
	}

	public void OnCancel ()
	{
		CancelAction.SafeInvoke ();
		Destroy (gameObject);
	}
}
