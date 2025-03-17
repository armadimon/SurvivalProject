using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 상호작용 가능한 객체를 위한 인터페이스
public interface IInteractable
{
    // 상호작용 시 표시할 UI 프롬프트 문자열 반환
    public string GetInteractPrompt();

    // 상호작용 시 호출되는 메서드
    public void OnInteract();
}

// 아이템 객체 클래스: 상호작용 가능한 아이템을 나타냄
public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData itemData; // 아이템 데이터 (ScriptableObject)

    // 상호작용 시 표시할 UI 프롬프트 문자열 반환
    public string GetInteractPrompt()
    {
        // 아이템의 이름과 설명을 포함한 문자열 반환
        string str = $"{itemData.disPlayName}\n{itemData.description}";
        return str;
    }

    // 상호작용 시 호출되는 메서드
    public void OnInteract()
    {
        // 플레이어의 아이템 데이터에 현재 아이템 데이터 설정
        CharacterManager.Instance.Player.item = itemData;
        // 플레이어의 아이템 추가 이벤트 호출
        CharacterManager.Instance.Player.addItem?.Invoke();
        // 현재 게임 오브젝트 파괴 (아이템 획득 처리)
        Destroy(gameObject);
    }
}
