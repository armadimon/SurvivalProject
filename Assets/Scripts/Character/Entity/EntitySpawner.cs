using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [Header("Statting")]
    public Terrain terrain;             // Terrain
    public int spawnCount;              // 한 번에 소환할 개수
    private int curEntityCount = 0;     // 현재 존재하는 엔티티 개수
    public int maxEntityCount;          // 최대 엔티티 개수
    public float terrainPadding = 5f;   // Terrain 가장자리 여유 공간

    [Header("Spawn")]
    public float timeRate;
    private int waveConut = 0;
    public int mediumWave = 2;  // 2번째 웨이브에서 중형 추가
    public int largeWave = 4;   // 4번째 웨이브에서 대형 추가
    private bool isNight = false;   // 밤인지 낮인지 여부

    [Header("Entity PrePrefab")]
    public GameObject[] smallEntityPrefab;
    public GameObject[] mediumEntityPrefab;
    public GameObject[] largeEntityPrefab;

    void Start()
    {
        DayNightCycle.OnNightStateChanged += SetNightState;
        StartCoroutine(SpawnWaves());
    }
    private void OnDestroy()
    {
        DayNightCycle.OnNightStateChanged -= SetNightState;

    }
    private void SetNightState(bool night)
    {
        isNight = night;
    }

    IEnumerator SpawnWaves()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeRate);

            if (!isNight && curEntityCount < maxEntityCount)
            {
                EntitySpawn();
                waveConut++;
                Debug.Log($"Wave: {waveConut}, Current Entities: {curEntityCount}/{maxEntityCount}");
            }
        }
    }

    void EntitySpawn()
    {
        if (isNight) return;
        int spawnLimit = Mathf.Min(spawnCount, maxEntityCount - curEntityCount);

        for (int i = 0; i < spawnLimit; i++)
        {
            Vector3 position = GetRandomPosition();
            if (position != Vector3.zero)
            {
                GameObject entity = Instantiate(GetCurrentWave(), position, Quaternion.identity);
                curEntityCount++;

                // 엔티티가 파괴될 때 개수를 줄이기 위해 이벤트 등록
                entity.AddComponent<EntityTracker>().SetSpawner(this);
            }
        }
    }

    Vector3 GetRandomPosition()
    {
        float terrainWidth = terrain.terrainData.size.x;
        float terrainLength = terrain.terrainData.size.z;

        float randomX = Random.Range(terrainPadding, terrainWidth - terrainPadding);
        float randomZ = Random.Range(terrainPadding, terrainLength - terrainPadding);

        Ray ray = new Ray(new Vector3(randomX, 1000f, randomZ), Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            return new Vector3(hit.point.x, hit.point.y, hit.point.z);
        }

        return Vector3.zero;
    }

    GameObject GetRandomPrefab(GameObject[] prefabs)
    {
        return prefabs[Random.Range(0, prefabs.Length)];
    }

    GameObject GetCurrentWave()
    {
        List<GameObject> animalPrefabs = new List<GameObject>();

        animalPrefabs.AddRange(smallEntityPrefab);
        if (waveConut >= mediumWave) animalPrefabs.AddRange(mediumEntityPrefab);
        if (waveConut >= largeWave) animalPrefabs.AddRange(largeEntityPrefab);

        return GetRandomPrefab(animalPrefabs.ToArray());
    }

    public void OnEntityDestroyed()
    {
        curEntityCount = Mathf.Max(0, curEntityCount - 1);
        Debug.Log($"Entity Destroyed. Current Count: {curEntityCount}/{maxEntityCount}");
    }
}

public class EntityTracker : MonoBehaviour
{
    private EntitySpawner spawner;

    public void SetSpawner(EntitySpawner spawner)
    {
        this.spawner = spawner;
    }

    void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.OnEntityDestroyed();
        }
    }
}
