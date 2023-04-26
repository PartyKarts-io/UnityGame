using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Main car controller
/// </summary>

[RequireComponent (typeof (Rigidbody))]
public class CarController :MonoBehaviourPunCallbacks, ISetColor
{

	[SerializeField] Wheel FrontLeftWheel;
	[SerializeField] Wheel FrontRightWheel;
	[SerializeField] Wheel RearLeftWheel;
	[SerializeField] Wheel RearRightWheel;
	[SerializeField] Transform COM;
	[SerializeField] List<ParticleSystem> BackFireParticles = new List<ParticleSystem>();
	[SerializeField] Renderer BaseView;                                                                     //To determine the visibility of the car.

	[SerializeField] CarConfig CarConfig;
    [SerializeField] CarDriftConfig CarDriftConfig;

    #region Properties of car parameters

    float MaxMotorTorque;
	float MaxSteerAngle { get { return CarConfig.MaxSteerAngle; } }
	DriveType DriveType { get { return CarConfig.DriveType; } }
	bool AutomaticGearBox { get { return CarConfig.AutomaticGearBox; } }
	AnimationCurve MotorTorqueFromRpmCurve { get { return CarConfig.MotorTorqueFromRpmCurve; } }
	float MaxRPM { get { return CarConfig.MaxRPM; } }
	float MinRPM { get { return CarConfig.MinRPM; } }
	float CutOffRPM { get { return CarConfig.CutOffRPM; } }
	float CutOffOffsetRPM { get { return CarConfig.CutOffOffsetRPM; } }
	float RpmToNextGear { get { return CarConfig.RpmToNextGear; } }
	float RpmToPrevGear { get { return CarConfig.RpmToPrevGear; } }
	float MaxForwardSlipToBlockChangeGear { get { return CarConfig.MaxForwardSlipToBlockChangeGear; } }
	float RpmEngineToRpmWheelsLerpSpeed { get { return CarConfig.RpmEngineToRpmWheelsLerpSpeed; } }
	float[] GearsRatio { get { return CarConfig.GearsRatio; } }
	float MainRatio { get { return CarConfig.MainRatio; } }
	float ReversGearRatio { get { return CarConfig.ReversGearRatio; } }

	float MaxBrakeTorque { get { return CarConfig.MaxBrakeTorque; } }
	float TargetSpeedIfBrakingGround { get { return CarConfig.TargetSpeedIfBrakingGround; } }
	float BrakingSpeedOneWheelTime { get { return CarConfig.BrakingSpeedOneWheelTime; } }

	#endregion //Properties of car parameters

	#region Properties of drif Settings

	public bool EnableSteerAngleMultiplier { get { return CarDriftConfig.EnableSteerAngleMultiplier; } }
	float MinSteerAngleMultiplier { get { return CarDriftConfig.MinSteerAngleMultiplier; } }
	float MaxSteerAngleMultiplier { get { return CarDriftConfig.MaxSteerAngleMultiplier; } }
	float MaxSpeedForMinAngleMultiplier { get { return CarDriftConfig.MaxSpeedForMinAngleMultiplier; } }
	float SteerAngleChangeSpeed { get { return CarDriftConfig.SteerAngleChangeSpeed; } }
	float MinSpeedForSteerHelp { get { return CarDriftConfig.MinSpeedForSteerHelp; } }
	float HelpSteerPower { get { return CarDriftConfig.HelpSteerPower; } }
	float OppositeAngularVelocityHelpPower { get { return CarDriftConfig.OppositeAngularVelocityHelpPower; } }
	float PositiveAngularVelocityHelpPower { get { return CarDriftConfig.PositiveAngularVelocityHelpPower; } }
	float MaxAngularVelocityHelpAngle { get { return CarDriftConfig.MaxAngularVelocityHelpAngle; } }
	float AngularVelucityInMaxAngle { get { return CarDriftConfig.AngularVelucityInMaxAngle; } }
	float AngularVelucityInMinAngle { get { return CarDriftConfig.AngularVelucityInMinAngle; } }

