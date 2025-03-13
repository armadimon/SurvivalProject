using UnityEngine;
using UnityEngine.InputSystem;
 
public class Equipment : MonoBehaviour
{
    // 현재 장착된 장비 객체
    public Equip curEquip;

    // 장비가 장착될 부모 오브젝트 (예: 손 위치)
    public Transform equipParent;

    // 플레이어 컨트롤러 및 상태 관리 객체
    private PlayerController controller;

    void Start()
    {
        // 플레이어 관련 컴포넌트 가져오기
        controller = GetComponent<PlayerController>();
    }

    /// <summary>
    /// 새로운 장비를 장착하는 메서드
    /// 기존 장비를 해제한 후, 새로운 장비를 생성하여 장착
    /// </summary>
    /// <param name="data">장착할 아이템 데이터</param>
    public void EquipNew(ItemData data)
    {
        UnEquip(); // 기존 장비 해제
        curEquip = Instantiate(data.equipPrefab, equipParent).GetComponent<Equip>(); // 새로운 장비 생성 및 장착
    }

    /// <summary>
    /// 현재 장비를 해제하는 메서드
    /// </summary>
    public void UnEquip()
    {
        if (curEquip != null)
        {
            Destroy(curEquip.gameObject); // 현재 장착된 장비 오브젝트 삭제
            curEquip = null;
        }
    }

    /// <summary>
    /// 공격 입력을 처리하는 메서드
    /// </summary>
    /// <param name="context">입력 컨텍스트</param>
    public void OnAttackInput(InputAction.CallbackContext context)
    {
        // 공격 입력이 수행되었으며, 현재 장착된 장비가 있고, 플레이어가 시점을 조작할 수 있는 상태라면
        if (context.phase == InputActionPhase.Performed && curEquip != null && controller.canLook)
        {
            curEquip.OnAttackInput(); // 장비의 공격 메서드 호출
        }
    }
}