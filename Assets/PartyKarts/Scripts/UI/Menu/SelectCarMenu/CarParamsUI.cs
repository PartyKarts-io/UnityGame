using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameBalance;

public class CarParamsUI :MonoBehaviour
{

	[SerializeField] Slider PowerSlider;
	[SerializeField] Slider ControlSlider;
	[SerializeField] Slider MassSlider;
	[SerializeField] float ChangeParamsSpeed;
	[SerializeField] TextMeshProUGUI DescriptionText;

	float MaxPower;
	float MaxControl;
	float MaxMass;
	Coroutine SelectCarCoroutine;
	List<CarPreset> CanSelectCars { get { return WorldLoading.AvailableCars; } }

	void UpdateMaxValues ()
	{
		MaxPower = CanSelectCars.Max (c => c.GetPower);
		MaxControl = CanSelectCars.Max (c => c.GetControl);
		MaxMass = CanSelectCars.Max (c => c.GetMass);
	}

	public void SelectCar (CarPreset newCar)
	{

		if (Mathf.Approximately (MaxPower, 0))
		{
			UpdateMaxValues ();
		}

		DescriptionText.text = newCar.Description;

		if (SelectCarCoroutine != null)
		{
			StopCoroutine (SelectCarCoroutine);
		}
		if (gameObject.activeInHierarchy)
		{
			SelectCarCoroutine = StartCoroutine (DoSelectCar (newCar));
		}
		else
		{
			DoForseSelectCar (newCar);
		}
	}

	void DoForseSelectCar (CarPreset newCar)
	{
		float targetPower = (newCar.GetPower) / (MaxPower);
		float targetControl = (newCar.GetControl) / (MaxControl);
		float targetMass = (newCar.GetMass) / (MaxMass);

		PowerSlider.value = targetPower;
		ControlSlider.value = targetControl;
		MassSlider.value = targetMass;
	}

	IEnumerator DoSelectCar (CarPreset newCar)
	{
		float targetPower = (newCar.GetPower) / (MaxPower);
		float targetControl = (newCar.GetControl) / (MaxControl);
		float targetMass = (newCar.GetMass) / (MaxMass);

		float currentPower = PowerSlider.value;
		float currentControl = ControlSlider.value;
		float currentMass = MassSlider.value;

		while (!Mathf.Approximately (targetPower, PowerSlider.value)
			|| !Mathf.Approximately (targetControl, ControlSlider.value)
			|| !Mathf.Approximately (targetMass, MassSlider.value)
		)
		{
			currentPower = Mathf.MoveTowards (currentPower, targetPower, Time.deltaTime * ChangeParamsSpeed);
			currentControl = Mathf.MoveTowards (currentControl, targetControl, Time.deltaTime * ChangeParamsSpeed);
			currentMass = Mathf.MoveTowards (currentMass, targetMass, Time.deltaTime * ChangeParamsSpeed);

			PowerSlider.value = currentPower;
			ControlSlider.value = currentControl;
			MassSlider.value = currentMass;

			yield return null;
		}

		SelectCarCoroutine = null;
	}
}
