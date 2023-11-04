using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemTrigger : MonoBehaviour
{
	public ItemTypes type;
	public UnityEvent action;
	public UnityEvent< Item > actionWithItem;
	private void OnTriggerEnter(Collider other)
	{
		var item = other.GetComponentInParent< Item >();
		Debug.Assert( item != null, "GameObject has layer 'Item' but no item component: " + other.gameObject.name );

		if( item.type == type )
		{
			if( item.IsHeld )
			{
				item.OnDrop.AddListener( Trigger );
			}
			else {
				Trigger( item );
			}
		}
	}

	private void Trigger( Item item )
	{
		action.Invoke();
		actionWithItem.Invoke( item );
	}

	private void OnTriggerExit(Collider other)
	{
		var item = other.GetComponentInParent< Item >();
		Debug.Assert( item != null, "GameObject has layer 'Item' but no item component: " + other.gameObject.name );

		if( item.type == type )
		{
			item.OnDrop.RemoveListener( Trigger );
		}
	}
}
