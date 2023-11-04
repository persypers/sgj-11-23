using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffcenteredMass : MonoBehaviour
{
	public Vector3 offset;
	Vector3 cachedCenter;
	// Start is called before the first frame update
	void OnEnable()
	{
		var rb = GetComponent< Rigidbody >();
		cachedCenter = rb.centerOfMass;
		rb.centerOfMass = offset;
	}

	void OnDisable()
	{
		GetComponent< Rigidbody >().centerOfMass = cachedCenter;
	}
}
