using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationPopulator : MonoBehaviour
{
	[System.Serializable]
	public class Entry
	{
		public ItemGenerator generator;
		public BoxCollider volume;
		public float chanceToSpawn = 1.0f;
		public int tries = 1;
		public bool onlyYRotation = false;
	}

	public List< Entry > itemGenerators;
	public void Populate( TileData tile )
	{
		for( int i = 0; i < itemGenerators.Count; i++ )
		{
			var entry = itemGenerators[ i ];
			for( int j = 0; j < entry.tries; j++ )
			{
				if( (entry.chanceToSpawn < 1.0f && Random.Range( 0.0f, 1.0f ) < entry.chanceToSpawn) || entry.chanceToSpawn >= 1.0f )
				{
					var go = Spawn( entry.generator.Get(), entry.volume, entry.onlyYRotation );
				}
			}
		}
	}


	const int MaxRotationIterations = 3;
	public static GameObject Spawn( GameObject prefab, BoxCollider volume, bool onlyYRotation )
	{
		GameObject go = Instantiate( prefab, null ); 
		var volumeCenterWorld = volume.transform.TransformPoint( volume.center );

		for( int i = 0; i < MaxRotationIterations; i++ )
		{
			var randomRot = Random.rotation.eulerAngles;
			Quaternion localRotation = Quaternion.Euler( 
				!onlyYRotation ? randomRot.x : 0.0f,
				randomRot.y,
				!onlyYRotation ? randomRot.z : 0.0f );
			
			var col = go.GetComponent< Collider >();
			Debug.Log( "unrotated: " + col.bounds.min + " : " + col.bounds.max );

			Vector3[] boundsVertices = new Vector3[8];
			var min = col.bounds.min;
			var max = col.bounds.max;
			boundsVertices[0] = new Vector3( min.x, min.y, min.z );
			boundsVertices[1] = new Vector3( min.x, max.y, min.z );
			boundsVertices[2] = new Vector3( min.x, max.y, max.z );
			boundsVertices[3] = new Vector3( min.x, min.y, max.z );
			boundsVertices[4] = new Vector3( max.x, min.y, min.z );
			boundsVertices[5] = new Vector3( max.x, max.y, min.z );
			boundsVertices[6] = new Vector3( max.x, max.y, max.z );
			boundsVertices[7] = new Vector3( max.x, min.y, max.z );
			var tr = Matrix4x4.Rotate( localRotation ) * go.transform.worldToLocalMatrix;
			var volumeSpaceBounds = GeometryUtility.CalculateBounds( boundsVertices, tr );
			
			//go.transform.rotation = localRotation;
			go.transform.localPosition = Vector3.zero;
			Vector3 posExtents = volume.size - volumeSpaceBounds.size;
			Debug.Log( "ROTA: " + volumeSpaceBounds.min + " : " + volumeSpaceBounds.max );
			if( posExtents.x >= 0.0f && posExtents.y >= 0.0f && posExtents.z >= 0.0f )
			{
				Vector3 randomMin = volume.center - volume.size * 0.5f - volumeSpaceBounds.min;
				Vector3 randomMax = volume.center + volume.size * 0.5f - volumeSpaceBounds.max;
				Debug.Log( randomMin + " : " + randomMax );
				Vector3 localOffset = new Vector3(
					Random.Range( randomMin.x, randomMax.x ),
					Random.Range( randomMin.y, randomMax.y ),
					Random.Range( randomMin.z, randomMax.z )
				);
				Debug.Log( "offset: " + localOffset.ToString());
				Vector3 localPosition = go.transform.localPosition + localOffset;
				go.transform.position = volume.transform.TransformPoint( localOffset );
				go.transform.rotation = Quaternion.Inverse( volume.transform.rotation ) * localRotation;
				//go.transform.position = volume.transform.TransformPoint( Vector3.zero );
				Debug.Log( "position: " + go.transform.position);
				return go;
			}
		}
		GameObject.Destroy( go );
		// failed to place item so that it fits inside volume
		return null;
	}
}
