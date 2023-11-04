using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
	public ItemTypes type;
	public float fuelValue;

	public UnityEvent OnPickUp;
}
