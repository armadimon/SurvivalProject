using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildController : MonoBehaviour
{
    public GameObject buildObject;
    public bool buildMode = false;
    public bool SetMode = false;
    private GameObject _buildObject;
    
    public LayerMask placementLayer;  // 배치가 가능한 레이어
    public float maxSlopeAngle = 45f; // 최대 허용 기울기 (단위: 도)

    private MeshRenderer _objectMeshRenderer;
    private Color _objectOriginalColor;
    
    public float rayCastDistance = 5f;


    private void Start()
    {
        buildMode = false;
        if (buildObject != null)
        {
            SetMode = true;
            _buildObject = Instantiate(buildObject,
                transform.position + (transform.forward * 2f)
                , Quaternion.identity)
                ;
            // _buildObject.transform.parent = transform;
            _objectMeshRenderer = _buildObject.GetComponent<MeshRenderer>();
            _objectOriginalColor = _objectMeshRenderer.material.color;
        }
    }

    void Update()
    {
        if (buildMode)
        {
            if (SetMode)
            {
                bool setable = TrySet();
                if (Input.GetMouseButtonDown(0))
                {
                    if (setable)
                        ObjectSet();
                }
            }
        }
    }

    private bool TrySet()
    {
        bool isSetable = false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, rayCastDistance, placementLayer))
        {
            Debug.DrawRay(ray.origin, ray.direction * rayCastDistance, Color.red);
            Vector3 hitNormal = hit.normal;
            float angle = Vector3.Angle(Vector3.up, hitNormal);

            _buildObject.transform.position = hit.point;
            _buildObject.transform.rotation = Quaternion.LookRotation(hitNormal);
            if (angle <= maxSlopeAngle)
            {
                _objectMeshRenderer.material.color = _objectOriginalColor;
                isSetable =  true;
            }
            else
            {
                _objectMeshRenderer.material.color = new Color(1,
                    0,
                    0,
                    0.5f);
                isSetable = false;
            }
        }
        else
        {
            _buildObject.transform.position = transform.forward * rayCastDistance;
        }

        return (isSetable);
    }

    private void ObjectSet()
    {   
            // _buildObject.transform.SetParent(null);
            _buildObject = null;
            SetMode = false;
    }
    
    public void OnBuildMode(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && buildMode == false)
        {
            Debug.Log("Build Mode On");
            buildMode = true;
        }
        else if (context.phase == InputActionPhase.Started && buildMode == true)
        {
            Debug.Log("Build Mode Off");
            buildMode = false;
        }
    }
}
