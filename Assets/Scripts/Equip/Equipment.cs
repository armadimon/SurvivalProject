using UnityEngine;
using UnityEngine.InputSystem;
 
public class Equipment : MonoBehaviour
{
    // ?꾩옱 ?μ갑???λ퉬 媛앹껜
    public Equip curEquip;

    // ?λ퉬媛 ?μ갑??遺紐??ㅻ툕?앺듃 (?? ???꾩튂)
    public Transform equipParent;

    // ?뚮젅?댁뼱 而⑦듃濡ㅻ윭 諛??곹깭 愿由?媛앹껜
    private PlayerController controller;

    void Start()
    {
        // ?뚮젅?댁뼱 愿??而댄룷?뚰듃 媛?몄삤湲?
        controller = GetComponent<PlayerController>();
    }

    /// <summary>
    /// ?덈줈???λ퉬瑜??μ갑?섎뒗 硫붿꽌??
    /// 湲곗〈 ?λ퉬瑜??댁젣???? ?덈줈???λ퉬瑜??앹꽦?섏뿬 ?μ갑
    /// </summary>
    /// <param name="data">?μ갑???꾩씠???곗씠??/param>
    public void EquipNew(ItemData data)
    {
        UnEquip(); // 湲곗〈 ?λ퉬 ?댁젣
        curEquip = Instantiate(data.equipPrefab, equipParent).GetComponent<Equip>(); // ?덈줈???λ퉬 ?앹꽦 諛??μ갑
    }

    /// <summary>
    /// ?꾩옱 ?λ퉬瑜??댁젣?섎뒗 硫붿꽌??
    /// </summary>
    public void UnEquip()
    {
        if (curEquip != null)
        {
            Destroy(curEquip.gameObject); // ?꾩옱 ?μ갑???λ퉬 ?ㅻ툕?앺듃 ??젣
            curEquip = null;
        }
    }

    /// <summary>
    /// 怨듦꺽 ?낅젰??泥섎━?섎뒗 硫붿꽌??
    /// </summary>
    /// <param name="context">?낅젰 而⑦뀓?ㅽ듃</param>
    public void OnAttackInput(InputAction.CallbackContext context)
    {
        // 怨듦꺽 ?낅젰???섑뻾?섏뿀?쇰ŉ, ?꾩옱 ?μ갑???λ퉬媛 ?덇퀬, ?뚮젅?댁뼱媛 ?쒖젏??議곗옉?????덈뒗 ?곹깭?쇰㈃
        if (context.phase == InputActionPhase.Performed && curEquip != null && controller.canLook)
        {
            curEquip.OnAttackInput(); // ?λ퉬??怨듦꺽 硫붿꽌???몄텧
        }
    }
}