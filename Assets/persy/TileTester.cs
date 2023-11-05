using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTester : MonoBehaviour
{
    void OnEnable()
    {
        GetComponent< SceneryManager >().CreateNewTileAtStart();
    }

}
