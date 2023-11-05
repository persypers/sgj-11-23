using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabItemBehaviour : MonoBehaviour
{
	public ItemTypes type;
	public ConfigurableJoint itemJoint;
	public ItemTrigger itemTrigger;
	public bool setIgnoreGravity = false;
	protected bool cachedGravity;
	protected ConfigurableJoint currentJoint;
	protected Coroutine behaviour = null;
	void OnEnable()
	{
		itemTrigger.type = type;
		itemTrigger.actionWithItem.AddListener( GrabItem );
	}

	public void GrabItem( Item item )
	{
		if( !currentJoint )
		{
			currentJoint = Fancy.Helpers.CopyComponent( itemJoint, item.gameObject );
			item.OnPickUp.AddListener( DropItem );
			item.OnItemJointBreak.AddListener( DropItem );
			if( setIgnoreGravity )
			{
				var body = item.GetComponent< Rigidbody >();
				cachedGravity = body.useGravity;
				body.useGravity = false;
			}
			if( behaviour != null)
			{
				StopCoroutine( behaviour );
			}
			behaviour = StartCoroutine( DoBehaviour( item ) );
		}
	}

	public void DropItem( Item item )
	{
		item.OnPickUp.RemoveListener( DropItem );
		item.OnItemJointBreak.RemoveListener( DropItem );
		if( setIgnoreGravity )
		{
			var body = item.GetComponent< Rigidbody >();
			body.useGravity = cachedGravity;
		}
		if( currentJoint )
			GameObject.Destroy( currentJoint );
		currentJoint = null;
		if( behaviour != null )
		{
			StopCoroutine( behaviour );
			OnBehaviourCanceled( item );
		}
	}

	virtual public IEnumerator DoBehaviour( Item item )
	{
		while( ( item.transform.position - transform.position ).sqrMagnitude > 3.0f )
		{
			yield return null;
		}
		// do something
	}

	virtual public void OnBehaviourCanceled( Item item )
	{

	}
}