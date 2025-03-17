using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICrafting : MonoBehaviour
{
    public Transform craftingSlotsTransform;     //제작슬롯의 부모(Slots)
    public GameObject craftingSlotPrefab;   // 제작슬롯 프리팹
    public List<CraftingRecipe> allRecipes; 

    private CraftingRecipe selectedRecipe;

    public CraftingSlot[] craftingSlot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RefreshCraftingUI()
    {
        foreach (Transform child in craftingSlotsTransform)
        {
            Destroy(child.gameObject);
        }

        foreach (CraftingRecipe recipe in allRecipes)
        {
            if (CraftingManager.Instance.CanCraft(recipe))
            {
                GameObject slot = Instantiate(craftingSlotPrefab, craftingSlotsTransform);
                slot.GetComponent<CraftingSlot>().Set(recipe);
            }
        }

    }

}
