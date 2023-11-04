using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Dembel : MonoBehaviour
{
	public ConfigurableJoint guitarJoint;
	public ItemTrigger guitarTrigger;
	ConfigurableJoint joint;
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
		}
	}

	void DropGuitar( Item guitar )
	{
		joint.GetComponent< Item >().OnPickUp.RemoveListener( DropGuitar );
		GameObject.Destroy( joint );
	}
}
