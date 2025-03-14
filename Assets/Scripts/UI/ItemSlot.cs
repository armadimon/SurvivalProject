using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        button.onClick.AddListener(OnItemUseImage);


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

    public void SetButton()
    {
        button.onClick.AddListener(OnItemUseImage);
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            Inventory.ShowItemDescription(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Inventory.HideItemDescription();


    }

    public void OnItemUseImage()
    {
        if (item == null) return;
        Vector3 mousePos = Input.mousePosition;
        RectTransform rectDescrition = Inventory.itemUseImage.GetComponent<RectTransform>();

        float offsetX = (rectDescrition.rect.width / 2) + 20;
        //float offsetY = rectDescrition.rect.height / 2;

        mousePos.x += offsetX;

        Inventory.itemUseImage.transform.position = mousePos;

        Inventory.itemUseImage.SetActive(!Inventory.itemUseImage.activeSelf);
        Inventory.isUseItemWindow = Inventory.itemUseImage.activeSelf;

        if (Inventory.isUseItemWindow)
        {
            Inventory.itemDescription.SetActive(false);
        }
        else if (!Inventory.isUseItemWindow)
        {
            Inventory.itemDescription.SetActive(true);
        }

        Inventory.selectItem = this;
        Inventory.SelectItem(index);


    }

}
