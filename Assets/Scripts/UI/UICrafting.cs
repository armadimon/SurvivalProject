using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UICrafting : MonoBehaviour
{
    [Header("Slot Data")]
    public Canvas craftingCanvas;
    public Transform craftingSlotsTransform;     //제작슬롯의 부모(Slots)
    public CraftingSlot craftingSlotPrefab;       // 제작슬롯 프리팹

    [Header("CraftingType Tap")]
    public GameObject equipTap;
    public GameObject foodTap;
    public GameObject processTap;

    public List<CraftingSlot> craftingSlots = new List<CraftingSlot>();

    private CraftingRecipe selectedRecipe;

    private void Awake()
    {
        if (craftingCanvas == null)
        {
            craftingCanvas = GetComponent<Canvas>();
        }

        if (CraftingManager.Instance != null)
        {
            CraftingManager.Instance.UICrafting = this;
        }

        equipTap.SetActive(false);
        foodTap.SetActive(false);
        processTap.SetActive(false);

    }

    private void Start()
    {
        craftingCanvas.enabled = false;
    }

    public void UpdateCraftingUI(List<CraftingRecipe> recipes)
    {
        // 기존 슬롯 제거
        foreach (var slot in craftingSlots)
        {
            Destroy(slot.gameObject);
        }
        craftingSlots.Clear();

        foreach (var recipe in recipes)
        {
            CraftingSlot newSlot = Instantiate(craftingSlotPrefab, craftingSlotsTransform);
            newSlot.gameObject.SetActive(true);
            newSlot.Set(recipe);
            craftingSlots.Add(newSlot);
        }

       

    }

    public bool IsOpen
    {
        get { return craftingCanvas.enabled; }
    }

    // UI를 활성화하고 업데이트를 시켜준다
    public void ShowCraftingUI(RecipeType recipeType)
    {
        craftingCanvas.enabled = true;
        CraftingManager.Instance.DisplayCraftingRecipes(recipeType);

        switch (recipeType)
        {
            case RecipeType.Equip:
                equipTap.SetActive(true);
                foodTap.SetActive(false);
                processTap.SetActive(false);
                break;

            case RecipeType.Food:
                equipTap.SetActive(false);
                foodTap.SetActive(true);
                processTap.SetActive(false);
                break;

            case RecipeType.Process:
                equipTap.SetActive(false);
                foodTap.SetActive(false);
                processTap.SetActive(true);
                break;


        }

    }

    public void HideCraftingUI()
    {
        craftingCanvas.enabled = false;
        
    }



}