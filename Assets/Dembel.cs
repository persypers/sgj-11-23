using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dembel : GrabItemBehaviour
{
	public AudioSource guitarSongSource;
	public AudioSource guitarPickupSource;

	override public IEnumerator DoBehaviour( Item item )
	{
		yield return base.DoBehaviour( item );		// ждём пока предмет подлетит

		guitarPickupSource.Play();
		guitarSongSource.Play();
	}

	override public void OnBehaviourCanceled( Item item )
	{
		guitarSongSource.Stop();
	}
}
