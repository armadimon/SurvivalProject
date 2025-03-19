using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlementBodty : MonoBehaviour, IInteractable
{
    public BuildObject settelBuildObject;
    
    public string GetInteractPrompt()
    {
        // 아이템의 이름과 설명을 포함한 문자열 반환
        string str = $"{settelBuildObject.data.displayName}\n{settelBuildObject.data.description}";
        return str;
    }

    // 상호작용 시 호출되는 메서드
    public void OnInteract()
    {
        SettlementManager.Instance.DisplaySettlementManageMenu();
    }
}