	#endregion //Properties of drif Settings

	PositioningCar m_PositioningCar;
	public PositioningCar PositioningCar
	{
		get
		{
			if (m_PositioningCar == null)
			{
				m_PositioningCar = GetComponent<PositioningCar> ();
				if (m_PositioningCar == null)
				{
					m_PositioningCar = gameObject.AddComponent<PositioningCar> ();
				}
			}
			return m_PositioningCar;
		}
	}
    public Wheel GetFrontLeftWheel { get { return FrontLeftWheel; } }
    public Wheel GetFrontRightWheel { get { return FrontRightWheel; } }
    public Wheel GetRearLeftWheel { get { return RearLeftWheel; } }
    public Wheel GetRearRightWheel { get { return RearRightWheel; } }
    
    public CarConfig GetCarConfig { get { return CarConfig; } }
	public Wheel[] Wheels { get; private set; }										//All wheels, public link.			
	public System.Action<CarController, Collision> CollisionAction;                 //Action invoked in collision.
	public System.Action ResetCarAction;											//Action invoked in reset position.
	public System.Action BackFireAction;                                            //Backfire invoked when cut off (You can add a invoke when changing gears).

	float[] AllGearsRatio;															 //All gears (Reverce, neutral and all forward).

	Rigidbody _RB;
	public Rigidbody RB
	{
		get
		{
			if (!_RB)
			{
				_RB = GetComponent<Rigidbody> ();
			}
			return _RB;
		}
	}

	List <SetColorForMaskMaterial> m_SetColorMeshes;
	List<SetColorForMaskMaterial> SetColorMeshes
	{
		get
		{
			if (m_SetColorMeshes == null)
			{
				m_SetColorMeshes = (gameObject.GetComponentsInChildren<SetColorForMaskMaterial> (true)).ToList ();
			}
			return m_SetColorMeshes;
		}
	}

	public bool CarIsVisible { get { return BaseView.isVisible; } }
	public float CurrentMaxSlip { get; private set; }               //Max slip of all wheels.
	public int CurrentMaxSlipWheelIndex { get; private set; }       //Max slip wheel index.
	public float CurrentSpeed { get; private set; }                 //Speed, magnitude of velocity.
	public float SpeedInHour { get { return CurrentSpeed * C.KPHMult; } }
	public int CarDirection { get { return CurrentSpeed < 1 ? 0 : (VelocityAngle < 90 && VelocityAngle > -90 ? 1 : -1); } }

	float CurrentSteerAngle;
	float CurrentAcceleration;
	float CurrentBrake;
	bool InHandBrake;

	int FirstDriveWheel;
	int LastDriveWheel;

	private void Awake ()
	{
		//For load the prefab in the select car menu.
		if (GameController.InMainMenuScene)
		{
			var bodyTilt = GetComponent<BodyTilt>();

			bodyTilt.enabled = false;
			this.enabled = false;

			return;
		}

		if (BaseView == null || !BaseView.gameObject.activeInHierarchy)
		{
			BaseView = gameObject.GetComponentInChildren<Renderer> ();
		}

		RB.centerOfMass = COM.localPosition;

		//Copy wheels in public property
		Wheels = new Wheel[4] {
			FrontLeftWheel,
			FrontRightWheel,
			RearLeftWheel,
			RearRightWheel
		};

		foreach (var wheel in Wheels)
		{
			wheel.WheelCollider.ConfigureVehicleSubsteps (40, 13, 8);
		}

		//Set drive wheel.
		switch (DriveType)
		{
			case DriveType.AWD:
			FirstDriveWheel = 0;
			LastDriveWheel = 3;
			break;
			case DriveType.FWD:
			FirstDriveWheel = 0;
			LastDriveWheel = 1;
			break;
			case DriveType.RWD:
			FirstDriveWheel = 2;
			LastDriveWheel = 3;
			break;
		}

		//Divide the motor torque by the count of driving wheels
		MaxMotorTorque = CarConfig.MaxMotorTorque / (LastDriveWheel - FirstDriveWheel + 1);


		//Calculated gears ratio with main ratio
		AllGearsRatio = new float[GearsRatio.Length + 2];
		AllGearsRatio[0] = ReversGearRatio * MainRatio;
		AllGearsRatio[1] = 0;
		for (int i = 0; i < GearsRatio.Length; i++)
		{
			AllGearsRatio[i + 2] = GearsRatio[i] * MainRatio;
		}

		foreach (var particles in BackFireParticles)
		{
			BackFireAction += () => particles.Emit (2);
		}
	}

