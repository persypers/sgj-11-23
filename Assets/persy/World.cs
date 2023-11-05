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
	public float physicsSearchHeight;
	public LayerMask warpLayers;
	public SceneryManager sceneryManager;
	
	public int onTile;

	public void WarpToNextTile()
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
		}
	}

	void FixedUpdate()
	{
		if( TrainScript.Instance.transform.position.z > WarpTriggerDistance )
		{
			onTile -= WarpTiles;
			WarpToNextTile();
		}

		if( TrainScript.Instance.transform.position.z > TileSize * (onTile+2) + firstTileShift)
		{
			onTile+= 1;
			sceneryManager.DeleteLastTile();
			sceneryManager.CreateNewTileAtStart();
			Debug.Log("Adding Tile");
		}

	}
}
