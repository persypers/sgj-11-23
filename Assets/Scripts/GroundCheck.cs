using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
	public GameObject me;
	private int count = 0;
	private int trainCount = 0;
	public int trainLayer = -1;
	public bool IsGrounded => count > 0;
	public bool IsGroundedOnTrain => trainCount > 0;
	void OnEnable()
	{
		count = 0;
	}

	private void OnTriggerEnter(Collider other)
	{
		if( other.gameObject != me )
		{
			count ++;
		}

		if( other.gameObject.layer == trainLayer )
		{
			trainCount ++;
			if( trainCount == 1 )
				Debug.Log( "Gained Train" );
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if( other.gameObject != me)
		{
			count--;
		}

		if( other.gameObject.layer == trainLayer )
		{
			trainCount --;
			if( trainCount == 0 )
				Debug.Log( "Lost Train" );
		}
	}
}
