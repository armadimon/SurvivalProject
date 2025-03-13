using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildController : MonoBehaviour
{
    public bool buildMode = false;
    public bool SetMode = false;
    
    public LayerMask placementLayer;  // 獄쏄퀣?귛첎? 揶쎛?館釉???됱뵠??
    public float maxSlopeAngle = 45f; // 筌ㅼ뮆? ??됱뒠 疫꿸퀣?길묾?(??μ맄: ??
    public BuildObject _buildObject;
    private MeshRenderer _objectMeshRenderer;
    private Color _objectOriginalColor;
    private Collider objectCollider;
    private Color _objectCantSetableColor;
    public float rayCastDistance = 5f;


    private void Start()
    {
        buildMode = false;
        // if (buildObject != null)
        // {
        //     SetMode = true;
        //     _buildObject = Instantiate(buildObject,
        //         transform.position + (transform.forward * 2f)
        //         , Quaternion.identity)
        //         ;
        //     objectCollider = _buildObject.GetComponentInChildren<Collider>();
        //     objectCollider.enabled = false;
        //     _objectMeshRenderer = _buildObject.GetComponentInChildren<MeshRenderer>();
        //     _objectOriginalColor = _objectMeshRenderer.material.color;
        //     _objectCantSetableColor = Color.red;
        //     _objectCantSetableColor.a = 0.5f;
        //     _objectMeshRenderer.material.color = _objectCantSetableColor;
        // }
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

        Transform cameraContainer = CharacterManager.Instance.Player.controller.cameraContainer;
        Ray ray = new Ray(cameraContainer.position, cameraContainer.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, rayCastDistance, placementLayer))
        {
            Debug.DrawRay(ray.origin, ray.direction * rayCastDistance, Color.red);
            Vector3 hitNormal = hit.normal;
            float angle = Vector3.Angle(Vector3.up, hitNormal);

            _buildObject.transform.position = hit.point;
            Vector3 forwardDirection = Vector3.Cross(hitNormal, Vector3.right);
            _buildObject.transform.rotation = Quaternion.LookRotation(forwardDirection, hitNormal);
            if (angle <= maxSlopeAngle)
            {
                _objectMeshRenderer.material.color = _objectOriginalColor;
                isSetable =  true;
            }
            else
            {
                _objectMeshRenderer.material.color = Color.red;
                Color color = _objectMeshRenderer.material.color;
                color.a = 0.5f;
                _objectMeshRenderer.material.color = color;
                isSetable = false;
            }
        }
        else
        {
            _buildObject.transform.position = cameraContainer.position + cameraContainer.forward * rayCastDistance;
            _objectMeshRenderer.material.color = _objectCantSetableColor;
        }

        return (isSetable);
    }

    
    private void ObjectSet()
    {   
            // _buildObject.transform.SetParent(null);
            objectCollider.enabled = true;
            _buildObject = null;
            SetMode = false;
            // ??곕뼊 ?袁⑸뻻嚥??곗눖??遺얜뼄.
            buildMode = false;
    }
    
    public void OnBuildMode(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && buildMode == false)
        {
            Debug.Log("Build Mode On");
            buildMode = true;
            BuildManager.Instance.buildMenu.SetActive(true);
            CharacterManager.Instance.Player.controller.canLook = false;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (context.phase == InputActionPhase.Started && buildMode == true)
        {
            Debug.Log("Build Mode Off");
            buildMode = false;
            SetMode = false;
            if (_buildObject != null)
            {
                Destroy(_buildObject.gameObject);
                _buildObject = null;
                _objectMeshRenderer = null;
            }
            BuildManager.Instance.buildMenu.SetActive(false);
            CharacterManager.Instance.Player.controller.canLook = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void SetBuildObject(BuildObject newBuildObject)
    {
        SetMode = true;
        _buildObject = Instantiate(newBuildObject,
            transform.position + (transform.forward * 2f)
            , Quaternion.identity)
            ;
        objectCollider = _buildObject.GetComponentInChildren<Collider>();
        objectCollider.enabled = false;
        _objectMeshRenderer = _buildObject.GetComponentInChildren<MeshRenderer>();
        _objectOriginalColor = _objectMeshRenderer.material.color;
        _objectCantSetableColor = Color.red;
        _objectCantSetableColor.a = 0.5f;
        _objectMeshRenderer.material.color = _objectCantSetableColor;
        BuildManager.Instance.buildMenu.SetActive(false);
        CharacterManager.Instance.Player.controller.canLook = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
