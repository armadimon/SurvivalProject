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
    
    public Camera snapPointCamera;
    public LayerMask placementLayer;
    public BuildObject _buildObject;
    private MeshRenderer _objectMeshRenderer;
    private Color _objectOriginalColor;
    private Collider objectCollider;
    private Color _objectCantSetableColor;
    public float rayCastDistance = 5f;

    private int currentRotationIndex = 0;
    private List<Quaternion> availableRotations = new List<Quaternion>();
    
    public float snapRange = 2.0f;
    public LayerMask snapLayer;
    private SnapPoint closestSnapPoint;
    
    // 테스트용
    
    private void Start()
    {
        buildMode = false;
    }

    void Update()
    {
        if (buildMode && SetMode)
        {
            closestSnapPoint = null;
            bool setable = TrySet();
            if (Input.GetMouseButtonDown(0) && setable)
            {
                ObjectSet();
            }

            // Q 또는 E 입력 시 회전 변경
            if (closestSnapPoint != null && availableRotations.Count > 0)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    currentRotationIndex--;
                    if (currentRotationIndex < 0)
                    {
                        currentRotationIndex = availableRotations.Count - 1;
                    }
                    closestSnapPoint.ApplyRotation(currentRotationIndex);
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    currentRotationIndex++;
                    if (currentRotationIndex >= availableRotations.Count)
                    {
                        currentRotationIndex = 0;
                    }
                    closestSnapPoint.ApplyRotation(currentRotationIndex);
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
            Vector3 hitNormal = hit.normal;
            float angle = Vector3.Angle(Vector3.up, hitNormal);

            _buildObject.transform.position = hit.point;
            if (TrySnapToClosestPoint(hit))
            {
                _buildObject.transform.position = closestSnapPoint.transform.position;
                _buildObject.transform.rotation = closestSnapPoint.transform.rotation;
                _objectMeshRenderer.material.color = _objectOriginalColor;
                isSetable =  true;
            }
            else if (angle <= _buildObject.data.minSlopeAngle)
            {
                _buildObject.transform.rotation = _buildObject.originalRotation;
                _objectMeshRenderer.material.color = _objectOriginalColor;
                isSetable =  true;
            }
            else if (angle <= _buildObject.data.maxSlopeAngle && angle > _buildObject.data.minSlopeAngle)
            {
                Vector3 forwardDirection = Vector3.Cross(hitNormal, Vector3.right);
                _buildObject.transform.rotation = Quaternion.LookRotation(forwardDirection, hitNormal);
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
            Transform _cameraContainer = CharacterManager.Instance.Player.controller.cameraContainer;
            _buildObject.transform.position = cameraContainer.position + cameraContainer.forward * rayCastDistance;
            _objectMeshRenderer.material.color = _objectCantSetableColor;
        }

        return (isSetable);
    }
    
    private bool TrySnapToClosestPoint(RaycastHit hit)
    {
        Collider[] colliders = Physics.OverlapSphere(hit.transform.position, snapRange, snapLayer);
        
        SnapPoint bestSnapPoint = null;
        float closestDistance = Mathf.Infinity;
        List<Quaternion> rotations = new List<Quaternion>();

        foreach (Collider col in colliders)
        {
            SnapPoint snapPoint = col.GetComponent<SnapPoint>();
            if (snapPoint == null) continue;

            float distance = Vector3.Distance(_buildObject.transform.position, col.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                bestSnapPoint = snapPoint;
                rotations = snapPoint.validRotations;
            }
        }

        if (bestSnapPoint != null)
        {
            _buildObject.transform.position = bestSnapPoint.transform.position;
            availableRotations = rotations;
            _buildObject.transform.rotation = bestSnapPoint.transform.rotation;
            closestSnapPoint = bestSnapPoint;
            return true;
        }
        return false;
    }
    
    private void OnDrawGizmos()
    {
        Collider[] colliders = new Collider[0];
        if (_buildObject != null)
        {
            colliders = Physics.OverlapSphere(_buildObject.transform.position, snapRange, snapLayer);
            Gizmos.color = colliders.Length > 0 ? Color.green : Color.red;
            Gizmos.DrawWireSphere(_buildObject.transform.position, snapRange);
        }
    }
    
    
    private void ObjectSet()
    {   
            // _buildObject.transform.SetParent(null);
            _buildObject.snapPointGroup.gameObject.SetActive(true);
            objectCollider.enabled = true;
            _buildObject = null;
            closestSnapPoint = null;
            SetMode = false;
            // ???⑤베????ш끽維뽳쭛?ｌ뒙???⑥ル땻???釉먮폇??
            buildMode = false;
    }
    
    public void OnBuildMode(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && buildMode == false)
        {
            int snapPointLayer = LayerMask.NameToLayer("SnapPoint");
            snapPointCamera.cullingMask |= (1 << snapPointLayer); 
            Debug.Log("Build Mode On");
            
            buildMode = true;
            BuildManager.Instance.buildMenu.SetActive(true);
            CharacterManager.Instance.Player.controller.canLook = false;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (context.phase == InputActionPhase.Started && buildMode == true)
        {
            Debug.Log("Build Mode Off");
            int snapPointLayer = LayerMask.NameToLayer("SnapPoint");
            snapPointCamera.cullingMask &= ~(1 << snapPointLayer); 
            buildMode = false;
            SetMode = false;
            if (_buildObject != null)
            {
                Destroy(_buildObject.gameObject);
                _buildObject = null;
                _objectMeshRenderer = null;
                closestSnapPoint = null;
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
        _buildObject.snapPointGroup?.gameObject.SetActive(false);
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
