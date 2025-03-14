using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    public float offsetDistance = 1.0f;

    private void Start()
    {
        Transform parent = transform.parent;
        if (parent == null) return;

        if (name.Contains("Forward")) transform.localPosition = new Vector3(0, 0, offsetDistance);
        else if (name.Contains("Backward")) transform.localPosition = new Vector3(0, 0, -offsetDistance);
        else if (name.Contains("Left")) transform.localPosition = new Vector3(-offsetDistance, 0, 0);
        else if (name.Contains("Right")) transform.localPosition = new Vector3(offsetDistance, 0, 0);
    }
}
