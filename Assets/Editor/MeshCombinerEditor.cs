// using UnityEditor;
// using UnityEngine;
//
// [CustomEditor(typeof(MeshCombiner))]
// public class MeshCombinerEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         base.OnInspectorGUI(); // 기존 인스펙터 UI를 유지
//
//         MeshCombiner meshCombiner = (MeshCombiner)target;
//
//         if (GUILayout.Button("Combine and Save as Prefab"))
//         {
//             // 결합 메쉬 함수 호출
//             meshCombiner.CombineMeshesAndSavePrefab();
//         }
//     }
// }