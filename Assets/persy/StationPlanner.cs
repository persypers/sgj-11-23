using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationPlanner : Fancy.MonoSingleton< StationPlanner >
{
	public float coalFuelOnBoard = 0;
	public float totalFuelOnBoard = 0;
	public Fancy.ObjectPool stationGenerator;

	public GameObject SpawnStation( TileData tile )
	{
		var go = stationGenerator.Get();
		go.transform.position = tile.transform.position;
		tile.Add( go );

		// populate station

		go.SetActive( true );

		go.GetComponent< StationPopulator >().Populate( tile );

		return go;
	}

	public void ChangeFuelOnBoard( float amount, bool isCoal )
	{
		if( isCoal )
		{
			coalFuelOnBoard += amount;
		}
		totalFuelOnBoard += amount;
	}
}