	private void Start ()
	{
        //Subscribe to the sound in a collision.
        if (FXController.Instance != null)
        {
            CollisionAction += FXController.Instance.PlayCollisionSound;
        }
	}

    public void SetCarDriftConfig (CarDriftConfig config)
    {
        CarDriftConfig = config;
    }

	/// <summary>
	/// Update controls of car, from user control (TODO AI control).
	/// </summary>
	/// <param name="horizontal">Turn direction</param>
	/// <param name="vertical">Acceleration</param>
	/// <param name="brake">Brake</param>
	public void UpdateControls (float horizontal, float vertical, bool brake)
	{
		float targetSteerAngle = horizontal * MaxSteerAngle;

		if (EnableSteerAngleMultiplier)
		{
			targetSteerAngle *= Mathf.Clamp (1 - SpeedInHour / MaxSpeedForMinAngleMultiplier, MinSteerAngleMultiplier, MaxSteerAngleMultiplier);
		}

		CurrentSteerAngle = Mathf.MoveTowards (CurrentSteerAngle, targetSteerAngle, Time.deltaTime * SteerAngleChangeSpeed);

		CurrentAcceleration = vertical;

		if (InHandBrake != brake)
		{
			float forwardStiffness = brake? CarDriftConfig.HandBrakeForwardStiffness: 1;
			float sidewaysStiffness = brake? CarDriftConfig.HandBrakeSidewaysStiffness: 1;
			RearLeftWheel.PG_WheelCollider.UpdateStiffness (forwardStiffness, sidewaysStiffness);
			RearRightWheel.PG_WheelCollider.UpdateStiffness (forwardStiffness, sidewaysStiffness);
		}

		InHandBrake = brake;
	}

	private void Update ()
	{
		for (int i = 0; i < Wheels.Length; i++)
		{
			Wheels[i].UpdateVisual (CarIsVisible);
		}
	}

	private void FixedUpdate ()
	{

		CurrentSpeed = RB.velocity.magnitude;

		UpdateSteerAngleLogic ();
		UpdateRpmAndTorqueLogic ();

		//Find max slip and update braking ground logic.
		CurrentMaxSlip = Wheels[0].CurrentMaxSlip;
		CurrentMaxSlipWheelIndex = 0;
		int wheelOnBrakingGroundCount = 0;

		if (InHandBrake)
		{
			RearLeftWheel.WheelCollider.brakeTorque = MaxBrakeTorque;
			RearRightWheel.WheelCollider.brakeTorque = MaxBrakeTorque;
			FrontLeftWheel.WheelCollider.brakeTorque = 0;
			FrontRightWheel.WheelCollider.brakeTorque = 0;
		}

		for (int i = 0; i < Wheels.Length; i++)
		{
			if (!InHandBrake)
			{
				Wheels[i].WheelCollider.brakeTorque = CurrentBrake;
			}

			Wheels[i].FixedUpdate ();

			if (CurrentMaxSlip < Wheels[i].CurrentMaxSlip)
			{
				CurrentMaxSlip = Wheels[i].CurrentMaxSlip;
				CurrentMaxSlipWheelIndex = i;
			}

			if (Wheels[i].WheelCollider.isGrounded && B.LayerSettings.BrakingGroundMask.LayerInMask (Wheels[i].GetHit.collider.gameObject.layer))
			{
				wheelOnBrakingGroundCount++;
			}
		}

		if (wheelOnBrakingGroundCount > 0 && SpeedInHour > TargetSpeedIfBrakingGround)
		{
			RB.velocity = Vector3.MoveTowards (RB.velocity, Vector3.zero, wheelOnBrakingGroundCount * BrakingSpeedOneWheelTime * Time.deltaTime);
		}

	}

