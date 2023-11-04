using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
	public ItemTypes type;
	public float fuelValue;
	public bool IsHeld { get; private set;}

	public UnityEvent< Item > OnPickUp;
	public UnityEvent< Item > OnDrop;

	bool cachedGravity;
	RigidbodyInterpolation cachedInterpolationMode;

	public void PickedUp()
	{
		Debug.Assert( !IsHeld );
		var body = GetComponent< Rigidbody >();
		cachedGravity = body.useGravity;
		cachedInterpolationMode = body.interpolation;
		body.useGravity = false;
		body.interpolation = RigidbodyInterpolation.Interpolate;

		IsHeld = true;
		OnPickUp.Invoke( this );
	}

	public void Dropped()
	{
		Debug.Assert( IsHeld );
		var body = GetComponent< Rigidbody >();
		body.useGravity = cachedGravity;
		body.interpolation = cachedInterpolationMode;
		IsHeld = false;
		OnDrop.Invoke( this );
	}

	void OnEnable()
	{

	}
}
