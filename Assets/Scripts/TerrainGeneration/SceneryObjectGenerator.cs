using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fancy;

//Ответственен за расстановку на тайлы одинаковых объектов(из одного префаба)
public class SceneryObjectGenerator : MonoBehaviour
{

//Пул, чтобы оттуда доставать объекты
    public ObjectPool objectPool;

//nearest_x_dist и furtherst_x_dist определяют то, насколько далеко/близко к дороге может быть объект (типа дома далеко, трава близко)
    public float nearest_x_dist;

    public float furtherst_x_dist;

//Чем больше вес - тем чаще будет спавниться объект
    public float weight;

//Допустимый Scale и тилт объекта (врещение у всех может быть 360)
    public float minScale;
    public float maxScale;
    public float minTiltAngle;
    public float maxTiltAngle;


    //Чекаем, насколько высоко надо заспавнить объект, чтобы он был не под полом.
    //!! Проверки на то, что мы не спавним объекты внутри других объектов нет.
    float GetFloorHight(float x, float z)
    {
        Vector3 castPosition = new Vector3(x, 100.0f, z);
        Ray ray = new Ray(castPosition, Vector3.down);
        RaycastHit hit;
        float maxY = float.MinValue;
        int layerMask = 1 << 10;

        if (Physics.Raycast(ray, out hit,  Mathf.Infinity, layerMask))
        {
            maxY = hit.point.y;

        }
        else
        {
            maxY = 0.0f;
        }

        return maxY;
    }

//Спавним объект в случайном месте над объектом земли groundObject
    public GameObject SpawnObject(GameObject groundObject)
    {

        //Bounds bounds = groundObject.GetComponent<Collider>().bounds;
        float z1 = groundObject.transform.position.z - 100;
        float z2 = groundObject.transform.position.z + 100;
        //Debug.Log("z1 = " + z1 + " z2 = " + z2);
        float road_center_x = groundObject.transform.position.x;

        //Get coordinates. Todo do it better
        float coorZ = Random.Range(z1, z2);
        float randomX_dist = Random.Range(nearest_x_dist, furtherst_x_dist);
        float coorX = 0;


        //Определяем, будет справа или слева от дороги
        if (Random.Range(0, 2) == 0)
        {
            coorX = road_center_x - randomX_dist;
        }
        else
        {
            coorX = road_center_x + randomX_dist;
        }
        float floorY = GetFloorHight(coorX, coorZ);
        Vector3 spawnPosition = new Vector3(coorX, floorY, coorZ);

        //Get rotation
        Quaternion spawnRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0); // Random rotation

        //Get tilt
        float randomTilt = Random.Range(minTiltAngle, maxTiltAngle);
        Quaternion tiltRotation = Quaternion.Euler(randomTilt, 0, 0);
        spawnRotation *= tiltRotation;

        //Get scale
        float randomScale = Random.Range(minScale, maxScale);
        Vector3 scale = new Vector3(randomScale, randomScale, randomScale);
       
        //Get scenery object
        GameObject sceneryObject = objectPool.Get();
        sceneryObject.SetActive(true);

        //apply transformations
        sceneryObject.transform.position = spawnPosition;
        sceneryObject.transform.rotation = spawnRotation;
        sceneryObject.transform.localScale = scale;
        return sceneryObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
