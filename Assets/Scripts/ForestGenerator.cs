using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestGenerator : MonoBehaviour
{
    public GameObject treePrefab; 
    public int numberOfTrees;     
    public float x1;    // Coordinates (x1, x2, z1, z2) where trees will be scattered
    public float x2;
    public float z1;
    public float z2;

    public float minScale;
    public float maxScale;
    public float minTiltAngle;
    public float maxTiltAngle;

    void Start()
    {
        Debug.Log("Starting spawning trees.");
        SpawnTrees();
    }

    float CheckFloorHight(float x, float z)
    {
        Vector3 castPosition = new Vector3(x, 100.0f, z);
        Ray ray = new Ray(castPosition, Vector3.down);
        RaycastHit hit;
        float maxY = float.MinValue;
        int layerMask = 1 << 10;

        if (Physics.Raycast(ray, out hit, layerMask))
        {
            Debug.Log("Floor found.");
            maxY = hit.point.y;


            //Debug.Log("Floor found.");
            //if (hit.collider.gameObject == floorPrefab)
            //{
            //     Debug.Log("!!!.");
            //     if (hit.point.y > maxY)
            //     {
                    
            //     }
            // }
            // else
            // {
            //     Debug.Log("Found something.");
            //     maxY = 0.0f;
            // }
        }
        else
        {
            Debug.Log("No floor.");
            maxY = 0.0f;
        }

        return maxY;
    }


    void SpawnTrees()
    {
        for (int i = 0; i < numberOfTrees; i++)
        {
            float randomX = Random.Range(x1, x2);
            float randomZ = Random.Range(z1, z2);

            float floorY = CheckFloorHight(randomX, randomZ);

            Vector3 spawnPosition = new Vector3(randomX, floorY, randomZ);



            Quaternion spawnRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0); // Random rotation

            float randomTilt = Random.Range(minTiltAngle, maxTiltAngle);
            Quaternion tiltRotation = Quaternion.Euler(randomTilt, 0, 0);
            spawnRotation *= tiltRotation;

            // Generate a random scale value within the defined range
            float randomScale = Random.Range(minScale, maxScale);

            // Apply the random scale to the tree
            Vector3 scale = new Vector3(randomScale, randomScale, randomScale);

            // Instantiate the tree with the random position, rotation, and scale
            GameObject tree = Instantiate(treePrefab, spawnPosition, spawnRotation);
            tree.transform.localScale = scale; // Apply the random scale

            Debug.Log("Tree created.");

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
