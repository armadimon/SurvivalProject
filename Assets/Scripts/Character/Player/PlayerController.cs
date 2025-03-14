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

    public float runSpeedMultiplier;    // ???怨ルЬ???????????쒖┣ ?꾩룄???
    public float runStamina;            // ???嫄??濡ル츎 ???怨ルЬ????댟誘?쾵筌뤾퍒??
    private float originMoveSpeed;      // ?貫?껆뵳?????????쒖┣ (?곌랜踰???

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

    // ??嶺뚮씭???룸Ь?
    public bool isInHydrateLocation = false;
    public bool isDrinking = false;


    public Action Inventory;            // ?筌뤾퍒萸??ル벣遊????⒱뵛 ???繹??
    private PlayerCondition playerCondition; // PlayerCondition ???샑???怨뺣콦 (???꾨Ф亦껋꼶梨뜻뤃?????⑤객臾???㉱??


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        playerCondition = GetComponent<PlayerCondition>();
        originMoveSpeed = moveSpeed;                // ?貫?껆뵳?????????쒖┣ ????
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
        // ?곸궠?筌???????嶺뚳퐣瑗?怨?ご???臾먮뺄 (嶺뚮씭???????놁졑 ?꾩룇瑗??
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


    // ???怨ルЬ????놁졑 嶺뚳퐣瑗??(???꾨Ф亦껋꼶梨뜻뤃????嫄???????????쒖┣ 嶺뚯빘鍮?)
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            // ???怨ルЬ???戮곗굚: ???꾨Ф亦껋꼶梨뜻뤃諛?ご????嫄???겶?????????쒖┣???沃섅굦肉?
            if (playerCondition.UseStamina(runStamina))
            {
                moveSpeed *= runSpeedMultiplier;
                StartCoroutine(RunStaminaDrain());
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            // ???怨ルЬ?繞벿살탳? ???貫?껆뵳?????????쒖┣???곌랜踰??
            moveSpeed = originMoveSpeed;
        }
    }

    // ???怨ルЬ???嶺뚯솘????곗쓤??怨쀬Ŧ ???꾨Ф亦껋꼶梨뜻뤃????嫄??濡ル츎 ?袁⑤?塋??
    private IEnumerator RunStaminaDrain()
    {
        while (moveSpeed > originMoveSpeed)
        {
            if (!playerCondition.UseStamina(runStamina * Time.deltaTime))
            {
                // ???꾨Ф亦껋꼶梨뜻뤃??遊붋????????????쒖┣???貫?껆뵳??
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

    // ?筌뤾퍒萸??ル벣遊??筌뤾쑵?????놁졑 嶺뚳퐣瑗??(?筌뤾퍒萸??ル벣遊?UI ??戮?뻣)
    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Inventory?.Invoke();
            ToggleCursur(); // ?筌뤾퍒萸??ル벣遊????덊깯 ????ｋ걠?묐슚紐???戮?뻣
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
