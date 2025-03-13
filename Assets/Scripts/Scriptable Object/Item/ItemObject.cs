using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public string GetInteractPrompt();  // UI에 표시할 정보
    public void OnInteract();   // 인터랙션 호출
}

public class ItemObject : MonoBehaviour
{
    public ItemData ItemData;
}
