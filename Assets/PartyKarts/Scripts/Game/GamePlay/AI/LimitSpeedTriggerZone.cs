using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is needed for AI to limit the speed in the desired zones.
public class LimitSpeedTriggerZone : MonoBehaviour {

	[SerializeField] float m_LimitSpeed = 50;
	[SerializeField] bool m_NeedBrake;

	public float LimitSpeed { get { return m_LimitSpeed; } }
	public bool NeedBrake { get { return m_NeedBrake; } }
}
