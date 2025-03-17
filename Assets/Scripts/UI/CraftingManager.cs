using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance;
    public List<CraftingRecipe> craftingRecipes;  // 등록된 제작법 리스트


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(this);
        }
    }

    // 아이템 제작이 가능한지 확인
    public bool CanCraft(CraftingRecipe recipe, int quantity = 1)
    {
        foreach (ItemData resource in recipe.requireResourcesItem)
        {
            //if (!InventotyManager.Instance.HasResourceAmount(resource, false, quantity));
            return false;
        }

        return true;
    }

    // 아이템을 제작한다.
    public void CraftItem(CraftingRecipe recipe, int quantity)
    {
        //재료가 모자르다면 제작 불가
        if (!CanCraft(recipe, quantity))
        {
            // 재료가 부족하다는 창 띄우기
            return;  
        }

        foreach (ItemData resource in recipe.requireResourcesItem)
        {
            InventotyManager.Instance.RemoveItem(resource, resource.requireResourceAmout.value * quantity);  //재료를 소모한다
        }
        // 아이템을 추가
        InventotyManager.Instance.AddItem(recipe.resultItem, recipe.resultAmount * quantity);
        InventotyManager.Instance.Inventory.UpdateUI();
        // 제작 완료 창 띄우기
    }

}
