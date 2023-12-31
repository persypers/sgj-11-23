using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using FMOD.Studio;
using UnityEngine.Events;
//using UnityEditor.ShaderGraph;
using RigidFps;

public class TrainScript : Fancy.MonoSingleton< TrainScript >
{
	public float maxSpeed = 30.0f;
	public float accelerationl = 5.0f;
	public float brakeDeceleration = 12.0f;
	public float noFuelDeceleration = 0.4f;

	public float emergyBrakesOnDistanceFromPlayer = 70.0f;
	
	public bool go = false;
	public bool hasFuel = true;
	public bool brakes = false;
	public bool emergencyBrake = false;
	
	Rigidbody body;

	public Vector3 currentVelocity => body.velocity;
	public float currentSpeed => currentVelocity.magnitude;
	public bool IsDriving => go && hasFuel;

	[System.Serializable]
	public class Indicators
	{
		public GameObject driveOn;
		public GameObject driveOff;
		public GameObject brakeOn;
		public GameObject brakeOff;
		public GameObject emergencyOn;
		public GameObject emergencyOff;
	}
	public Indicators indicators;


	public Lever driveLever;

	public UnityEvent OnControlsChanged;

	void Start()
	{
		body = GetComponent< Rigidbody >();
	}

	public void IControlTheTrain( bool enabled )
	{
		if( emergencyBrake )
			return;

		bool wasEnabled = go;
		go = enabled;
		brakes = !enabled;
		driveLever.SetNoCallback( enabled );
		if( wasEnabled != enabled )
			OnControlsChanged.Invoke();
		UpdateIndicators();
	}

	public void DoEmergencyBrake()
	{
		IControlTheTrain( false );
		driveLever.disableSet = true;
		emergencyBrake = true;
		UpdateIndicators();
	}

	// рычаг газа/тормоза вызывает это, когда игрок берётся за него
	public void ClearEmergencyBrake()
	{
		driveLever.disableSet = false;
		emergencyBrake = false;
		UpdateIndicators();
	}

	public void UpdateIndicators()
	{
		indicators.driveOn.SetActive( go );
		indicators.driveOff.SetActive( !go );
		indicators.brakeOn.SetActive( brakes );
		indicators.brakeOff.SetActive( !brakes );
		indicators.emergencyOn.SetActive( emergencyBrake );
		indicators.emergencyOff.SetActive( !emergencyBrake );
	}

	void FixedUpdate()
	{
		float dt = Time.fixedDeltaTime;

		// figure out horizontal acceleration to be applied
		var horizontalVelocity = Vector3.ProjectOnPlane( body.velocity, Vector3.up );

		var desiredVelocity = ( go && hasFuel ) ? new Vector3( 0.0f, 0.0f, maxSpeed ) : Vector3.zero;

		var velDiff = desiredVelocity - horizontalVelocity;

		float maxAccelMagnitude = Mathf.Min( velDiff.magnitude / dt, ( go && hasFuel ) ? accelerationl : brakes ? brakeDeceleration : noFuelDeceleration );
		var horizontalAccel = velDiff.normalized * maxAccelMagnitude;

		body.AddForce( horizontalAccel, ForceMode.Acceleration );
	}

	void Update()
	{
		if( !emergencyBrake && go && hasFuel )
		{
			var distanceToPlayerSqr = ( transform.position - Player.Instance.transform.position ).sqrMagnitude;
			if( distanceToPlayerSqr > emergyBrakesOnDistanceFromPlayer * emergyBrakesOnDistanceFromPlayer )
				DoEmergencyBrake();
		}

		//Fmod.SetParameter( "skorost" ) = currentSpeed / maxSpeed;
		// Calculate the normalized speed (0 to 1) of the train
    	float normalizedSpeed = currentSpeed / maxSpeed;

    	// Set the FMOD global parameter "SPEED" to the normalized speed value
    	FMODUnity.RuntimeManager.StudioSystem.setParameterByName("SPEED", normalizedSpeed);
	}
}
