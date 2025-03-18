using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// AI의 상태를 나타내는 열거형
public enum AIState
{
    Idle,       // 대기 상태
    Wandering,  // 배회 상태 (랜덤 이동)
    Attacking   // 공격 상태
}

// NPC 클래스: 네비게이션, 배회, 전투 기능을 수행
public class AIEntity : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public float health;            // 체력
    public float walkSpeed;         // 걷기 속도
    public float runSpeed;          // 뛰기 속도
    public ItemData[] dropOnDeath;  // 사망 시 드롭할 아이템 목록

    [Header("AI")]
    private NavMeshAgent agent;     // 네비게이션 에이전트 (이동 제어)
    private NavMeshPath path;       // 경로 계산을 위한 NavMeshPath
    public float detectDistance;    // 플레이어 감지 거리
    private AIState aiState;        // 현재 AI 상태
    public LayerMask buildObject;   // 건물 레이어
    bool player = false;

    [Header("Wandering")]
    public float minWanderDistance;  // 배회 시 최소 이동 거리
    public float maxWanderDistance;  // 배회 시 최대 이동 거리
    public float minWanderWaitTime;  // 배회 대기 최소 시간
    public float maxWanderWaitTime;  // 배회 대기 최대 시간

    [Header("Combat")]
    public float damage;             // 공격력
    public float attackRate;         // 공격 속도 (공격 간격)
    private float lastAttackTime;    // 마지막 공격 시간
    public float attackDistance;     // 공격 거리

    private float playerDistance;    // 플레이어와의 거리
    public float fieldOfView = 120f; // NPC의 시야각

    private Animator animator;                  // 애니메이터
    private SkinnedMeshRenderer[] meshRenderers; // 캐릭터의 스킨 메쉬 렌더러 (피격 효과용)

    private float defaultDetectDistance;            // 기본 감지 거리
    public float nightDetectDistanceMultiplier;    // 밤에 감지 거리

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();           // 네비게이션 에이전트 가져오기
        animator = GetComponent<Animator>();            // 애니메이터 가져오기
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(); // 캐릭터의 메쉬 렌더러 가져오기

        // NavMesh 위의 가장 가까운 지점으로 이동 (Terrain에서 오류 방지)
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
        {
            transform.position = (hit.position);
        }
    }

    void Start()
    {
        defaultDetectDistance = detectDistance; // 기본 감지 거리 저장
        DayNightCycle.OnNightStateChanged += SetDetectDistanceForNight;

        SetState(AIState.Wandering); // 시작할 때 배회 상태로 설정
    }

    void Update()
    {
        // 플레이어와의 거리 계산
        playerDistance = Vector3.Distance(transform.position, CharacterManager.Instance.Player.transform.position);
        // 이동 여부를 애니메이터에 전달 (Idle 상태 제외)
        animator.SetBool("Moving", aiState != AIState.Idle);

        // 현재 AI 상태에 따라 동작 분기
        switch (aiState)
        {
            case AIState.Idle:
            case AIState.Wandering:
                PassiveUpdate(); // 감지 및 배회 로직 실행
                break;
            case AIState.Attacking:
                AttackingUpdate(); // 전투 로직 실행
                break;
        }
    }

    /// <summary>
    /// AI 상태 변경 함수
    /// </summary>
    /// <param name="state">변경할 AI 상태</param>
    public void SetState(AIState state)
    {
        aiState = state;

        switch (aiState)
        {
            case AIState.Idle:
                agent.speed = walkSpeed;
                agent.isStopped = true; // 이동 중지
                break;
            case AIState.Wandering:
                agent.speed = walkSpeed;
                agent.isStopped = false; // 이동 시작
                break;
            case AIState.Attacking:
                agent.speed = runSpeed;
                agent.isStopped = true; // 공격 시 이동 정지
                break;
        }

        // 애니메이션 속도를 걷기 속도 기준으로 조절
        animator.speed = agent.speed / walkSpeed;
    }

    // 플레이어 감지 및 배회 관련 업데이트
    void PassiveUpdate()
    {
        // 현재 위치에서 경로를 미리 계산
        path = new NavMeshPath();
        agent.CalculatePath(CharacterManager.Instance.Player.transform.position, path);

        if (path.status != NavMeshPathStatus.PathComplete)
        {
            SetState(AIState.Wandering);
            return;
        }

        // 배회 중이며 목표 지점에 도착했을 경우 일정 시간 후 새로운 위치로 이동
        if (aiState == AIState.Wandering && agent.remainingDistance < 0.3f)
        {
            SetState(AIState.Idle);
            Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime));
        }

        // 플레이어가 감지 거리 내에 있으면 공격 상태로 전환
        if (playerDistance < detectDistance && path.status != NavMeshPathStatus.PathPartial)
        {
            SetState(AIState.Attacking);
        }
        else
        {
            // 플레이어를 찾지 못할 경우, 감지 거리 내에 건물이 있는지 확인
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectDistance, buildObject);
            if (hitColliders.Length > 0)
            {
                SetState(AIState.Attacking);
            }
        }
    }

    // 일정 시간 후 새로운 배회 위치로 이동
    void WanderToNewLocation()
    {
        if (aiState != AIState.Idle) return;

        SetState(AIState.Wandering);

        Vector3 targetLocation = GetWanderLocation();

        if (!NavMesh.SamplePosition(targetLocation, out NavMeshHit hit, maxWanderDistance, NavMesh.AllAreas)) return;

        agent.areaMask &= ~(1 << NavMesh.GetAreaFromName("SettlementArea"));
        agent.SetDestination(hit.position);
    }

    // 랜덤한 배회 위치 반환
    Vector3 GetWanderLocation()
    {
        NavMeshHit hit;
        int i = 0;
        do
        {
            // 현재 위치에서 랜덤한 방향으로 이동할 위치 설정
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)),
                out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
        }
        while (Vector3.Distance(transform.position, hit.position) < detectDistance && i < 30);

        return hit.position;
    }

    // 공격 관련 업데이트
    void AttackingUpdate()
    {
        if (playerDistance < detectDistance)
        {
            player = true;
            // 플레이어가 공격 거리 내에 있고 시야 내에 있는지 확인
            if (playerDistance < attackDistance && IsPlayerInFieldOfView())
            {
                agent.isStopped = true; // 공격 시 이동 정지

                // 공격 가능 시간인지 체크
                if (Time.time - lastAttackTime > attackRate)
                {
                    lastAttackTime = Time.time; // 마지막 공격 시간 갱신
                    animator.speed = 1f; // 애니메이션 속도 설정
                    animator.SetTrigger("Attack"); // 공격 애니메이션 실행
                }

            }
            else
            {
                path = new NavMeshPath();

                // HighCostArea를 제외한 영역으로 경로 계산
                agent.areaMask = NavMesh.GetAreaFromName("SettlementArea");

                agent.CalculatePath(CharacterManager.Instance.Player.transform.position, path);

                // HighCostArea로 인해 경로가 완전하지 않다면 배회 상태로 전환
                if (path.status != NavMeshPathStatus.PathComplete)
                {
                    SetState(AIState.Idle);
                    Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime));
                }
                else
                {
                    // 플레이어가 감지 거리 내에 있다면 따라가기
                    agent.isStopped = false;
                    agent.SetDestination(CharacterManager.Instance.Player.transform.position);
                }
            }
        }
        else
        {
            player = false;
            // 플레이어를 찾지 못할 경우, 감지 거리 내에 건물이 있는지 확인
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectDistance, buildObject);
            if (hitColliders.Length > 0)
            {
                // 가장 가까운 건물로 이동
                agent.isStopped = false;
                agent.SetDestination(hitColliders[0].transform.position);

                // 공격 가능 시간인지 체크
                if (Time.time - lastAttackTime > attackRate)
                {
                    lastAttackTime = Time.time; // 마지막 공격 시간 갱신
                    animator.speed = 1f; // 애니메이션 속도 설정
                    animator.SetTrigger("Attack"); // 공격 애니메이션 실행
                }
            }
            else
            {
                SetState(AIState.Idle);
                Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime));
            }
        }
    }

    // 애니메이션 이벤트로 호출될 메서드
    public void DealDamage()
    {
        if (!player) // 플레이어가 아닌 경우 (건물을 공격)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackDistance, buildObject);
            BuildObject closestBuildObject = null;
            float closestDistance = Mathf.Infinity;

            // 감지된 모든 건물 중 가장 가까운 건물 찾기
            foreach (var hitCollider in hitColliders)
            {
                BuildObject buildObjectComponent = hitCollider.GetComponent<BuildObject>();
                if (buildObjectComponent != null)
                {
                    float distance = Vector3.Distance(transform.position, buildObjectComponent.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestBuildObject = buildObjectComponent;
                    }
                }
            }

            // 가장 가까운 건물에 데미지 적용
            if (closestBuildObject != null)
            {
                closestBuildObject.TakeDamage((int)damage);
            }
        }
        else // 플레이어를 공격하는 경우
        {
            CharacterManager.Instance.Player.condition.GetComponent<IDamageable>().TakeDamage(damage);
        }
    }


    // 플레이어가 NPC의 시야 내에 있는지 확인
    bool IsPlayerInFieldOfView()
    {
        Vector3 directionToPlayer = CharacterManager.Instance.Player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < fieldOfView * 0.5f;
    }

    // 데미지를 받았을 때 처리
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }

        StartCoroutine(DamageFlash()); // 피격 효과
    }

    // 사망 처리
    void Die()
    {
        // 사망 시 아이템 드롭
        foreach (var item in dropOnDeath)
        {
            Instantiate(item.dropPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    // 피격 시 빨갛게 변했다가 다시 원래 색으로 복귀
    IEnumerator DamageFlash()
    {
        // 모든 스킨 메쉬 렌더러에 대해 색상 변경
        foreach (var renderer in meshRenderers)
        {
            renderer.material.color = new Color(1.0f, 0.6f, 0.6f);  // 빨갛게 변경
        }
        yield return new WaitForSeconds(0.1f);
        // 원래 색상으로 변경
        foreach (var renderer in meshRenderers)
        {
            renderer.material.color = Color.white;
        }
    }

    private void OnDestroy()
    {
        // 이벤트 해제
        DayNightCycle.OnNightStateChanged -= SetDetectDistanceForNight;
    }

    private void SetDetectDistanceForNight(bool isNight)
    {
        // 밤에는 감지 거리를 늘림
        detectDistance = isNight ? defaultDetectDistance * nightDetectDistanceMultiplier : defaultDetectDistance;
    }
}
