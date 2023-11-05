using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData : MonoBehaviour
{
	public static Vector2 tileSize = new Vector2( 60.0f, 60.0f );
	public static float Height = 200.0f;
	// понадобится позже
	Vector2Int tileCoordinates;
	// просто включать и выключать объекты быстрее, чем менять их родителя
	List< GameObject > children = new List<GameObject>();

	public void Add( GameObject child )
	{
		children.Add( child );
	}

	void OnDisable()
	{
		for( var i = 0; i < children.Count; i++ )
		{
			children[i].SetActive( false );
		}
		children.Clear();
	}
}
