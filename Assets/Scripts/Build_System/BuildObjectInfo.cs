using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildObjectInfo : MonoBehaviour
{
    public Image selectBuildObjectIcon;
    public TextMeshProUGUI selectBuildObjectName;
    public TextMeshProUGUI selectBuildObjectHP;
    public TextMeshProUGUI selectBuildObjectDescription;

    // RequireResource UI를 담을 컨테이너(예: VerticalLayoutGroup이 붙은 Panel)
    public Transform requiredResourceContainer;
    // 각 리소스를 표시할 프리팹 (ResourceItemUI가 붙어있어야 함)
    public GameObject resourceItemPrefab;

    // BuildObject 데이터를 받아 UI를 업데이트하는 함수
    public void SetBuildObjectData(BuildObjectData buildObjectData)
    {
        // 기존 리소스 UI 삭제
        foreach (Transform child in requiredResourceContainer)
        {
            Destroy(child.gameObject);
        }

        // 나머지 정보 업데이트
        selectBuildObjectIcon.sprite = buildObjectData.icon;
        selectBuildObjectName.text = buildObjectData.displayName;
        selectBuildObjectHP.text = buildObjectData.maxHealth.ToString();
        selectBuildObjectDescription.text = buildObjectData.description;

        // RequireResource 부분 처리 (단일 또는 다중)
        if (buildObjectData.requireResources != null && buildObjectData.requireResources.Length > 0)
        {
            foreach (RequireResourceAmount resource in buildObjectData.requireResources)
            {
                // 프리팹을 인스턴스화하여 컨테이너에 추가
                GameObject resourceItem = Instantiate(resourceItemPrefab, requiredResourceContainer);
                ResourceItemUI resourceUI = resourceItem.GetComponent<ResourceItemUI>();
                if (resourceUI != null)
                {
                    resourceUI.SetResource(resource.type, resource.value);
                }
            }
        }
    }
}