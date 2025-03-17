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

    private bool _setable = false;
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
            _setable = TrySet();

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
    
    
    public void OnObjectSet(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started &&
            SetMode == true && CheckOverlap(_buildObject.transform.position, _buildObject.transform.rotation))
        {
            _setable = false;
            objectCollider.enabled = false; 
            NotificationManager.Instance.ShowNotification("겹치는 물체가 있습니다!");
            return;
        }
        if (context.phase == InputActionPhase.Started && _setable == true && SetMode == true)
        {
            if (context.control.name == "leftButton")
            {
                _buildObject.snapPointGroup.gameObject.SetActive(true);
                SetMode = false;
                objectCollider.enabled = true;
                SettlementManager.Instance.RegisterBuildObject(_buildObject, _buildObject.isSafe);
                _buildObject = null;
                BuildManager.Instance.buildMenu.SetActive(true);
                CharacterManager.Instance.Player.controller.canLook = false;
                Cursor.lockState = CursorLockMode.None;
                return;
            }
            if (context.control.name == "rightButton")
            {
                _buildObject.snapPointGroup.gameObject.SetActive(true);
                objectCollider.enabled = true;
                closestSnapPoint = null;
                SettlementManager.Instance.RegisterBuildObject(_buildObject, _buildObject.isSafe);
                SetBuildObject(_buildObject);
            }
        }
    }
    
    public void OnBuildMode(InputAction.CallbackContext context)
    {
        if (buildMode == false)
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
            CharacterManager.Instance.Player.controller.playerInput.SwitchCurrentActionMap("Player");
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

    public void OnObjectRotation(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && SetMode == true)
        {
            Vector2 scrollValue = context.ReadValue<Vector2>(); // Vector2로 가져옴
            float scrollY = scrollValue.y; // Y값만 사용
            _buildObject.transform.Rotate(Vector3.up, scrollY * 10 * Time.deltaTime);
            _buildObject.originalRotation = _buildObject.transform.rotation;
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
    
    private bool CheckOverlap(Vector3 position, Quaternion rotation)
    {
        bool ret = false;
        if (objectCollider == null)
            return false;
        Collider[] hitColliders;
        
        int checkLayer = LayerMask.NameToLayer("BuildObject");
        if (objectCollider is BoxCollider boxCollider)
        {
            hitColliders = Physics.OverlapBox(position + boxCollider.center, boxCollider.size / 4f, rotation, checkLayer, QueryTriggerInteraction.Collide);
        }
        else if (objectCollider is SphereCollider sphereCollider)
        {
            hitColliders = Physics.OverlapSphere(position + sphereCollider.center, sphereCollider.radius / 2f, checkLayer, QueryTriggerInteraction.Collide);
        }
        else if (objectCollider is CapsuleCollider capsuleCollider)
        {
            Vector3 center = position + capsuleCollider.center;
            Vector3 point1, point2;
            float radius = capsuleCollider.radius;
        
            if (capsuleCollider.direction == 0) // X-axis
            {
                point1 = center + Vector3.right * (capsuleCollider.height / 2f - radius);
                point2 = center - Vector3.right * (capsuleCollider.height / 2f - radius);
            }
            else if (capsuleCollider.direction == 1) // Y-axis
            {
                point1 = center + Vector3.up * (capsuleCollider.height / 2f - radius);
                point2 = center - Vector3.up * (capsuleCollider.height / 2f - radius);
            }
            else // Z-axis
            {
                point1 = center + Vector3.forward * (capsuleCollider.height / 2f - radius);
                point2 = center - Vector3.forward * (capsuleCollider.height / 2f - radius);
            }
            hitColliders = Physics.OverlapCapsule(point1 / 2, point2 / 2 , radius / 2, checkLayer, QueryTriggerInteraction.Collide);
        }
        else
        {
            Debug.LogWarning("지원하지 않는 콜라이더 타입: " + objectCollider.GetType());
            return true;
        }

        foreach (Collider hitCollider in hitColliders)
        {
            Debug.Log(hitCollider);
        }
        return ret;
    }
}
