//using UnityEngine;
//using UnityEditor;

//public class MeshCombiner : MonoBehaviour
//{
//    public GameObject[] meshesToCombine; // 결합할 메쉬 오브젝트들

//    // 이 함수는 인스펙터에서 버튼 클릭 시 호출됨
//    public void CombineMeshesAndSavePrefab()
//    {
//        if (meshesToCombine == null || meshesToCombine.Length == 0)
//        {
//            Debug.LogWarning("결합할 메쉬가 없습니다!");
//            return;
//        }

//        CombineInstance[] combine = new CombineInstance[meshesToCombine.Length];
//        int i = 0;

//        // 각 메쉬 오브젝트를 CombineInstance 배열에 추가
//        foreach (var meshObject in meshesToCombine)
//        {
//            MeshFilter meshFilter = meshObject.GetComponent<MeshFilter>();

//            if (meshFilter != null && meshFilter.sharedMesh != null)
//            {
//                combine[i].mesh = meshFilter.sharedMesh;
//                combine[i].transform = meshObject.transform.localToWorldMatrix;
//                i++;
//            }
//            else
//            {
//                Debug.LogWarning("유효한 MeshFilter 또는 Mesh가 없습니다: " + meshObject.name);
//            }
//        }

//        if (i == 0)
//        {
//            Debug.LogWarning("결합할 유효한 메쉬가 없습니다.");
//            return;
//        }

//        // 결합된 메쉬 생성
//        Mesh combinedMesh = new Mesh();
//        combinedMesh.CombineMeshes(combine);

//        // 새로운 GameObject 생성 및 결합된 메쉬 할당
//        GameObject combinedObject = new GameObject("CombinedMesh");

//        // Combine된 메쉬가 null인지 확인
//        if (combinedMesh != null && combinedMesh.isReadable)
//        {
//            MeshFilter meshFilterCombined = combinedObject.AddComponent<MeshFilter>();
//            meshFilterCombined.mesh = combinedMesh;
//            MeshRenderer meshRenderer = combinedObject.AddComponent<MeshRenderer>();
//            meshRenderer.sharedMaterial = meshesToCombine[0].GetComponent<MeshRenderer>().sharedMaterial;

//            // 결합된 메쉬를 에셋으로 저장
//            string meshAssetPath = "Assets/Prefabs/CombinedMesh_Mesh.asset";
//            AssetDatabase.CreateAsset(combinedMesh, meshAssetPath);
//            AssetDatabase.SaveAssets();
//            Debug.Log("결합된 메쉬가 에셋으로 저장되었습니다: " + meshAssetPath);

//            // 프리팹 저장 (Editor에서만 가능)
//            string prefabPath = "Assets/Prefabs/CombinedMesh.prefab";
//            PrefabUtility.SaveAsPrefabAsset(combinedObject, prefabPath);
//            Debug.Log("프리팹이 저장되었습니다: " + prefabPath);
//        }
//        else
//        {
//            Debug.LogWarning("결합된 메쉬가 유효하지 않거나 읽을 수 없습니다.");
//        }

//        // 원본 오브젝트들 비활성화
//        foreach (var meshObject in meshesToCombine)
//        {
//            meshObject.SetActive(false);
//        }

//        // 결합된 오브젝트는 씬에 남겨두기
//        DestroyImmediate(combinedObject);  // 씬에서 바로 제거
//    }
//}