using System.Collections;
using System.Collections.Generic;
using Fancy;
using TMPro;
using UnityEngine;

public class Furnace : Fancy.MonoSingleton< Furnace >
{
	public FurnaceDoor door;
	public TMP_Text fuelLabel;
	public float itemBurnRate = 1.0f;
	public float fuel = 40.0f;
	public bool debugUnlimitedFuel = false;
	public float fuelBurnPerMinute = 20.0f;
	public const float perMinuteCoef = 1.0f / 60.0f;

	
	public float maxAudioAtFuel = 5.0f;
	public float audioBumpPerItem = 0.3f;

	public ObjectPool particles;
	public int particlesOnBurn = 3;
	// You should use "FuelRoarIntensity" parameter for currently burning items.
	// You can browse through them wia FMOD top menu in Unity. FMOD/Event browser/Global parameters

	public AudioSource[] burnSounds;

	List< Item > items = new List< Item >();

	public bool HasFuel => fuel > 0.0f;

	private void OnTriggerEnter(Collider other)
	{
		var item = other.GetComponent< Item >();
		if( item != null )
		{
			items.Add( item );
			item.isBurning = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		var item = other.GetComponent< Item >();
		if( item != null )
		{
			item.isBurning = false;
		}
	}

	int fuelInt = -1;

	void Update()
	{
		if( TrainScript.Instance.go && fuel > 0.0f )
		{
			fuel -= fuelBurnPerMinute * perMinuteCoef * Time.deltaTime;
			if( fuel <= 0.0f && !debugUnlimitedFuel )
			{
				TrainScript.Instance.hasFuel = false;
			}
		}
		int prevLabel = fuelInt;
		fuelInt = Mathf.Clamp( (int) fuel, 0, 999 );
		if( fuelInt != prevLabel )
			fuelLabel.text = fuelInt.ToString();


		float audioLevel = Mathf.Clamp01( fuel / maxAudioAtFuel );
		float maxBurn = 0.0f;

		for( int i = items.Count - 1; i >= 0; i-- )
		{
			var item = items[ i ];
			item.burn += ( item.isBurning ? itemBurnRate : -itemBurnRate ) * Time.deltaTime;
			if( item.burn > 1.0f )
			{
				items.Remove( item );
				Consume( item );
				audioLevel = Mathf.Clamp01( audioLevel + audioBumpPerItem );
			} 
			else if( item.burn <= 0.0f )
			{
				items.Remove( item );
				item.burn = 0.0f;
			}
			maxBurn = Mathf.Max( maxBurn, item.burn );
		}

		audioLevel *= 0.5f + 0.5f * Mathf.Clamp01( Mathf.InverseLerp( 0.4f, 0.0f, door.Openness ) );

		FMODUnity.RuntimeManager.StudioSystem.setParameterByName( "FuelLevel", audioLevel );
		FMODUnity.RuntimeManager.StudioSystem.setParameterByName( "FurnaceDoorOpen", door.Openness );
		FMODUnity.RuntimeManager.StudioSystem.setParameterByName( "FuelRoarIntensity", maxBurn );
	}

	private void Consume( Item item )
	{
		var col = item.GetComponent< Collider >();
		var bounds = col == null ? new Bounds( item.transform.position, new Vector3( 0.5f, 0.5f, 0.5f ) ) : item.GetComponent< Collider >().bounds;
		for( int i = 0; i < particlesOnBurn; i++ )
		{
			var go = particles.Get();
			Vector3 pos = new Vector3(
				Random.Range( bounds.min.x, bounds.max.x ),
				Random.Range( bounds.min.y, bounds.max.y ) - 0.6f,
				Random.Range( bounds.min.z, bounds.max.z ) );
			go.transform.position = pos;
			go.transform.localRotation = Quaternion.Euler(
				Random.Range( -15.0f, 15.0f ),
				Random.Range( -5.0f, 5.0f ),
				Random.Range( 0.0f, 360.0f ) );
			go.SetActive( true );
			go.GetComponentInChildren< ParticleSystem >().Play();
			StartCoroutine( DisableParticles( go ) );
		}
		
		var audio = burnSounds[ Random.Range( 0, burnSounds.Length ) ];
		audio.PlayOneShot( audio.clip );

		fuel += item.fuelValue;
		TrainScript.Instance.hasFuel = true;
		GameObject.Destroy( item.gameObject );
	}

	IEnumerator DisableParticles( GameObject bo )
	{
		yield return new WaitForSeconds( 2.0f );
		bo.SetActive( false );
	}
}