	private void OnCollisionEnter (Collision collision)
	{
		CollisionAction.SafeInvoke (this, collision);
	}

	private void OnTriggerEnter (Collider trigger)
	{
		if (trigger.gameObject.tag == C.ResetCarTriggerTag)
		{
			ResetPosition ();
			CollisionAction.SafeInvoke (this, null);
		}
	}

	#region Steer help logic

	//Angle between forward point and velocity point.
	public float VelocityAngle { get; private set; }

	/// <summary>
	/// Update all helpers logic.
	/// </summary>
	void UpdateSteerAngleLogic ()
	{
		var needHelp = SpeedInHour > MinSpeedForSteerHelp && CarDirection > 0;
		float targetAngle = 0;
		VelocityAngle = -Vector3.SignedAngle (RB.velocity, transform.TransformDirection (Vector3.forward), Vector3.up);

		if (needHelp)
		{
			//Wheel turning helper.
			targetAngle = Mathf.Clamp (VelocityAngle * HelpSteerPower, -MaxSteerAngle, MaxSteerAngle);
		}

		//Wheel turn limitation.
		targetAngle = Mathf.Clamp (targetAngle + CurrentSteerAngle, -(MaxSteerAngle + 10), MaxSteerAngle + 10);

		//Front wheel turn.
		Wheels[0].WheelCollider.steerAngle = targetAngle;
		Wheels[1].WheelCollider.steerAngle = targetAngle;

		if (needHelp)
		{
			//Angular velocity helper.
			var absAngle = Mathf.Abs (VelocityAngle);

			//Get current procent help angle.
			float currentAngularProcent = absAngle / MaxAngularVelocityHelpAngle;

			var currAngle = RB.angularVelocity;

			if (VelocityAngle * CurrentSteerAngle > 0)
			{
				//Turn to the side opposite to the angle. To change the angular velocity.
				var angularVelocityMagnitudeHelp = OppositeAngularVelocityHelpPower * CurrentSteerAngle * Time.fixedDeltaTime;
				currAngle.y += angularVelocityMagnitudeHelp * currentAngularProcent;
			}
			else if (!Mathf.Approximately (CurrentSteerAngle, 0))
			{
				//Turn to the side positive to the angle. To change the angular velocity.
				var angularVelocityMagnitudeHelp = PositiveAngularVelocityHelpPower * CurrentSteerAngle * Time.fixedDeltaTime;
				currAngle.y += angularVelocityMagnitudeHelp * (1 - currentAngularProcent);
			}

			//Clamp and apply of angular velocity.
			var maxMagnitude = ((AngularVelucityInMaxAngle - AngularVelucityInMinAngle) * currentAngularProcent) + AngularVelucityInMinAngle;
			currAngle.y = Mathf.Clamp (currAngle.y, -maxMagnitude, maxMagnitude);
			RB.angularVelocity = currAngle;
		}
	}

	#endregion //Steer help logic

	#region Rpm and torque logic

	public int CurrentGear { get; private set; }
	public int CurrentGearIndex { get { return CurrentGear + 1; } }
	public float EngineRPM { get; private set; }
	public float GetMaxRPM { get { return MaxRPM; } }
	public float GetMinRPM { get { return MinRPM; } }
	public float GetInCutOffRPM { get { return CutOffRPM - CutOffOffsetRPM; } }

	float CutOffTimer;
	bool InCutOff;

