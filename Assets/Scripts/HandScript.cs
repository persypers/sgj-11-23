using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandScript : MonoBehaviour
{
	public float linealSpringBase = 100f;
	public float slerpSpringBase = 300f;

	public float minScaledMass = 9.0f;
	public float maxScaledMass = 60.0f;
	public AnimationCurve massScaleCurve;

	public ConfigurableJoint armJoint;
	public ConfigurableJoint gimbalJoint;
	public ConfigurableJoint gimbalJointPrefab;
	public Rigidbody itemDummy;

	Item item;	
	Quaternion localGimbalRotation;

	public bool IsEmpty => item == null;

	void Start()
	{
		var go = new GameObject( "GimbalJointPrefab" );
		go.transform.parent = transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		go.SetActive( false );
		gimbalJointPrefab = Fancy.Helpers.CopyComponent< ConfigurableJoint >( gimbalJoint, go );

		itemDummy.transform.SetParent( null );
		itemDummy.transform.position = transform.position;
		gimbalJoint.connectedBody = itemDummy;

		LockGimbal( false );
	}

	void OnEnable()
	{
		armJoint = GetComponent< ConfigurableJoint >();
		UpdateSprings();
	}

	public void PickUp( Item item, Rigidbody body, float armDistance )
	{
		if( !IsEmpty )
			return;

		//var localRotation = transform.rotation * Quaternion.Inverse( body.transform.rotation );		// zeroed
		/*var cubeRot = Quaternion.Euler( 45.0f, 90.0f, 0.0f );
		cubeRot = Quaternion.identity;
		var cubeInJointLocal = cubeRot = Quaternion.Inverse( transform.rotation ) * body.transform.rotation;
		var localRotation = transform.rotation * cubeRot * Quaternion.Inverse( body.transform.rotation );
		*/

		this.item = item;

		var localRotation = Quaternion.identity;

		var euler = localRotation.eulerAngles;
		var targetRotation = Quaternion.Euler( -euler.x, -euler.z, euler.y );

		var targetPosition = armJoint.targetPosition;
		targetPosition.y = Mathf.Clamp( -( armDistance - Mathf.Abs( armJoint.anchor.z ) ), -armJoint.linearLimit.limit, armJoint.linearLimit.limit );
		armJoint.targetPosition = targetPosition;

		gimbalJoint.connectedBody = body;

		var offcenteredMass = body.GetComponent< OffcenteredMass >();
		if( offcenteredMass != null )
			offcenteredMass.enabled = false;

		gimbalJoint.targetRotation = targetRotation;

		UpdateSprings();
		LockGimbal( true );

		item.PickedUp();

		// TODO: detach item joints
	}

	public GameObject Drop()
	{
		if( IsEmpty )
			return null;

		var offcenteredMass = item.GetComponent< OffcenteredMass >();
		if( offcenteredMass != null )
			offcenteredMass.enabled = true;

		//gimbalJoint.connectedBody = null;
		itemDummy.transform.position = transform.position;
		gimbalJoint.connectedBody = itemDummy;

		LockGimbal( false );

		item.Dropped();

		var result = item.gameObject;
		item = null;
		return result;
	}

	void LockGimbal( bool enableJoint )
	{
		var value = enableJoint ? ConfigurableJointMotion.Locked : ConfigurableJointMotion.Free;
		gimbalJoint.xMotion = value;
		gimbalJoint.yMotion = value;
		gimbalJoint.zMotion = value;
		value = enableJoint ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Free;
		gimbalJoint.angularXMotion = value;
		gimbalJoint.angularYMotion = value;
		gimbalJoint.angularZMotion = value;
	}

	public void SetTargetRotation( Vector3 euler )
	{
		armJoint.targetRotation = Quaternion.Euler( -euler.x, -euler.z, euler.y );		// ...yeah :-(
	}

	public void SetGimbalTargetRotation( Vector3 euler )
	{

	}

	public void UpdateSprings()
	{
		float mass = GetComponent< Rigidbody >().mass;
		if( item != null )
			mass += item.GetComponent< Rigidbody >().mass;

		float k = 1.0f;

		var drive = armJoint.xDrive;
		drive.positionSpring = linealSpringBase * k;
		drive.positionDamper = 2.0f * k * Mathf.Sqrt( linealSpringBase );
		armJoint.xDrive = drive;
		armJoint.yDrive = drive;
		armJoint.zDrive = drive;

		drive = armJoint.slerpDrive;
		drive.positionSpring = slerpSpringBase * k;
		drive.positionDamper = 2.0f * k * Mathf.Sqrt( slerpSpringBase );
		armJoint.slerpDrive = drive;
		gimbalJoint.slerpDrive = drive;

		var massScale = massScaleCurve.Evaluate( Mathf.InverseLerp( minScaledMass, maxScaledMass, mass ) );
		armJoint.massScale = mass * massScale;
		gimbalJoint.massScale = mass * massScale;
	}

	void OnJointBreak(float breakForce)
	{
		if( item != null )
		{
			gimbalJoint = Fancy.Helpers.CopyComponent< ConfigurableJoint >( gimbalJointPrefab, gameObject );
			Drop();
		}
		Debug.Log("Hand joint has just been broken!, force: " + breakForce);
	}
}