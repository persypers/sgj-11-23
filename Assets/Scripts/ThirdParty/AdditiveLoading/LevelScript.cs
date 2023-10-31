using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fancy
{

public class LevelScript : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		Debug.Log(" Я РОДИЛСЯ! " + name);
	}

	public void ToLevelOne()
	{
		ScenesController.Instance.LoadScene( "Level1" );
	}
	public void ToLevelTwo()
	{
		ScenesController.Instance.LoadScene( "Level2" );
	}
	public void ChangeLevel( string sceneName )
	{
		ScenesController.Instance.LoadScene( sceneName );
	}
}

}