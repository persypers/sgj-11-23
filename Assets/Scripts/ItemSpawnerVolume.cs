using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ItemSpawnerVolume : MonoBehaviour
{
	public BoxCollider volume;

	public bool randomRotationX = false;
	public bool randomRotationY = true;
	public bool randomRotationZ = false;

	const int MaxRotationIterations = 4;

	public GameObject Spawn( GameObject prefab )
	{
		GameObject go = Instantiate( prefab, null ); 
		var volumeCenterWorld = volume.transform.TransformPoint( volume.center );

		for( int i = 0; i < MaxRotationIterations; i++ )
		{
			var randomRot = Random.rotation.eulerAngles;
			Quaternion localRotation = Quaternion.Euler( 
				randomRotationX ? randomRot.x : 0.0f,
				randomRotationY ? randomRot.y : 0.0f,
				randomRotationZ ? randomRot.z : 0.0f );
			
			go.transform.rotation = localRotation;
			go.transform.localPosition = Vector3.zero;
			var col = go.GetComponent< Collider >();
			Vector3 posExtents = volume.size - col.bounds.size;
			if( posExtents.x >= 0.0f && posExtents.y >= 0.0f && posExtents.z >= 0.0f )
			{
				Vector3 randomMin = volume.center - volume.size * 0.5f - col.bounds.min;
				Vector3 randomMax = volume.center + volume.size * 0.5f - col.bounds.max;
				Vector3 localOffset = new Vector3(
					Random.Range( randomMin.x, randomMax.x ),
					Random.Range( randomMin.y, randomMax.y ),
					Random.Range( randomMin.z, randomMax.z )
				);
				Vector3 localPosition = go.transform.localPosition + localOffset;
				go.transform.position = volume.transform.InverseTransformPoint( localPosition );
				return go;
			}
		}
		GameObject.Destroy( go );
		// failed to place item so that it fits inside volume
		return null;
	}

	// Start is called before the first frame update
	void Start()
	{
		if( volume == null )
			volume = GetComponent< BoxCollider >();
	}
}
