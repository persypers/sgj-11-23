using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainVolume : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		var item = other.GetComponent< Item >();
		if( item != null )
			StationPlanner.Instance.ChangeFuelOnBoard( item.fuelValue, item.type == ItemTypes.Coal );
	}

	private void OnTriggerExit(Collider other)
	{
		var item = other.GetComponent< Item >();
		if( item != null )
			StationPlanner.Instance.ChangeFuelOnBoard( -item.fuelValue, item.type == ItemTypes.Coal );
	}
}
