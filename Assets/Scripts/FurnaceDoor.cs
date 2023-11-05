using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceDoor : MonoBehaviour
{
	public GameObject upperDoor;
	public GameObject lowerDoor;
	public Vector3 upperOpenPosition;
	public Vector3 upperClosedPositon;
	public Vector3 lowerOpenPosition;
	public Vector3 lowerClosedPosition;
	public float doorOpenTime = 1.0f;

	public bool IsOpen {get; private set;}

	public void Open( bool isOpen )
	{
		IsOpen = isOpen;
	}
	// Start is called before the first frame update
	void Start()
	{
		upperClosedPositon = upperDoor.transform.localPosition;
		lowerClosedPosition = lowerDoor.transform.localPosition;
	}

	void FixedUpdate()
	{
		UpdateDoors( Time.fixedDeltaTime );
	}

	Vector3 upperVelocity;
	Vector3 lowerVelocity;
	void UpdateDoors( float dt )
	{
		upperDoor.transform.localPosition = Vector3.SmoothDamp(
			upperDoor.transform.localPosition,
			IsOpen ? upperOpenPosition : upperClosedPositon,
			ref upperVelocity,
			doorOpenTime,
			5.0f,
			dt
		);

		lowerDoor.transform.localPosition = Vector3.SmoothDamp(
			lowerDoor.transform.localPosition,
			IsOpen ? lowerOpenPosition : lowerClosedPosition,
			ref lowerVelocity,
			doorOpenTime,
			5.0f,
			dt
		);

	}
}
