using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSlot : MonoBehaviour
{
    public Image materialFirstImage;
    public TextMeshProUGUI materialFirstQuantityText;
    public Image materialSecondImage;
    public TextMeshProUGUI materialSecondQuantityText;
    public Image resultImage;
    public TextMeshProUGUI resultItemQuantityText;
    public TextMeshProUGUI addText;
    public Button craftingButton;

    public ItemData materialFirstItem;
    public ItemData meterialSecondItem;
    public ItemData resultItem;

    private CraftingRecipe recipe;

    private void Awake()
    {
        materialFirstImage = transform.Find("materialFirst").gameObject.GetComponent<Image>();
        materialSecondImage = transform.Find("materialSecond").gameObject.GetComponent<Image>();
        resultImage = transform.Find("completeItem").gameObject.GetComponent<Image>();

        materialFirstQuantityText = transform.Find("materialFirst/firstQuantityText").gameObject.GetComponent<TextMeshProUGUI>();
        materialSecondQuantityText = transform.Find("materialSecond/secondQuantityText").gameObject.GetComponent<TextMeshProUGUI>();
        resultItemQuantityText = transform.Find("completeItem/resultQuantityText").gameObject.GetComponent<TextMeshProUGUI>();
        addText = transform.Find("AddText").gameObject.GetComponent <TextMeshProUGUI>();

        craftingButton = transform.Find("CraftingButton").GetComponent<Button>();
    }

    private void Start()
    {
        materialSecondQuantityText.enabled = false;
        addText.gameObject.SetActive(false);
        materialSecondImage.enabled = false;
    }

    public void Set(CraftingRecipe newRecipe)
    {
        recipe = newRecipe;
        

        materialFirstItem = recipe.requireResourcesItem[0];
        materialFirstQuantityText.text = recipe.requireResourcesItem[0].requireResourceAmout.value.ToString();
        materialFirstImage.sprite = recipe.requireResourcesItem[0].icon;

        if (recipe.requireResourcesItem.Length > 1)
        {
            materialSecondImage.enabled = true;
            materialSecondQuantityText.enabled = true;
            addText.enabled = true;

            meterialSecondItem = recipe.requireResourcesItem[1];
            materialSecondQuantityText.text = recipe.requireResourcesItem[1].requireResourceAmout.value.ToString();
            materialSecondImage.sprite = recipe.requireResourcesItem[1].icon;

        }
        else
        {
            materialSecondImage.enabled = false;
            materialSecondQuantityText.enabled = false;
            addText.enabled = false;
        }



        resultItem = recipe.resultItem;
        resultItemQuantityText .text = recipe.resultAmount.ToString();
        resultImage.sprite = recipe.resultItem.icon;

        craftingButton.onClick.AddListener(() => CraftItem());
    }


    private void CraftItem()
    {
        CraftingManager.Instance.CraftItem(recipe, 1);
    }

   



}
