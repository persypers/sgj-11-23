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
		bool trainVolume = other.gameObject.layer == trainLayer;
		if( other.gameObject != me && !trainVolume)
		{
			count ++;
		}

		if( trainVolume )
		{
			trainCount ++;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		bool trainVolume = other.gameObject.layer == trainLayer;
		if( other.gameObject != me && !trainVolume)
		{
			count--;
		}

		if( trainVolume )
		{
			trainCount --;
		}
	}
}
