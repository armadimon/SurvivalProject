using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate;        // 怨듦꺽 ?쒓컙
    public float attackDistance;    // 怨듦꺽 嫄곕━
    public float useStamina;        // ?ㅽ깭誘몃꼫 ?ъ슜??

    private bool attacking = false;         // 怨듦꺽 以묒씤吏 ?뺤씤

    [Header("Resource Gathering")]
    public bool doesGatherResource; // ?먯썝??梨꾩쭛?섎뒗吏 ?뺤씤

    [Header("Combat")]
    public bool doseDealDamage;     // ?곕?吏瑜?二쇰뒗吏 ?뺤씤
    public float damage;            // ?곕?吏

    private Animator animator;
    private Camera cam;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        cam = GetComponent<Camera>();
    }

    public override void OnAttackInput()
    {
        if (!attacking)
        {
            //if (CharacterManager.Instance.Player.condition.UseStamina(useStamina))
            //{
            //    attacking = true;
            //    animator.SetTrigger("Attack");      // 怨듦꺽 ?좊땲硫붿씠???ㅽ뻾
            //    Invoke("OnCanAttack", attackRate);
            //}

        }
    }

    public void OnCanAttack()
    {
        attacking = false;
    }

    public void OnHit()
    {
        // 怨듦꺽???덉쓣 ???덉씠罹먯뒪?몃? ?듯빐 異⑸룎???ㅻ툕?앺듃瑜??뺤씤
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));    // ?붾㈃ 以묒븰??湲곗??쇰줈 ?덉씠 諛쒖궗

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackDistance))
        {
            // ?곕?吏瑜?二쇰뒗 寃쎌슦
            if (doesGatherResource && hit.collider.TryGetComponent(out Resource resource))
            {
                resource.Gather(hit.point, hit.normal);
            }
            else
            {
                if (!doesGatherResource && hit.collider.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(damage); // ?곕?吏 二쇨린
                }
            }
        }
    }
}
