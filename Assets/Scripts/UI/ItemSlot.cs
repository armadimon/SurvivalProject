using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemSlot : MonoBehaviour
{
    public ItemData item;
    public UIInventory Inventory;

    public GameObject ItemQuantityImage;

    public int index;
    public bool equipped;
    public int quantity;


    private void Awake()
    {
        ItemQuantityImage = transform.Find("ItemQuantityImage").gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        ItemQuantityImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
