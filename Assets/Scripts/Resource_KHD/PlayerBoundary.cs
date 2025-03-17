using UnityEngine;

public class PlayerBoundary : MonoBehaviour
{
    private Terrain terrain;
    private Vector3 terrainMin;
    private Vector3 terrainMax;

    void Start()
    {
        // Terrain 가져오기
        terrain = Terrain.activeTerrain;

        // Terrain의 최소, 최대 좌표 계산
        terrainMin = terrain.transform.position;
        terrainMax = terrainMin + terrain.terrainData.size;
    }

    void Update()
    {
        // 현재 플레이어 위치 가져오기
        Vector3 playerPos = transform.position;

        // X, Z 좌표를 Terrain 범위 내로 제한
        playerPos.x = Mathf.Clamp(playerPos.x, terrainMin.x, terrainMax.x);
        playerPos.z = Mathf.Clamp(playerPos.z, terrainMin.z, terrainMax.z);

        // 플레이어 위치 적용
        transform.position = playerPos;
    }
}
