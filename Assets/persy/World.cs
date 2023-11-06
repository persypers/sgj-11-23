using System.Collections;
using System.Collections.Generic;
using RigidFps;
using UnityEngine;

public class World : Fancy.MonoSingleton< World >
{
	public float firstTileShift = -100.0f;
	public float WarpTriggerDistance = 500.0f;
	public int TileSize = 200;
	public int WarpTiles = 3;
	public int ForwardViewDistanceInTiles = 3;
	public int BackViewDistanceInTiles = 3;
	public float physicsSearchHeight;
	public LayerMask warpLayers;
	public SceneryManager sceneryManager;
	
	public int relativeTileIndex = 1;
	public int absoluteTileIndex = 1;

	GameObject nextStation = null;
	int nextStationTileIndex = -1;

	public void WarpPhysics()
	{
		{
			Bounds bounds = sceneryManager.GroundTiles[ 0 ].GetComponent< Collider >().bounds;
			var size = bounds.size;
			size.y = physicsSearchHeight;
			bounds.size = size;

			for( int i = 0; i < sceneryManager.GroundTiles.Count; i++ )
			{
				bounds.Encapsulate( sceneryManager.GroundTiles[ i ].GetComponent< Collider >().bounds );
			}

			var objects = Physics.OverlapBox( bounds.center, bounds.extents, Quaternion.identity, warpLayers );

			HashSet< Rigidbody > bodies = new HashSet<Rigidbody>();

			Vector3 move = new Vector3( 0.0f, 0.0f, -TileSize * WarpTiles );

			for( int i = 0; i < objects.Length; i++ )
			{
				var body = objects[i].attachedRigidbody;
				if( body == null )
					continue;
				if( !bodies.Contains( body ) )
				{
					bodies.Add( body );
					body.position += move;
					body.transform.position += move;
				}
			}


			Player.Instance.Warp( move );
			sceneryManager.Warp(move);

			Physics.autoSimulation = false;
			Physics.Simulate( Time.fixedDeltaTime );
			Physics.autoSimulation = true;

		}
	}

	void FixedUpdate()
	{
		// fixed update is for physical-based stuff
		if( TrainScript.Instance.transform.position.z > WarpTriggerDistance )
		{
			Debug.Log("Warping");
			relativeTileIndex -= WarpTiles;
			WarpPhysics();
		}
	}

	void Update()
	{
		// creating and deleting objects is better to be done in Update
		if( TrainScript.Instance.transform.position.z > TileSize * (relativeTileIndex+1) + firstTileShift)
		{
			relativeTileIndex+= 1;
			absoluteTileIndex += 1;
			sceneryManager.DeleteTilesUpToIndex( absoluteTileIndex - BackViewDistanceInTiles );
			var tile = sceneryManager.CreateNewTileAtStart();
			OnTileCreated( tile );
		}
	}

	void OnTileCreated( TileData tile )
	{
		Debug.Log( "Tile entered: " + absoluteTileIndex );
		var index = absoluteTileIndex;
		//var createdTileIndex = index + 

		if( nextStationTileIndex == -1 || nextStation == null )
		{
			// первая рандомная станция будет планироваться при первой смене тайла - когда мы уже поехали
			nextStationTileIndex = CalculateNextStationTileIndex();
		}

		if( tile.index == nextStationTileIndex )
		{
			nextStation = StationPlanner.Instance.SpawnStation( tile );
		}
	}

	float EstimateBrakingDistance( float currentSpeed, float deceleration )
	{
		float time = currentSpeed / deceleration;
		return currentSpeed * time - deceleration * time * time * 0.5f;
	}

	int CalculateNextStationTileIndex()
	{
		// potom
		return absoluteTileIndex + 5;
	}
}
