using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventotyManager : MonoBehaviour
{
    private static InventotyManager _instance;
    public static InventotyManager Instance 
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("InventotyManager").AddComponent<InventotyManager>();
            }
            return _instance;
        }
    }

    public UIInventory _inventory;

    public UIInventory Inventory
    {
        get { return _inventory; }
        set { _inventory = value; }
    }

    public int maxSlots = 20;


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
    }




}
