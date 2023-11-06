using System.Collections;
using System.Collections.Generic;
using RigidFps;
using UnityEngine;

public class StationSign : MonoBehaviour
{
	static string[] names = {
		"Кукуево",
		"Академ",
		"Верхний Ларс"
	};

	static List< string > deck = new List< string >();


	public TMPro.TMP_Text label;

	public void OnEnable()
	{
		if( deck.Count == 0 )
		{
			for( int i = 0; i < names.Length; i++ )
			{
				deck.Add( names[ i ] );
			}

			for( int i = 0; i < deck.Count; i++ )
			{
				int j = Random.Range( i, deck.Count );
				var temp = deck[ j ];
				deck[ j ] = deck [ i ];
				deck[ i ] = temp;
			}
		}

		label.text = deck[ deck.Count - 1 ];
	}

	public void Detach()
	{
		gameObject.layer = label.gameObject.layer;
		transform.parent = null;
		GetComponent< Rigidbody >().isKinematic = false;
	}

	void OnCollisionEnter( Collision collision )
	{
		if( collision.gameObject.GetComponent< Player >() || collision.gameObject.GetComponent< Item >() )
		{
			Detach();
		}
	}
}
