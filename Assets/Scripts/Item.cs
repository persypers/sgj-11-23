using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[SelectionBase]
public class Item : MonoBehaviour
{
	public ItemTypes type;
	public float fuelValue;
	public bool IsHeld { get; private set;}

	public AudioSource pickupAudio;
	public AudioSource dropAudio;
	public AudioSource impactAudio;
	public float minImpactAudioImpulse = 300.0f;
	public float maxImpactAudioImpulse = 2000.0f;
	public float impactAudioPitchVariance = 0.08f;

	public UnityEvent< Item > OnPickUp;
	public UnityEvent< Item > OnDrop;
	public UnityEvent< Item > OnItemJointBreak;

	new Rigidbody rigidbody;

	bool cachedGravity;
	RigidbodyInterpolation cachedInterpolationMode;
	CollisionDetectionMode cachedCollisionDecetectionMode;
	

	public void PickedUp()
	{
		Debug.Assert( !IsHeld );

		OnPickUp.Invoke( this );
		var body = GetComponent< Rigidbody >();
		cachedGravity = body.useGravity;
		cachedInterpolationMode = body.interpolation;
		body.useGravity = false;
		body.interpolation = RigidbodyInterpolation.Interpolate;
		body.collisionDetectionMode = CollisionDetectionMode.Continuous;

		if( pickupAudio != null )
		{
			pickupAudio.Play();
		}

		IsHeld = true;
	}

	public void Dropped()
	{
		Debug.Assert( IsHeld );
		var body = GetComponent< Rigidbody >();
		body.useGravity = cachedGravity;
		body.interpolation = cachedInterpolationMode;
		IsHeld = false;

		if( dropAudio != null )
		{
			dropAudio.Play();
		}

		OnDrop.Invoke( this );
	}

	IEnumerator RestoreCollisionModeAfterTimeout()
	{
		yield return new WaitForSeconds( 3.0f );
		if( !IsHeld )
			GetComponent< Rigidbody >().collisionDetectionMode = cachedCollisionDecetectionMode;
	}

	void OnJointBreak(float breakForce)
	{
		OnItemJointBreak.Invoke( this );
	}

	void OnCollisionEnter( Collision collision )
	{
		if( impactAudio != null )
		{
			//Debug.Log( collision.impulse.sqrMagnitude);
			float volume = Mathf.Clamp01( Mathf.InverseLerp( minImpactAudioImpulse, maxImpactAudioImpulse, collision.impulse.sqrMagnitude ) );
			impactAudio.pitch = 1.0f + Random.Range( -impactAudioPitchVariance, impactAudioPitchVariance);
			impactAudio.PlayOneShot( impactAudio.clip, volume );
		}
	}

	void OnEnable()
	{
		rigidbody = GetComponent< Rigidbody >();
	}
}
