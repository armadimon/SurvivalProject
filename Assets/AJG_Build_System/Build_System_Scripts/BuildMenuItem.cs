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

    public void SetData(BuildObject buildObject)
    {   Debug.Log(buildObject);
        Debug.Log(icon);
        icon.sprite = buildObject.data.icon;
        nameText.text = buildObject.data.name;
        DescriptionText.text = buildObject.data.description;
        // buildObject.data.OnClick = () => BuildManager.Instance.buildController.SetBuildObject(buildObject);
        //
        selectButton.onClick.AddListener(() => BuildManager.Instance.buildController.SetBuildObject(buildObject));
    }
}
