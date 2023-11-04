using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VinylPlayer : MonoBehaviour
{
	public ConfigurableJoint jointPrefab;
	public ConfigurableJoint joint;
	public ItemTrigger trigger;
	public AudioSource source;
	Coroutine delay;

	void Start()
	{
		trigger.actionWithItem.AddListener( PlayVinyl );
	}

	void PlayVinyl( Item item )
	{
		if( !joint )
		{
			Debug.Log( "PLAY VINYL" );
			joint = Fancy.Helpers.CopyComponent( jointPrefab, item.gameObject );
			item.OnPickUp.AddListener( DropVinyl );
			item.OnItemJointBreak.AddListener( DropVinyl );
			if( delay != null )
			{
				StopCoroutine( delay );
				delay = null;
			}
			delay = StartCoroutine( StartAudio( 1.0f ) );
		}

	}

	void DropVinyl( Item item )
	{
		Debug.Log( "DROP VINYL" );
		item.OnPickUp.RemoveListener( DropVinyl );
		item.OnItemJointBreak.RemoveListener( DropVinyl );
		if( delay != null )
		{
			StopCoroutine( delay );
			delay = null;
		}
		source.Stop();
		if( joint )
			GameObject.Destroy( joint );
	}

	public IEnumerator StartAudio( float delay )
	{
		yield return new WaitForSeconds( delay );
		var vinyl = joint.GetComponent< Vinyl >();
		source.loop = vinyl.loop;
		source.clip = vinyl.clip;
		source.Play();
	}
}
