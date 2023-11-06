using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Random Prefab", menuName = "Custom/Random Prefab")]
public class RandomPrefab : ItemGenerator
{
	[System.Serializable]
	public struct Entry
	{
		public Object prefabOrGenerator;
		public float weight;
		public GameObject Get()
		{
			ItemGenerator asGeneratpr = prefabOrGenerator as ItemGenerator;
			if( asGeneratpr != null )
				return asGeneratpr.Get();
			if( prefabOrGenerator is GameObject )
				return prefabOrGenerator as GameObject;
			else
				return null;
		}
	}

	public List< Entry > entries;

	override public GameObject Get()
	{
		float totalWeight = 0.0f;
		for( int i = 0; i < entries.Count; i++ )
		{
			totalWeight += entries[ i ].weight;
		}

		float value = Random.Range( 0.0f, totalWeight );
		for( int i = 0; i < entries.Count; i++ )
		{
			value -= entries[ i ].weight;
			if( value <= 0.0f )
			{
				return entries[ i ].Get();
			}
		}
		return entries[ entries.Count - 1 ].Get();
	}
}
