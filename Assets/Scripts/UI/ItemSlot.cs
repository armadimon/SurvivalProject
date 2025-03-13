using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ItemSlot : MonoBehaviour
{
    public ItemData item;
    public UIInventory Inventory;

    public Button button;
    public Image icon;
    public GameObject ItemQuantityImage;
    public TextMeshProUGUI quantityText;
    private Outline outline;
    private Shadow shadow;


    public int index;
    public bool equipped;
    public int quantity;


    private void Awake()
    {
        ItemQuantityImage = transform.Find("ItemQuantityImage").gameObject;
        quantityText = transform.Find("ItemQuantityImage/QuantityText").GetComponent<TextMeshProUGUI>();
        outline = GetComponent<Outline>();
        shadow = GetComponent<Shadow>();
        button = GetComponent<Button>();
        icon = transform.Find("Icon").GetComponent<Image>();
        

    }

    // Start is called before the first frame update
    void Start()
    {
        Inventory = InventotyManager.Instance.Inventory;
        ItemQuantityImage.SetActive(false);
    }

    private void OnEnable()
    {
        outline.enabled = equipped;
        shadow.enabled = equipped;
    }


    public void Set()
    {
        icon.gameObject.SetActive(true);
        icon.sprite = item.icon;
        if (quantity > 1)
        {
            quantityText.text = quantity.ToString();
            ItemQuantityImage.SetActive(true);
        }
        else
        {
            quantityText.text = string.Empty;
            ItemQuantityImage.SetActive(false);
        }

        if (outline != null)
        {
            outline.enabled = equipped;
        }
        if (shadow != null)
        {
            shadow.enabled = equipped;
        }
    }

    public void Clear()
    {
        item = null;
        icon.gameObject.SetActive(false);
        quantityText.text = string.Empty;
        ItemQuantityImage.SetActive(false);
    }

}
