using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : MonoBehaviour, IInteractable
{
    public BuildObject BuildObject;

    public string GetInteractPrompt()
    {
        // 아이템의 이름과 설명을 포함한 문자열 반환
        string str = $"{BuildObject.data.displayName}\n{BuildObject.data.description}";
        return str;
    }

    // 상호작용 시 호출되는 메서드
    public void OnInteract()
    {
        if (!CraftingManager.Instance.UICrafting.IsOpen)
        {
            CraftingManager.Instance.UICrafting.ShowCraftingUI(RecipeType.Process);
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
        Debug.Log("Enter");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit");
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
