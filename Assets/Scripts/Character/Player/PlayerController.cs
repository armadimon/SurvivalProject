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

    public float runSpeedMultiplier;    // �޸��� �� �̵� �ӵ� ���
    public float runStamina;            // �Ҹ�Ǵ� �޸��� ���׹̳�
    private float originMoveSpeed;      // �ʱ� �̵� �ӵ� (������)

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

    // �� ���ñ�
    public bool isInHydrateLocation = false;
    public bool isDrinking = false;


    public Action Inventory;            // �κ��丮 ���� �̺�Ʈ
    private PlayerCondition playerCondition; // PlayerCondition ������Ʈ (���¹̳� �� ���� ����)


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        playerCondition = GetComponent<PlayerCondition>();
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
        // ī�޶� ȸ�� ó���� ���� (���콺 �Է� �ݿ�)
        if (canLook)
        {
            CameraLook();
        }
        // ����� �뵵�� �÷��̾� �Ʒ��� Ray�� �׸�
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


    // �޸��� �Է� ó�� (���¹̳� �Ҹ� �� �̵� �ӵ� ����)
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            // �޸��� ����: ���¹̳��� �Ҹ��ϰ� �̵� �ӵ��� ����
            if (playerCondition.UseStamina(runStamina))
            {
                moveSpeed *= runSpeedMultiplier;
                StartCoroutine(RunStaminaDrain());
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            // �޸��� ���� �� �ʱ� �̵� �ӵ��� ����
            moveSpeed = originMoveSpeed;
        }
    }

    // �޸��� �� ���������� ���¹̳� �Ҹ��ϴ� �ڷ�ƾ
    private IEnumerator RunStaminaDrain()
    {
        while (moveSpeed > originMoveSpeed)
        {
            if (!playerCondition.UseStamina(runStamina * Time.deltaTime))
            {
                // ���¹̳� ���� �� �̵� �ӵ��� �ʱ�ȭ
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

    // �κ��丮 ȣ�� �Է� ó�� (�κ��丮 UI ǥ��)
    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Inventory?.Invoke();
            ToggleCursur(); // �κ��丮 ���� �� Ŀ���� ǥ��
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
