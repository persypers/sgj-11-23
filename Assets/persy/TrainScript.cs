using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TrainScript : Fancy.MonoSingleton< TrainScript >
{
	public float maxSpeed = 30.0f;
	public float accelerationl = 5.0f;
	public float brakeDeceleration = 12.0f;
	public float noFuelDeceleration = 0.1f;
	
	public bool go = false;
	public bool hasFuel = true;
	public bool brakes = false;
	
	Rigidbody body;

	public Vector3 currentVelocity => body.velocity;

	void Start()
	{
		body = GetComponent< Rigidbody >();
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

	
}
