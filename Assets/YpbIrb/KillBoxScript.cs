using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBoxScript : MonoBehaviour
{
    public TrainScript train;
    public GameObject cabin;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == player)
        {
            player.transform.position = cabin.transform.position;
            player.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, train.currentSpeed);
        }
        else
        {
            GameObject.Destroy(collider.gameObject);
        }
    }
}
