using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    private static CraftingManager _instance;
    public static CraftingManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CraftingManager>();

                if (_instance == null)
                {
                    _instance = new GameObject("CraftingManager").AddComponent<CraftingManager>();

                }
            }
            return _instance;
        }
    }




    private UICrafting _uiCrafting;

    public UICrafting UICrafting
    {

        get { return _uiCrafting; }
        set { _uiCrafting = value; }
    }

    public List<CraftingRecipe> allRecipes;         // 전체 레시피 목록

    

    private void Awake()
    {
        if (_instance != null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (_instance == this)
            {
                Destroy(gameObject);

            }
        }

        allRecipes = new List<CraftingRecipe>(Resources.LoadAll<CraftingRecipe>("Recipes"));

        if (allRecipes == null || allRecipes.Count == 0)
        {
            allRecipes = new List<CraftingRecipe>(Resources.LoadAll<CraftingRecipe>("Recipes"));
        }
    }

    private void Start()
    {

        if (allRecipes == null || allRecipes.Count == 0)
        {
            Debug.LogError("CraftingManager : allRecipes 리스트가 비어있음");
        }
        else
        {
            Debug.Log($"{allRecipes.Count}개의 제작법이 로드됨");
        }

    }

    public void DisplayCraftingRecipes(RecipeType recipeType)
    {
        if (allRecipes == null || allRecipes.Count == 0)
        {
            Debug.LogError("CraftingManager : allRecipes 가 비어있음");
            return;
        }

        List<CraftingRecipe> filterRecipes = allRecipes.FindAll(_recipe => _recipe.recipeType == recipeType);
        if (filterRecipes == null || filterRecipes.Count == 0)
        {
            Debug.Log($"{recipeType}에 해당하는 레시피가 존재하지 않음");
        }

        UICrafting.UpdateCraftingUI(filterRecipes);

    }

    // 아이템을 제작한다.
    public void CraftItem(CraftingRecipe recipe, int quantity)
    {
        InventotyManager inventory = InventotyManager.Instance;

        // 필요한 재료가 충분히 있는지
        foreach (var requiredItem in recipe.requrireResourcesItem)
        {
            if (!inventory.HasItem(requiredItem.requireItem, requiredItem.amount * quantity))
            {
                NotificationManager.Instance.ShowNotification("재료가 모자랍니다.");
                return;
            }
        }

        // 재료 소모
        foreach (var resourceItem in recipe.requrireResourcesItem)
        {
            inventory.RemoveItem(resourceItem.requireItem, resourceItem.amount * quantity);  //재료를 소모한다
        }
        // 아이템을 추가
        InventotyManager.Instance.AddItem(recipe.resultItem, recipe.resultAmount * quantity);
        InventotyManager.Instance.Inventory.UpdateUI();
        NotificationManager.Instance.ShowNotification($"{recipe.resultItem.disPlayName} 제작 완료!");
        // 제작 완료 창 띄우기
    }

}
