using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandScript : MonoBehaviour
{
	public float linealSpringBase = 100f;
	public float slerpSpringBase = 300f;

	public ConfigurableJoint armJoint;
	public ConfigurableJoint gimbalJoint;
	public Rigidbody itemDummy;

	GameObject item;
	bool cachedGravity;
	RigidbodyInterpolation cachedInterpolationMode;
	
	Quaternion localGimbalRotation;

	public bool IsEmpty => item == null;

	void Start()
	{
		itemDummy.transform.SetParent( null );
		if( gimbalJoint.connectedBody != null && gimbalJoint.connectedBody != itemDummy )
			PickUp( gimbalJoint.connectedBody );
		else
		{
			itemDummy.transform.position = transform.position;
			gimbalJoint.connectedBody = itemDummy;
		}

	}

	void OnEnable()
	{
		armJoint = GetComponent< ConfigurableJoint >();
		UpdateSprings();
	}

	public void PickUp( Rigidbody body )
	{
		if( !IsEmpty )
			return;

		//var localRotation = transform.rotation * Quaternion.Inverse( body.transform.rotation );		// zeroed
		/*var cubeRot = Quaternion.Euler( 45.0f, 90.0f, 0.0f );
		cubeRot = Quaternion.identity;
		var cubeInJointLocal = cubeRot = Quaternion.Inverse( transform.rotation ) * body.transform.rotation;
		var localRotation = transform.rotation * cubeRot * Quaternion.Inverse( body.transform.rotation );
		*/

		var localRotation = Quaternion.identity;

		var euler = localRotation.eulerAngles;
		var targetRotation = Quaternion.Euler( -euler.x, -euler.z, euler.y );

		cachedGravity = body.useGravity;
		body.useGravity = false;
		cachedInterpolationMode = body.interpolation;
		body.interpolation = RigidbodyInterpolation.Interpolate;
		gimbalJoint.connectedBody = body;

		item = body.gameObject;

		gimbalJoint.targetRotation = targetRotation;

		UpdateSprings();
		LockGimbal( true );

		// TODO: detach item joints
	}

	public GameObject Drop()
	{
		if( IsEmpty )
			return null;
		
		var rb = item.GetComponent< Rigidbody >();
		rb.useGravity = cachedGravity;
		rb.interpolation = cachedInterpolationMode;
		//gimbalJoint.connectedBody = null;
		itemDummy.transform.position = transform.position;
		gimbalJoint.connectedBody = itemDummy;

		LockGimbal( false );

		var result = item;
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

		var drive = armJoint.xDrive;
		drive.positionSpring = linealSpringBase * mass;
		drive.positionDamper = 2.0f * mass * Mathf.Sqrt( linealSpringBase );
		armJoint.xDrive = drive;
		armJoint.yDrive = drive;
		armJoint.zDrive = drive;

		drive = armJoint.slerpDrive;
		drive.positionSpring = slerpSpringBase * mass;
		drive.positionDamper = 2.0f * mass * Mathf.Sqrt( slerpSpringBase );
		armJoint.slerpDrive = drive;

		gimbalJoint.slerpDrive = drive;
	}
}