	void UpdateRpmAndTorqueLogic ()
	{

		if (InCutOff)
		{
			if (CutOffTimer > 0)
			{
				CutOffTimer -= Time.fixedDeltaTime;
				EngineRPM = Mathf.Lerp (EngineRPM, GetInCutOffRPM, RpmEngineToRpmWheelsLerpSpeed * Time.fixedDeltaTime);
			}
			else
			{
				InCutOff = false;
			}
		}

		if (GameController.Instance != null && !GameController.RaceIsStarted)
		{
			if (InCutOff) return;

			float rpm = CurrentAcceleration > 0 ? MaxRPM : MinRPM;
			float speed = CurrentAcceleration > 0 ? RpmEngineToRpmWheelsLerpSpeed : RpmEngineToRpmWheelsLerpSpeed * 0.2f;
			EngineRPM = Mathf.Lerp (EngineRPM, rpm, speed * Time.fixedDeltaTime);
			if (EngineRPM >= CutOffRPM)
			{
				PlayBackfireWithProbability ();
				InCutOff = true;
				CutOffTimer = CarConfig.CutOffTime;
			}
			return;
		}

		//Get drive wheel with MinRPM.
		float minRPM = 0;
		for (int i = FirstDriveWheel + 1; i <= LastDriveWheel; i++)
		{
			minRPM += Wheels[i].WheelCollider.rpm;
		}

		minRPM /= LastDriveWheel - FirstDriveWheel + 1;

		if (!InCutOff)
		{
			//Calculate the rpm based on rpm of the wheel and current gear ratio.
			float targetRPM = ((minRPM + 20) * AllGearsRatio[CurrentGearIndex]).Abs ();              //+20 for normal work CutOffRPM
			targetRPM = Mathf.Clamp (targetRPM, MinRPM, MaxRPM);
			EngineRPM = Mathf.Lerp (EngineRPM, targetRPM, RpmEngineToRpmWheelsLerpSpeed * Time.fixedDeltaTime);
		}

		if (EngineRPM >= CutOffRPM)
		{
			PlayBackfireWithProbability ();
			InCutOff = true;
			CutOffTimer = CarConfig.CutOffTime;
			return;
		}

		if (!Mathf.Approximately (CurrentAcceleration, 0))
		{
			//If the direction of the car is the same as Current Acceleration.
			if (CarDirection * CurrentAcceleration >= 0)
			{
				CurrentBrake = 0;

				float motorTorqueFromRpm = MotorTorqueFromRpmCurve.Evaluate (EngineRPM * 0.001f);
				var motorTorque = CurrentAcceleration * (motorTorqueFromRpm * (MaxMotorTorque * AllGearsRatio[CurrentGearIndex]));
				if (Mathf.Abs (minRPM) * AllGearsRatio[CurrentGearIndex] > MaxRPM)
				{
					motorTorque = 0;
				}

				//If the rpm of the wheel is less than the max rpm engine * current ratio, then apply the current torque for wheel, else not torque for wheel.
				float maxWheelRPM = AllGearsRatio[CurrentGearIndex] * EngineRPM;
				for (int i = FirstDriveWheel; i <= LastDriveWheel; i++)
				{
					if (Wheels[i].WheelCollider.rpm <= maxWheelRPM)
					{
						Wheels[i].WheelCollider.motorTorque = motorTorque;
					}
					else
					{
						Wheels[i].WheelCollider.motorTorque = 0;
					}
				}
			}
			else
			{
				CurrentBrake = MaxBrakeTorque;
				for (int i = FirstDriveWheel; i <= LastDriveWheel; i++)
				{
					Wheels[i].WheelCollider.motorTorque = 0;
				}
			}
		}
		else
		{
            CurrentBrake = 0;

            for (int i = FirstDriveWheel; i <= LastDriveWheel; i++)
			{
				Wheels[i].WheelCollider.motorTorque = 0;
			}
		}

		//Automatic gearbox logic. 
		if (AutomaticGearBox)
		{

			bool forwardIsSlip = false;
			for (int i = FirstDriveWheel; i <= LastDriveWheel; i++)
			{
				if (Wheels[i].CurrentForwardSleep > MaxForwardSlipToBlockChangeGear)
				{
					forwardIsSlip = true;
					break;
				}
			}

			float prevRatio = 0;
			float newRatio = 0;

			if (!forwardIsSlip && EngineRPM > RpmToNextGear && CurrentGear >= 0 && CurrentGear < (AllGearsRatio.Length - 2))
			{
				prevRatio = AllGearsRatio[CurrentGearIndex];
				CurrentGear++;
				newRatio = AllGearsRatio[CurrentGearIndex];
			}
			else if (EngineRPM < RpmToPrevGear && CurrentGear > 0 && (EngineRPM <= MinRPM || CurrentGear != 1))
			{
				prevRatio = AllGearsRatio[CurrentGearIndex];
				CurrentGear--;
				newRatio = AllGearsRatio[CurrentGearIndex];
			}

			if (!Mathf.Approximately (prevRatio, 0) && !Mathf.Approximately (newRatio, 0))
			{
				EngineRPM = Mathf.Lerp (EngineRPM, EngineRPM * (newRatio / prevRatio), RpmEngineToRpmWheelsLerpSpeed * Time.fixedDeltaTime); //EngineRPM * (prevRatio / newRatio);// 
			}

			if (CarDirection <= 0 && CurrentAcceleration < 0)
			{
				CurrentGear = -1;
			}
			else if (CurrentGear <= 0 && CarDirection >= 0 && CurrentAcceleration > 0)
			{
				CurrentGear = 1;
			}
			else if (CarDirection == 0 && CurrentAcceleration == 0)
			{
				CurrentGear = 0;
			}
		}

		//TODO manual gearbox logic.
	}
	void PlayBackfireWithProbability ()
	{
		PlayBackfireWithProbability (GetCarConfig.ProbabilityBackfire);
	}

