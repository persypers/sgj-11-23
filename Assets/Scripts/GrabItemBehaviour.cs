using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabItemBehaviour : MonoBehaviour
{
	public ItemTypes type;
	public ConfigurableJoint itemJoint;
	public Collider itemTriggerCollider;
	ItemTrigger itemTrigger = null;
	public bool setIgnoreGravity = false;
	public bool nonUnique = false;
	public bool allowTriggerOnPlayerHeldItem = false;
	protected bool cachedGravity;
	protected ConfigurableJoint currentJoint;
	protected Coroutine behaviour = null;

	public bool IsBusy => behaviour != null;

	BehaviourBusiness business = null;
	public class BehaviourBusiness : MonoBehaviour
	{
		HashSet< GrabItemBehaviour > activeBehaviours = new HashSet< GrabItemBehaviour >();
		GrabItemBehaviour activeUnique = null;
		List< QueueEntry > queue = new List< QueueEntry >();
		[System.Serializable]
		struct QueueEntry {
			public GrabItemBehaviour behaviour;
			public Item item;
		}
		public bool IsBusy => activeBehaviours.Count > 0;
		public void OnBehaviourEnd( GrabItemBehaviour behaviour )
		{
			activeBehaviours.Remove( behaviour );
			if( activeUnique == behaviour )
				activeUnique = null;
			if( !IsBusy && queue.Count > 0 )
			{
				var toStart = queue[ 0 ];
				queue.RemoveAt( 0 );
				toStart.behaviour.GrabItem( toStart.item );
			}
		}
		public void Queue( GrabItemBehaviour behaviour, Item item )
		{
			if( !behaviour.nonUnique && !IsBusy )
			{
				activeUnique = behaviour;
				activeBehaviours.Add( behaviour );
				behaviour.GrabItem( item );
				return;
			}
			
			if( behaviour.nonUnique && activeUnique == null )
			{
				activeBehaviours.Add( behaviour );
				behaviour.GrabItem( item );
				return;
			}

			var entry = new QueueEntry();
			entry.behaviour = behaviour;
			entry.item = item;
			queue.Add( entry );
			return;
		}

		public void Dequeue( GrabItemBehaviour behaviour, Item item )
		{
			for( int i = queue.Count - 1; i >= 0; i-- )
			{
				if( queue[ i ].behaviour == behaviour && queue[ i ].item == item )
					queue.RemoveAt( i );
			}
		}
	}
	

	void OnEnable()
	{
		business = Fancy.Helpers.GetOrAddComponent< BehaviourBusiness >( gameObject );
		itemTrigger = itemTriggerCollider.gameObject.AddComponent< ItemTrigger >();
		itemTrigger.type = type;
		itemTrigger.actionWithItem.AddListener( ( Item item ) => { business.Queue( this, item ); } );
		itemTrigger.itemExit.AddListener( DropItem );
		itemTrigger.allowHeldItems = allowTriggerOnPlayerHeldItem;
	}

	void OnDisable()
	{
		GameObject.Destroy( itemTrigger );
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
		business.Dequeue( this, item );
		business.OnBehaviourEnd( this );
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