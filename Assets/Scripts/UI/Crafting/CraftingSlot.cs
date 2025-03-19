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

        materialSecondQuantityText.enabled = false;
        addText.enabled = false;
        materialSecondImage.enabled = false;
    }


    public void Set(CraftingRecipe newRecipe)
    {
        recipe = newRecipe;
        

        materialFirstItem = recipe.requrireResourcesItem[0].requireItem;
        materialFirstQuantityText.text = recipe.requrireResourcesItem[0].amount.ToString();
        materialFirstImage.sprite = recipe.requrireResourcesItem[0].requireItem.icon;

        if (recipe.requrireResourcesItem.Length > 1)
        {
            materialSecondImage.enabled = true;
            materialSecondQuantityText.enabled = true;
            addText.enabled = true;

            meterialSecondItem = recipe.requrireResourcesItem[1].requireItem;
            materialSecondQuantityText.text = recipe.requrireResourcesItem[1].amount.ToString();
            materialSecondImage.sprite = recipe.requrireResourcesItem[1].requireItem.icon;

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
