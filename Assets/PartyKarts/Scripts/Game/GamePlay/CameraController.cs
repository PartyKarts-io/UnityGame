using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Move and rotation camera controller
/// </summary>

public class CameraController :Singleton<CameraController>
{
	[SerializeField] List<CameraPreset> CamerasPreset = new List<CameraPreset>();   //Camera presets
    [SerializeField] CarController OverrideCar;

	int ActivePresetIndex = -1;
	public CameraPreset ActivePreset { get; private set; }

	CarController TargetCar { get { return OverrideCar?? GameController.PlayerCar; } }
	GameController GameController { get { return GameController.Instance; } }

	float SqrMinDistance;
	int CurrentFrame = 0;

	//The target point is calculated from velocity of car.
	Vector3 m_TargetPoint;
	Vector3 TargetPoint
	{
		get
		{
			if (CurrentFrame != Time.frameCount)
			{
				if (!OverrideCar && GameController == null || TargetCar == null)
				{
					return transform.position;
				}
				m_TargetPoint = TargetCar.RB.velocity * ActivePreset.VelocityMultiplier;
				m_TargetPoint += TargetCar.transform.position;

				CurrentFrame = Time.frameCount;
			}
			return m_TargetPoint;
		}
	}

	protected override void AwakeSingleton ()
	{
		CamerasPreset.ForEach (c => c.CameraHolder.SetActive (false));

		ActivePresetIndex = GameOptions.ActiveCameraIndex;
		UpdateActiveCamera ();
	}

	private IEnumerator Start ()
	{
		while (!OverrideCar && (GameController == null || TargetCar == null))
		{
			yield return null;
		}
		transform.position = TargetPoint;
		ActivePreset.CameraHolder.rotation = TargetCar.transform.rotation;
	}

	private void Update ()
	{
		if (ActivePreset.EnableRotation)
		{
			var position = transform.position.ZeroHeight ();
			var target = TargetPoint.ZeroHeight ();

			if ((position - target).sqrMagnitude >= SqrMinDistance)
			{
				Quaternion rotation = Quaternion.LookRotation (target - position, Vector3.up);
				ActivePreset.CameraHolder.rotation = Quaternion.Lerp (ActivePreset.CameraHolder.rotation, rotation, Time.deltaTime * ActivePreset.SetRotationSpeed);
			}
		}

		transform.position = Vector3.LerpUnclamped (transform.position, TargetPoint, Time.deltaTime * ActivePreset.SetPositionSpeed);

		if (Input.GetKeyDown (KeyCode.C) || Input.GetKeyDown (KeyCode.Joystick1Button2))
		{
			SetNextCamera ();
		}
	}

	public void SetNextCamera ()
	{
		ActivePresetIndex = MathExtentions.LoopClamp (ActivePresetIndex + 1, 0, CamerasPreset.Count);
		GameOptions.ActiveCameraIndex = ActivePresetIndex;
		UpdateActiveCamera ();
	}

	public void UpdateActiveCamera ()
	{
		if (ActivePreset != null)
		{
			ActivePreset.CameraHolder.SetActive (false);
		}

		ActivePreset = CamerasPreset[ActivePresetIndex];
		ActivePreset.CameraHolder.SetActive (true);

		SqrMinDistance = ActivePreset.MinDistanceForRotation * 2;

		if (ActivePreset.EnableRotation && (TargetPoint - transform.position).sqrMagnitude >= SqrMinDistance)
		{
			Quaternion rotation = Quaternion.LookRotation (TargetPoint - transform.position, Vector3.up);
			ActivePreset.CameraHolder.rotation = rotation;
		}
	}

	private void OnDrawGizmosSelected ()
	{
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere (TargetPoint, 1);
            Gizmos.color = Color.white;
        }
	}

	[System.Serializable]
	public class CameraPreset
	{
		public Transform CameraHolder;                  //Parent fo camera.
		public float SetPositionSpeed = 1;              //Change position speed.
		public float VelocityMultiplier;                //Velocity of car multiplier.

		public bool EnableRotation;
		public float MinDistanceForRotation = 0.1f;     //Min distance for potation, To avoid uncontrolled rotation.
		public float SetRotationSpeed = 1;              //Change rotation speed.
	}
}
