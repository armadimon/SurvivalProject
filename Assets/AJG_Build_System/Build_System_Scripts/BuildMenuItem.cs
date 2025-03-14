using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuItem : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI DescriptionText;
    public Button selectButton;
    public RequireResourceAmount[] requireResourceTypes;
    // public BuildObject buildObject;
    public int index;

    public void SetData(BuildObject newBuildObject, int index)
    {  
        icon.sprite = newBuildObject.data.icon;
        nameText.text = newBuildObject.data.displayName;
        DescriptionText.text = newBuildObject.data.description;
        requireResourceTypes = newBuildObject.data.requireResources;
        // buildObject = newBuildObject;
        // buildObject.data.OnClick = () => BuildManager.Instance.buildController.SetBuildObject(buildObject);
        //
        selectButton.onClick.AddListener(() => BuildManager.Instance.CheckSufficientResources(newBuildObject, index));
    }
}
