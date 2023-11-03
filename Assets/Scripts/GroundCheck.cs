using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
	public GameObject me;
	private int count = 0;
	public bool IsGrounded => count > 0;
	void OnEnable()
	{
		count = 0;
	}

	private void OnTriggerEnter(Collider other)
	{
		if( other.gameObject != me )
			count ++;
	}

	private void OnTriggerExit(Collider other)
	{
		if( other.gameObject != me)
			count--;
	}
}
