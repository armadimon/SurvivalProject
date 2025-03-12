using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    private static BuildManager _instance;

    public static BuildManager Instance
    {
        get { return _instance; }
    }

    public GameObject buildMenu;
    public Transform buildMenuContent;
    public BuildObject[] buildObjects;
    public BuildMenuItem buildMenuItemPrefabs;
    public BuildController buildController;
    public Action OnClick;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    
    void Start()
    {
        foreach (BuildObject buildObject in buildObjects)
        {
            BuildMenuItem buildItem = Instantiate(buildMenuItemPrefabs, buildMenuContent);

            buildItem.SetData(buildObject);
        }
    }
    
}
