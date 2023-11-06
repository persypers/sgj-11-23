using System.Collections;
using System.Collections.Generic;
using RigidFps;
using UnityEngine;

public class LeverHandle : MonoBehaviour
{
	public bool isDriveHandle;
	public GameObject outline;
	public void EnableOutline( bool isEnabled )
	{
		outline.gameObject.SetActive( isEnabled );
	}

	void OnCollisionEnter( Collision collision )
	{
		if( collision.body.gameObject == Player.Instance.gameObject )
			TrainScript.Instance.ClearEmergencyBrake();

	}
}
