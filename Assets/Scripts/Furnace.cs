using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

public class Furnace : Fancy.MonoSingleton< Furnace >
{
	public FurnaceDoor door;
	public float itemBurnRate = 1.0f;
	public float fuel = 10.0f;
	public float fuelBurnPerSecond = 0.02f;

	
	public float maxAudioAtFuel = 5.0f;
	public float audioBumpPerItem = 0.3f;

		// You should use "FuelRoarIntensity" parameter for currently burning items.
		// You can browse through them wia FMOD top menu in Unity. FMOD/Event browser/Global parameters

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
		fuel += item.fuelValue;
		GameObject.Destroy( item.gameObject );
	}
}
