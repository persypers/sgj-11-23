using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RigidFps;
public class KillBoxScript : MonoBehaviour
{
    public TrainScript train;
    public GameObject spawnPoint;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == player)
        {
            float x_move = spawnPoint.transform.position.x - player.transform.position.x;
            float y_move = spawnPoint.transform.position.y - player.transform.position.y;
            float z_move = spawnPoint.transform.position.z - player.transform.position.z;

            Debug.Log("x_move: " + x_move + " y_move: " + y_move + " z_move: " + z_move);
            Vector3 move = new Vector3(x_move, y_move, z_move);
            player.GetComponent<Player>().Warp(move);
            player.GetComponent<Rigidbody>().position = spawnPoint.transform.position;
            player.GetComponent<Rigidbody>().velocity = new Vector3(0,0,train.currentSpeed);
        }
        else
        {
            GameObject.Destroy(collider.gameObject);
        }
    }
}
