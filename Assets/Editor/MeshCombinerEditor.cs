using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshCombiner))]
public class MeshCombinerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // 기존 인스펙터 UI를 유지

        // 현재 객체를 MeshCombiner로 캐스팅
        MeshCombiner meshCombiner = (MeshCombiner)target;

        // "Combine and Save as Prefab" 버튼을 인스펙터에 추가
        if (GUILayout.Button("Combine and Save as Prefab"))
        {
            // 결합 메쉬 함수 호출
            meshCombiner.CombineMeshesAndSavePrefab();
        }
    }
}