// #if UNITY_EDITOR
// using UnityEditor;
// using UnityEngine;
// using System.IO;

// [CustomEditor(typeof(GameObject))]
// public class PrefabThumbnailEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         base.OnInspectorGUI();

//         GameObject prefab = (GameObject)target;

//         if (PrefabUtility.GetPrefabAssetType(prefab) == PrefabAssetType.Regular)
//         {
//             if (GUILayout.Button("Save Prefab as Sprite"))
//             {
//                 SavePrefabAsSprite(prefab);
//             }
//         }
//         else
//         {
//             EditorGUILayout.HelpBox("This is not a regular Prefab.", MessageType.Warning);
//         }
//     }

//     private void SavePrefabAsSprite(GameObject prefab)
//     {
//         // 프리팹 썸네일을 얻는다
//         EditorApplication.delayCall += () => { 
//             Texture2D previewTexture = AssetPreview.GetAssetPreview(prefab);

//             if (previewTexture == null)
//             {
//                 Debug.LogWarning("Preview is not ready yet. Try again.");
//                 return;
//             }

//             // 썸네일을 PNG로 저장
//             string folderPath = Application.dataPath + "/PrefabThumbnails/";
//             Directory.CreateDirectory(folderPath);
//             string filePath = folderPath + prefab.name + ".png";

//             byte[] bytes = previewTexture.EncodeToPNG();
//             File.WriteAllBytes(filePath, bytes);
//             AssetDatabase.Refresh();

//             // PNG 파일을 임포트하고 스프라이트로 변환
//             string assetPath = "Assets/PrefabThumbnails/" + prefab.name + ".png";
//             AssetDatabase.ImportAsset(assetPath);

//             // 임포트된 PNG 파일을 스프라이트로 변환
//             Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
//             if (texture != null)
//             {
//                 Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

//                 // 스프라이트 저장
//                 string spritePath = "Assets/PrefabThumbnails/" + prefab.name + "_Sprite.asset";
//                 AssetDatabase.CreateAsset(sprite, spritePath);
//                 AssetDatabase.SaveAssets();

//                 Debug.Log("Prefab saved as Sprite at: " + spritePath);
//             }
//             else
//             {
//                 Debug.LogWarning("Failed to load PNG as Texture2D.");
//             }
//         };
//     }
// }
// #endif