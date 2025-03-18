using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    public static event System.Action OnBossSpawned;

    [Header("Statting")]
    public Terrain terrain;             // 스폰할 지형 (Terrain)
    public int spawnCount;              // 한 번에 소환할 개수
    private int curEntityCount = 0;     // 현재 존재하는 엔티티 개수
    public int maxEntityCount;          // 최대 엔티티 개수
    public float terrainPadding = 5f;   // Terrain 가장자리 여유 공간
    public LayerMask layerMask;         // 엔티티가 스폰될 레이어 마스크

    [Header("Spawn")]
    public float timeRate;              // 웨이브 간 간격 (초 단위)
    private int waveConut = 0;          // 현재 웨이브 번호
    public int mediumWave = 2;          // 2번째 웨이브에서 중형 엔티티 추가
    public int largeWave = 4;           // 4번째 웨이브에서 대형 엔티티 추가
    public int bossWave = 10;           // 10번째 웨이브에서 보스 엔티티 추가
    private bool isNight = false;       // 밤인지 낮인지 여부

    [Header("Entity PrePrefab")]
    public GameObject[] smallEntityPrefab;  // 소형 엔티티 프리팹 목록
    public GameObject[] mediumEntityPrefab; // 중형 엔티티 프리팹 목록
    public GameObject[] largeEntityPrefab;  // 대형 엔티티 프리팹 목록
    public GameObject[] bossEntityPrefab;   // 보스 엔티티 프리팹 목록

    void Start()
    {
        // 낮과 밤 상태 변화 이벤트 등록
        DayNightCycle.OnNightStateChanged += SetNightState;
        // 웨이브 스폰 코루틴 시작
        StartCoroutine(SpawnWaves());
    }

    private void OnDestroy()
    {
        // 이벤트 등록 해제 (메모리 누수 방지)
        DayNightCycle.OnNightStateChanged -= SetNightState;
    }

    private void SetNightState(bool night)
    {
        // 낮과 밤 상태를 업데이트
        isNight = night;
    }

    IEnumerator SpawnWaves()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeRate); // 지정된 시간 간격마다 웨이브 실행

            if (!isNight && curEntityCount < maxEntityCount) // 낮이고, 최대 개수를 초과하지 않은 경우
            {
                EntitySpawn(); // 엔티티 스폰 실행
            }
        }
    }

    void EntitySpawn()
    {
        if (isNight) return; // 밤에는 스폰하지 않음

        int spawnLimit = Mathf.Min(spawnCount, maxEntityCount - curEntityCount); // 스폰 가능 개수 계산

        for (int i = 0; i < spawnLimit; i++)
        {
            Vector3 position = GetRandomPosition(); // 랜덤 위치 가져오기
            if (position != Vector3.zero)
            {
                GameObject entity = Instantiate(GetCurrentWave(), position, Quaternion.identity); // 엔티티 생성
                curEntityCount++;


                // 엔티티가 파괴될 때 개수를 줄이기 위해 이벤트 등록
                entity.AddComponent<EntityTracker>().SetSpawner(this);
            }
        }

        if (waveConut > 0 && waveConut % bossWave == 0)
        {
            Vector3 bossPosition = GetRandomPosition(); // 랜덤 위치 가져오기
            if (bossPosition != Vector3.zero)
            {
                GameObject boss = Instantiate(GetRandomPrefab(bossEntityPrefab), bossPosition, Quaternion.identity); // 엔티티 생성
                curEntityCount++;
                NotificationManager.Instance.ShowNotification("보스가 나타났습니다. 조심하세요");


                // 엔티티가 파괴될 때 개수를 줄이기 위해 이벤트 등록
                boss.AddComponent<EntityTracker>().SetSpawner(this);
                OnBossSpawned?.Invoke();
            }
        }
        waveConut++; // 한 웨이브가 끝날 때 증가
        Debug.Log($"Wave {waveConut} Completed. Current Entities: {curEntityCount}/{maxEntityCount}");
    }

    Vector3 GetRandomPosition()
    {
        float terrainWidth = terrain.terrainData.size.x;
        float terrainLength = terrain.terrainData.size.z;

        // Terrain 내부에서 랜덤 위치 설정
        float randomX = Random.Range(terrainPadding, terrainWidth - terrainPadding);
        float randomZ = Random.Range(terrainPadding, terrainLength - terrainPadding);

        // 위에서 아래로 레이캐스트를 발사하여 Terrain 위의 위치 찾기
        Ray ray = new Ray(new Vector3(randomX, 1000f, randomZ), Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            return new Vector3(hit.point.x, hit.point.y, hit.point.z); // Terrain 위의 좌표 반환
        }

        return Vector3.zero; // 유효한 위치를 찾지 못한 경우 (스폰하지 않음)
    }

    GameObject GetRandomPrefab(GameObject[] prefabs)
    {
        return prefabs[Random.Range(0, prefabs.Length)]; // 프리팹 배열에서 랜덤 선택
    }

    GameObject GetCurrentWave()
    {
        List<GameObject> animalPrefabs = new List<GameObject>();

        // 웨이브에 따라 소환할 엔티티 종류 결정
        animalPrefabs.AddRange(smallEntityPrefab);
        if (waveConut >= mediumWave) animalPrefabs.AddRange(mediumEntityPrefab);
        if (waveConut >= largeWave) animalPrefabs.AddRange(largeEntityPrefab);

        return GetRandomPrefab(animalPrefabs.ToArray()); // 현재 웨이브에서 가능한 엔티티 중 랜덤 선택
    }

    public void OnEntityDestroyed()
    {
        curEntityCount = Mathf.Max(0, curEntityCount - 1); // 엔티티 개수 감소 (0 이하로 내려가지 않도록 보정)
        Debug.Log($"Entity Destroyed. Current Count: {curEntityCount}/{maxEntityCount}");
    }
}

public class EntityTracker : MonoBehaviour
{
    public static event System.Action OnBossDestroyed;
    private EntitySpawner spawner;

    public void SetSpawner(EntitySpawner spawner)
    {
        this.spawner = spawner;
    }

    void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.OnEntityDestroyed(); // 엔티티가 파괴될 때 스포너에게 알림
            if (gameObject.CompareTag("Boss"))
            {
                OnBossDestroyed?.Invoke();
                NotificationManager.Instance.ShowNotification("보스를 처치하였습니다!");
            }
        }
    }
}
