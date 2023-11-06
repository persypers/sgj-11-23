using System.Collections;
using System.Collections.Generic;
using Fancy;
using UnityEngine;

public class Furnace : Fancy.MonoSingleton< Furnace >
{
	public FurnaceDoor door;
	public float itemBurnRate = 1.0f;
	public float fuel = 10.0f;
	public float fuelBurnPerSecond = 0.02f;

	
	public float maxAudioAtFuel = 5.0f;
	public float audioBumpPerItem = 0.3f;

	public ObjectPool particles;
	public int particlesOnBurn = 3;

	List< Item > items = new List< Item >();

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

	void Update()
	{
		float audioLevel = Mathf.Clamp01( fuel / maxAudioAtFuel );

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
		}

		FMODUnity.RuntimeManager.StudioSystem.setParameterByName( "FuelLevel", audioLevel );
		FMODUnity.RuntimeManager.StudioSystem.setParameterByName( "FurnaceDoorOpen", door.Openness );
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
				Random.Range( bounds.min.y, bounds.max.y ) - 0.4f,
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
		fuel += item.fuelValue;
		GameObject.Destroy( item.gameObject );
	}

	IEnumerator DisableParticles( GameObject bo )
	{
		yield return new WaitForSeconds( 2.0f );
		bo.SetActive( false );
	}
}
