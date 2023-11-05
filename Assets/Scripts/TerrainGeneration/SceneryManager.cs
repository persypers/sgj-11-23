using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fancy;


public class SceneryManager : MonoBehaviour
{

    public List<SceneryObjectGenerator> ObjectGeneratorsList;
    public List<ObjectPool> groundPools;
    
//Ренж того, насколько загруженным/пустым может быть тайл
    public float min_weight;
    public float max_weight;

    public GameObject startTile;
    public bool playTestCoro = true;
    public List<GameObject> GroundTiles;

//Для того, чтобы определять, какие объекты кидаем
    private Dictionary<(float, float), SceneryObjectGenerator> randomRanges;
    private Dictionary<SceneryObjectGenerator, float> objectWeights;


    private float max_random_range;

    void ScatterSceneryObjects(GameObject groundObject, TileData tileData)
    {
        //Генерим случайный вес тайла, чтобы понять, сколько всего по нему рассыпать
        float curr_weight = Random.Range(min_weight, max_weight);
        Transform groundTransform = groundObject.transform;
        
        //Пока есть свободный вес, рандомим, какой айтем закинуть
        while (curr_weight > 0)
        {
            float rand_range = Random.Range(0, max_random_range);
            foreach (var kvp in randomRanges)
            {
                if ((kvp.Key.Item1 < rand_range) && (rand_range < kvp.Key.Item2))
                {
                    //Генерим объект и привязываем его к земле
                    GameObject sceneryObject = kvp.Value.SpawnObject(groundObject);
                    tileData.Add( sceneryObject );
                    curr_weight -= kvp.Value.weight;

                    break;
                }
            }
        }
    }

    //Создание только ландшафта исходя из списка пулов, из которых мы можем рандомного его потянуть
    public GameObject GenerateGroundTile(float x, float y, float z)
    {
        Vector3 spawnPosition = new Vector3(x, y, z);
        int ground_num = Random.Range(0, groundPools.Count);
        GameObject groundObject = groundPools[ground_num].Get();

        int LayerTerrain = LayerMask.NameToLayer("Terrain");
        groundObject.transform.position = spawnPosition;
        groundObject.layer = LayerTerrain;

        groundObject.SetActive(true);
        return groundObject;
    }

    //Добавить тайл в начало дороги
    public void CreateNewTileAtStart()
    {
        var firstTile = GroundTiles[0];

        float x = firstTile.transform.position.x;
        float y = firstTile.transform.position.y;
        float z = firstTile.transform.position.z + World.Instance.TileSize;

        GroundTiles.Insert(0, CreateTileAt( x, y, z ) );
    }

    GameObject CreateTileAt( float x, float y, float z )
    {
        GameObject groundObject = GenerateGroundTile(x, y, z);
        var tileData = groundObject.GetComponent<TileData>();
        ScatterSceneryObjects(groundObject, tileData);
        return groundObject;
    }

    void GenerateNewTiles(int n)
    {
        for (int i = 0; i < n; i++)
        {
            CreateNewTileAtStart();
        } 
    }

    //Деактивация тайла и всех объектов, которые были на нем рассыпаны
    public void DeleteLastTile()
    {
        GameObject tile = GroundTiles[GroundTiles.Count - 1];
        if( tile == startTile )
            GameObject.Destroy( tile );
        else
            tile.SetActive(false);
        GroundTiles.RemoveAt(GroundTiles.Count - 1);
    }

    void DeleteLastTiles(int n)
    {
        for (int i = 0; i < n; i++)
        {
            DeleteLastTile();
        } 
    }

    IEnumerator test()
    {
        CreateNewTileAtStart();
        CreateNewTileAtStart();
        yield return new WaitForSecondsRealtime(4);
        CreateNewTileAtStart();
        yield return new WaitForSecondsRealtime(4);
        DeleteLastTile();
        yield return new WaitForSecondsRealtime(4);
        CreateNewTileAtStart();
        yield return new WaitForSecondsRealtime(4);
        DeleteLastTile();
    }

    void Start()
    {
        GroundTiles = new List<GameObject>();
        randomRanges = new Dictionary<(float, float), SceneryObjectGenerator>();
        objectWeights = new Dictionary<SceneryObjectGenerator, float>();
        
        //Сетап рандомных отрезков для того, чтобы чаще спавнить айтемы с маленькими весами
        float curr_max_rand = 0;
        for (int i = 0; i < ObjectGeneratorsList.Count; i++)
        {
            float old_max_rand = curr_max_rand;
            curr_max_rand = old_max_rand + 1/ObjectGeneratorsList[i].weight;
            randomRanges.Add((old_max_rand, curr_max_rand), ObjectGeneratorsList[i]);
        }
        max_random_range = curr_max_rand;

        if( startTile )
        {
            GroundTiles.Add( startTile );
            for( int i = 0; i < World.Instance.WarpTiles; i++ )
            {
                CreateNewTileAtStart();
            }
        }
        else
            GroundTiles.Add( CreateTileAt( 0, 0, 0 ) );
        
        if( playTestCoro )
            StartCoroutine((test()));
    }

    public void Warp( Vector3 move )
    {
        for( int i = 0; i < GroundTiles.Count; i++ )
        {
            GroundTiles[i].GetComponent< TileData >().Warp( move );
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
