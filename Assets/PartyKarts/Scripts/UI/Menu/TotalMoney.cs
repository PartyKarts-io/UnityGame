using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TotalMoney : MonoBehaviour {

	[SerializeField] TextMeshProUGUI TotalMoneyText;

	void Awake ()
	{
		PlayerProfile.OnMoneyChanged += SetMoney;
		SetMoney (PlayerProfile.Money);
	}

	void Update ()
	{
		TotalMoneyText.SetActive(WindowsController.Instance.HasWindowsHistory);
	}

	void OnDestroy ()
	{
		PlayerProfile.OnMoneyChanged -= SetMoney;
	}

	void SetMoney (int money)
	{
		TotalMoneyText.text = string.Format("${0}", money);
	}
}
