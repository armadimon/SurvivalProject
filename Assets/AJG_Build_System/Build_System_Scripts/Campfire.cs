using System.Collections;
using UnityEngine;

public class Campfire : MonoBehaviour, IInteractable
{
    public BuildObject buildObject;
    private Coroutine healCoroutine; // 힐을 위한 Coroutine 저장

    public string GetInteractPrompt()
    {
        // 아이템의 이름과 설명을 포함한 문자열 반환
        string str = $"{buildObject.data.displayName}\n{buildObject.data.description}";
        return str;
    }

    // 상호작용 시 호출되는 메서드
    public void OnInteract()
    {
        if (!CraftingManager.Instance.UICrafting.IsOpen)
        {
            CraftingManager.Instance.UICrafting.ShowCraftingUI(RecipeType.Food);
            CharacterManager.Instance.Player.controller.ToggleCursur();
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
        else
        {
            CraftingManager.Instance.UICrafting.HideCraftingUI();
            CharacterManager.Instance.Player.controller.ToggleCursur();
            if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어만 체크
        {
            // 체력 회복 시작
            if (healCoroutine == null)
            {
                healCoroutine = StartCoroutine(HealPlayerOverTime());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (healCoroutine != null)
            {
                StopCoroutine(healCoroutine);
                healCoroutine = null;
            }
            if (CraftingManager.Instance.UICrafting.IsOpen)
            {
                CraftingManager.Instance.UICrafting.HideCraftingUI();
                CharacterManager.Instance.Player.controller.ToggleCursur();
                if (Cursor.lockState == CursorLockMode.None)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
    }
    private IEnumerator HealPlayerOverTime()
    {
        while (true)
        {
            CharacterManager.Instance.Player.condition.Heal(1);
            yield return new WaitForSeconds(1f);
        }
    }
}