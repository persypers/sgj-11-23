using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationPlanner : Fancy.MonoSingleton< StationPlanner >
{
	public float coalFuelOnBoard = 0;
	public float totalFuelOnBoard = 0;

	public List<Fancy.ObjectPool> stationGenerators;
	
	//public Fancy.ObjectPool stationGenerator;

	int stationCount = 0;
	public float PlanNextStationDistance()
	{
		float minDistance = Mathf.Lerp( 1500.0f, 800.0f, stationCount / 3.0f );
		float maxDistance = Mathf.Lerp( 1500.0f, 8000.0f, stationCount / 6.0f );
/*
		float coalBurnTime = ( Furnace.Instance.fuel + coalFuelOnBoard ) / Furnace.Instance.fuelBurnPerMinute * Furnace.perMinuteCoef;
		float fullBurnTime = ( Furnace.Instance.fuel + totalFuelOnBoard ) / Furnace.Instance.fuelBurnPerMinute * Furnace.perMinuteCoef;

		float maxCoalDistance = coalBurnTime * TrainScript.Instance.maxSpeed + World.Instance.EstimateBrakingDistance( TrainScript.Instance.maxSpeed, TrainScript.Instance.noFuelDeceleration );
		float maxFullDistance = fullBurnTime * TrainScript.Instance.maxSpeed + World.Instance.EstimateBrakingDistance( TrainScript.Instance.maxSpeed, TrainScript.Instance.noFuelDeceleration );

		float angry = Random.Range( 
			Mathf.Lerp( 0.3f, 0.8f, stationCount / 3.0f ),
			Mathf.Lerp( 0.8f, 1.3f, stationCount / 6.0f ) );

		maxDistance = Mathf.Min( maxDistance, maxFullDistance * angry );
*/

		return Random.Range( minDistance, maxDistance );
	}

	public GameObject SpawnStation( TileData tile )
	{
		int random = Random.Range(0, stationGenerators.Count);
		var go = stationGenerators[random].Get();
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
