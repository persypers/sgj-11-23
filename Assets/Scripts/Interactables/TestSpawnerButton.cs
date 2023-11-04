using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawnerButton : MonoBehaviour, Interactable
{
	public GameObject itemToSpawn;
	public bool DoAction( GameObject actor)
	{
		Debug.Log( "Test interact " + gameObject.name );
		return true;
	}
}
