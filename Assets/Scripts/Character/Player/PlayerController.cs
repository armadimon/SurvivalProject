using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private Vector2 _curMoveInput;
    public float jumpPower;
    public LayerMask groundLayerMask;
    public bool isGrounded;
    public float playerHeight;

    public float runSpeedMultiplier;    // 달리기 시 이동 속도 배수
    public float runStamina;            // 소모되는 달리기 스테미나
    private float originMoveSpeed;      // 초기 이동 속도 (복구용)

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;
    private Vector2 mouseDelta;
    public bool canLook = true;

    private Rigidbody _rigidbody;
    private BuildController _buildController;

    // 물 마시기
    public bool isInHydrateLocation = false;
    public bool isDrinking = false;


    public Action Inventory;            // 인벤토리 열기 이벤트
    private PlayerCondition playerCondition; // PlayerCondition 컴포넌트 (스태미나 등 상태 관리)


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        playerCondition = GetComponent<PlayerCondition>();
        originMoveSpeed = moveSpeed;                // 초기 이동 속도 저장
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _buildController = GetComponent<BuildController>();
    }

    private void Update()
    {
        LimitSpeed();
    }

    private void FixedUpdate()
    {
        IsGrounded();
        Move();
    }

    private void LateUpdate()
    {
        // 카메라 회전 처리를 수행 (마우스 입력 반영)
        if (canLook)
        {
            CameraLook();
        }
        //Debug.DrawRay(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down * 1.5f, Color.red);
    }

    void Move()
    {
        Vector3 dir = transform.forward * _curMoveInput.y + transform.right * _curMoveInput.x;
        // dir.y = _rigidbody.velocity.y;
        
        _rigidbody.AddForce(dir.normalized * (moveSpeed * 10f), ForceMode.Force);
    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
        
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _curMoveInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _curMoveInput = Vector2.zero;
        }
    }


    // 달리기 입력 처리 (스태미나 소모 및 이동 속도 증가)
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Debug.Log("a");
            // 달리기 시작: 스태미나를 소모하고 이동 속도를 높임
            if (playerCondition.UseStamina(runStamina))
            {
                moveSpeed *= runSpeedMultiplier;
                StartCoroutine(RunStaminaDrain());
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            // 달리기 중지 시 초기 이동 속도로 복구
            moveSpeed = originMoveSpeed;
        }
    }

    // 달리기 시 지속적으로 스태미나 소모하는 코루틴
    private IEnumerator RunStaminaDrain()
    {
        while (moveSpeed > originMoveSpeed)
        {
            if (!playerCondition.UseStamina(runStamina * Time.deltaTime))
            {
                // 스태미나 부족 시 이동 속도를 초기화
                moveSpeed = originMoveSpeed;
                break;
            }
            yield return null;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }
    }
    

    void ToggleCursur()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f), Vector3.down),
        };
        for (int i = 0; i < rays.Length; ++i)
        {
            Debug.DrawRay(rays[i].origin, rays[i].direction * playerHeight, Color.red);
            if (Physics.Raycast(rays[i], playerHeight + 0.2f, groundLayerMask))
            {
                return true;
            }
        }
        return false;
    }

    private void LimitSpeed()
    {
        Vector3 flatVelocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);

        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitVelocity = flatVelocity.normalized * moveSpeed;
            _rigidbody.velocity = new Vector3(limitVelocity.x, _rigidbody.velocity.y, limitVelocity.z);
        }
    }

    // 인벤토리 호출 입력 처리 (인벤토리 UI 표시)
    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Inventory?.Invoke();
            ToggleCursur(); // 인벤토리 오픈 시 커서를 표시
        }
    }

    public void OnDrinking(InputAction.CallbackContext context)
    {
        if (isInHydrateLocation && context.phase == InputActionPhase.Started)
        {
            isDrinking = true;
            Invoke("StopDrinking", 5f);
        }
    }

    void StopDrinking()
    {
        isDrinking = false;
    }
}
