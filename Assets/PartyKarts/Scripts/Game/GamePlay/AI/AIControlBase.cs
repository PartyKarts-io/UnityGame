using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class AI control.
/// </summary>
[RequireComponent (typeof (CarController))]
public class AIControlBase :MonoBehaviour, ICarControl
{
	public float Vertical { get; protected set; }
	public float Horizontal { get; protected set; }
	public bool Brake { get; protected set; }

	public bool HasLimit { get { return CurrentLimitZone != null; } }
	protected float SpeedLimit { get; private set; }
	protected bool NeedBrake { get; private set; }

	LimitSpeedTriggerZone CurrentLimitZone;

	public void OnTriggerEnter (Collider other)
	{
		var zone = other.GetComponent<LimitSpeedTriggerZone>();
		if (zone != null)
		{
			CurrentLimitZone = zone;
			SpeedLimit = CurrentLimitZone.LimitSpeed;
			NeedBrake = CurrentLimitZone.NeedBrake;
		}
	}

	public void OnTriggerExit (Collider other)
	{
		if (HasLimit && other.gameObject == CurrentLimitZone.gameObject)
		{
			CurrentLimitZone = null;
			SpeedLimit = 0;
			NeedBrake = false;
		}
	}
}
