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
			action.Invoke();
			actionWithItem.Invoke( item );
		}
	}

	private void OnTriggerExit(Collider other)
	{
	}
}
