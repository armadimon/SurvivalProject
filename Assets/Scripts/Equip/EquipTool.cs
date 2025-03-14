using UnityEngine;
public class EquipTool : Equip
{
    public float attackRate;        // 공격 시간
    private bool attacking;         // 공격 중인지 확인
    public float attackDistance;    // 공격 거리
    public float useStamina;        // 스태미너 사용량

    [Header("Resource Gathering")]
    public bool doesGatherResource; // 자원을 채집하는지 확인

    [Header("Combat")]
    public bool doesDealDamage; // 데미지를 주는지 확인
    public int damage;          // 데미지

    private Animator animator;
    private Camera _camera;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        _camera = Camera.main;
    }

    public override void OnAttackInput()
    {
        if (!attacking)
        {
            if (CharacterManager.Instance.Player.condition.UseStamina(useStamina))
            {
                attacking = true;
                animator.SetTrigger("Attack");      // 공격 애니메이션 실행
                Invoke("OnCanAttack", attackRate);
            }

        }
    }
    void OnCanAttack()
    {
        attacking = false;
    }

    public void OnHiT()
    {
        // 공격을 했을 때 레이캐스트를 통해 충돌한 오브젝트를 확인
        Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));    // 화면 중앙을 기준으로 레이 발사

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackDistance))   // 레이캐스트가 충돌한 오브젝트
        {
            // 데미지를 주는 경우
            if (doesGatherResource && hit.collider.TryGetComponent(out Resource resource))
            {
                resource.Gather(hit.point, hit.normal);
            }
            else
            {
                if (!doesGatherResource && hit.collider.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(damage); // 데미지 주기
                }
            }
        }
    }
}