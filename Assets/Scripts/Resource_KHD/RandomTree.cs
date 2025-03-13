using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RandomTree : MonoBehaviour
{
    public Terrain terrain;  // Terrain 객체
    public GameObject treePrefab; // 나무 프리팹
    public int treeCount = 50; // 생성할 나무 개수
    public float terrainPadding = 5f; // Terrain 가장자리 여유 공간

    void Start()
    {  
        SpawnTrees();
    }

    [ContextMenu("SpawnTree")] // 함수를 임의로 실행할 수 있게 해주는 기능
    void SpawnTrees()
    {
        for (int i = 0; i < treeCount; i++)
        {
            Vector3 position = GetRandomPosition();
            if (position != Vector3.zero)
            {
                Instantiate(treePrefab, position, Quaternion.identity); 
            }
        }
    }

    Vector3 GetRandomPosition()
    {
        // Terrain의 영역을 기준으로 랜덤한 x, z 좌표 선택
        float terrainWidth = terrain.terrainData.size.x;
        float terrainLength = terrain.terrainData.size.z;

        float randomX = Random.Range(terrainPadding, terrainWidth - terrainPadding);
        float randomZ = Random.Range(terrainPadding, terrainLength - terrainPadding);

        // Ray를 쏴서 Terrain의 높이를 찾음
        Ray ray = new Ray(new Vector3(randomX, 1000f, randomZ), Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // Terrain 위에 나무 배치
            return new Vector3(hit.point.x, hit.point.y, hit.point.z);
        }

        return Vector3.zero;
    }
}
