using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dembel : MonoBehaviour
{
	public ConfigurableJoint guitarJoint;
	public ItemTrigger guitarTrigger;
	public AudioSource guitarSongSource;
	public AudioSource guitarPickupSource;
	ConfigurableJoint joint;
	Coroutine guitarCoro;
	void Start()
	{
		guitarTrigger.actionWithItem.AddListener( PlayGuitar );
	}

	public void PlayGuitar( Item item )
	{
		if( !joint )
		{
			joint = Fancy.Helpers.CopyComponent( guitarJoint, item.gameObject );
			item.OnPickUp.AddListener( DropGuitar );
			if( guitarCoro != null)
				StopCoroutine( guitarCoro );
			guitarCoro = StartCoroutine( GuitarCoroutine( item ) );
		}
	}

	void DropGuitar( Item guitar )
	{
		if( guitarCoro != null)
			StopCoroutine( guitarCoro );
		guitarSongSource.Stop();
		joint.GetComponent< Item >().OnPickUp.RemoveListener( DropGuitar );
		GameObject.Destroy( joint );
	}

	IEnumerator GuitarCoroutine( Item guitar )
	{
		while( ( guitar.transform.position - transform.position ).sqrMagnitude > 3.0f )
		{
			yield return null;
		}
		guitarPickupSource.Play();
		guitarSongSource.Play();
	}

	public void TestFunction()
	{
		Debug.Log( "PATEFON PRINESLI");
	}
}
