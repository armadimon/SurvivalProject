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

    public float runSpeedMultiplier;    // 달릴 때 이동속도에 곱해주는 값
    public float runStamina;            // 달리기 stamina
    private float originMoveSpeed;      // 처음 이동속도    

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

    public Action Inventory; // 인벤토리
    private PlayerCondition playerCondition;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        playerCondition = GetComponent<PlayerCondition>();
        originMoveSpeed = moveSpeed; // 처음 이동속도 저장
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
        // hunger 일정량 이하 이동속도 감소
        playerCondition.SlowFromHunger();
    }

    private void LateUpdate()
    {        
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

    // 달리기 InputAction
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            // 달리기 stamina 있을 때
            if (playerCondition.UseStamina(runStamina))
            {
                moveSpeed *= runSpeedMultiplier;
                StartCoroutine(RunStaminaDrain());
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            // 달리기 종료 시 처음 이동속도
            moveSpeed = originMoveSpeed;
        }
    }
    
    private IEnumerator RunStaminaDrain()
    {
        while (moveSpeed > originMoveSpeed)
        {
            if (!playerCondition.UseStamina(runStamina * Time.deltaTime))
            {                
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
    
    public void ToggleCursur()
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

    // 인벤토리 InputAction
    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Inventory?.Invoke();
            ToggleCursur(); // 인벤토리 열었을 때 마우스 잠금 해제
        }
    } 
}