	void PlayBackfireWithProbability (float probability)
	{
		if (Random.Range (0f, 1f) <= probability)
		{
			BackFireAction.SafeInvoke ();
		}
	}

	#endregion

	#region Visual

	public void SetColor (CarColorPreset color)
	{
		foreach (var c in SetColorMeshes)
		{
			c.SetColor (color);
		}
	}

	#endregion //Visual

	// Reset car position to prev checkpoint logic.
	float LastResetTime;
	Coroutine ResetPositionCoroutine;
	public void ResetPosition ()
	{
		if (ResetPositionCoroutine != null)
		{
			StopCoroutine (ResetPositionCoroutine);
		}
		ResetPositionCoroutine = StartCoroutine (DoResetPosition ());
	}

	/// <summary>
	/// A coroutine is used to wait for one FixedUpdate.
	/// </summary>
	IEnumerator DoResetPosition ()
	{
		if (PositioningCar.LastCorrectProgressDistance == 0 || (Time.time - LastResetTime) < 5)
		{
			yield break;
		}

		LastResetTime = Time.time;

		for (int i = 0; i < Wheels.Length; i++)
		{
			Wheels[i].StopEmitFX = true;
			Wheels[i].UpdateVisual (CarIsVisible);
		}

		CurrentGear = 0;
		EngineRPM = MinRPM;
		RB.velocity = Vector3.zero;
		RB.angularVelocity = Vector3.zero;
		transform.position = PositioningCar.LastCorrectPosition.position;
		transform.rotation = Quaternion.LookRotation (PositioningCar.LastCorrectPosition.direction, Vector3.up);
		ResetCarAction.SafeInvoke ();

		yield return new WaitForFixedUpdate ();

		for (int i = 0; i < Wheels.Length; i++)
		{
			Wheels[i].StopEmitFX = false;
		}

		ResetPositionCoroutine = null;
	}

	private void OnDrawGizmosSelected ()
	{
		var centerPos = transform.position;
		var velocity = transform.position + (Vector3.ClampMagnitude (RB.velocity, 4));
		var forwardPos = transform.TransformPoint (Vector3.forward * 4);

		Gizmos.color = Color.green;

		Gizmos.DrawWireSphere (centerPos, 0.2f);
		Gizmos.DrawLine (centerPos, velocity);
		Gizmos.DrawLine (centerPos, forwardPos);

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (forwardPos, 0.2f);

		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere (velocity, 0.2f);

		Gizmos.color = Color.white;
	}

}

public interface ICarControl
{
	float Horizontal { get; }
	float Vertical { get; }
	bool Brake { get; }
}

/// <summary>
/// For easy initialization and change of parameters in the future. TODO Add tuning.
/// </summary>
[System.Serializable]
public class CarConfig
{
	[Header("Steer Settings")]
	public float MaxSteerAngle = 25;

	[Header("Engine and power settings")]
	public DriveType DriveType = DriveType.RWD;				//Drive type AWD, FWD, RWD. With the current parameters of the car only RWD works well. TODO Add rally and offroad regime.
	public bool AutomaticGearBox = true;
	public float MaxMotorTorque = 150;						//Max motor torque engine (Without GearBox multiplier).
	public AnimationCurve MotorTorqueFromRpmCurve;			//Curve motor torque (Y(0-1) motor torque, X(0-7) motor RPM).
	public float MaxRPM = 7000;
	public float MinRPM = 700;
	public float CutOffRPM = 6800;							//The RPM at which the cutoff is triggered.
	public float CutOffOffsetRPM = 500;
	public float CutOffTime = 0.1f;
	[Range(0, 1)]public float ProbabilityBackfire = 0.2f;   //Probability backfire: 0 - off backfire, 1 always on backfire.
	public float RpmToNextGear = 6500;						//The speed at which there is an increase in gearbox.
	public float RpmToPrevGear = 4500;						//The speed at which there is an decrease in gearbox.
	public float MaxForwardSlipToBlockChangeGear = 0.5f;	//Maximum rear wheel slip for shifting gearbox.
	public float RpmEngineToRpmWheelsLerpSpeed = 15;		//Lerp Speed change of RPM.
	public float[] GearsRatio;								//Forward gears ratio.
	public float MainRatio;
	public float ReversGearRatio;							//Reverse gear ratio.

	[Header("Braking settings")]
	public float MaxBrakeTorque = 1000;
	public float TargetSpeedIfBrakingGround = 20;
	public float BrakingSpeedOneWheelTime = 2;
}

/// <summary>
/// For easy initialization and change of parameters in the future. TODO Add tuning.
/// </summary>
[System.Serializable]
public class CarDriftConfig
{
    public bool EnableSteerAngleMultiplier = true;
    [PG_Atributes.ShowInInspectorIf ("EnableSteerAngleMultiplier")] public float MinSteerAngleMultiplier = 0.05f;       //Min steer angle multiplayer to limit understeer at high speeds.
    [PG_Atributes.ShowInInspectorIf ("EnableSteerAngleMultiplier")] public float MaxSteerAngleMultiplier = 1f;          //Max steer angle multiplayer to limit understeer at high speeds.          
    [PG_Atributes.ShowInInspectorIf ("EnableSteerAngleMultiplier")] public float MaxSpeedForMinAngleMultiplier = 250;   //The maximum speed at which there will be a minimum steering angle multiplier.
    
    [Space(10)]
    public float SteerAngleChangeSpeed = 1f;                    //Wheel turn speed.
    public float MinSpeedForSteerHelp  = 20f;                   //Min speed at which helpers are enabled.
    [Range(0, 1)]public float HelpSteerPower = 0.1f;            //The power of turning the wheels in the direction of the drift.
    public float OppositeAngularVelocityHelpPower = 0.1f;       //The power of the helper to turn the rigidbody in the direction of the control turn.
    public float PositiveAngularVelocityHelpPower = 0.1f;       //The power of the helper to positive turn the rigidbody in the direction of the control turn.
    public float MaxAngularVelocityHelpAngle = 90f;             //The angle at which the assistant works 100%.
    public float AngularVelucityInMaxAngle = 0.5f;              //Min angular velocity, reached at max drift angles.
    public float AngularVelucityInMinAngle = 4f;                //Max angular velocity, reached at min drift angles.
    public float HandBrakeForwardStiffness = 0.5f;              //To change the friction of the rear wheels with a hand brake.
    public float HandBrakeSidewaysStiffness = 0.5f;             //To change the friction of the rear wheels with a hand brake.
}
