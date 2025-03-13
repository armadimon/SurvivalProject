using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle,
    Wander,
    Attack,
}

public class AIEntity : MonoBehaviour
{
    [Header("Stats")]
    public float health;            // 체력
    public float walkSpeed;         // 걷기 속도
    public float runSpeed;          // 뛰기 속도
    public ItemData[] dropOnDeath;  // 사망 시 드롭할 아이템 목록

    [Header("AI")]
    private NavMeshAgent agent;    // 네비게이션 에이전트 (이동 제어)
    private NavMeshPath path;      // 경로 계산을 위한 NavMeshPath
    private AIState aiState;       // 현재 AI 상태

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
    public float detectDistance;   // 플레이어 감지 거리
    public float fieldOfView = 120f; // NPC의 시야각

    private Animator animator;                // 애니메이터
    private SkinnedMeshRenderer[] meshRenderers; // 캐릭터의 스킨 메쉬 렌더러 (피격 효과용)

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(); // 캐릭터의 메쉬 렌더러 가져오기
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerDistance = Vector3.Distance(transform.position, CharacterManager.Instance.Player.transform.position);
        // 이동 여부를 애니메이터에 전달 (Idle 상태 제외)
        animator.SetBool("Moving", aiState != AIState.Idle);

        // 현재 AI 상태에 따라 동작 분기
        switch (aiState)
        {
            case AIState.Idle:
            case AIState.Wander:
                PassiveUpdate(); // 감지 및 배회 로직 실행
                break;
            case AIState.Attack:
                AttackingUpdate(); // 전투 로직 실행
                break;
        }
    }

    private void AttackingUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void SetState(AIState _aIState)
    {
        aiState = _aIState;

        switch (aiState)
        {
            case AIState.Idle:
                agent.isStopped = true;
                break;
            case AIState.Wander:
                agent.isStopped = false;
                break;
            case AIState.Attack:
                agent.speed = runSpeed;
                agent.isStopped = true;
                break;
        }
        animator.speed = agent.speed / walkSpeed;
    }

    void PassiveUpdate()
    {
        path = new NavMeshPath();
        agent.CalculatePath(CharacterManager.Instance.Player.transform.position, path);

        if (path.status != NavMeshPathStatus.PathComplete)
        {
            SetState(AIState.Wander);
            return;
        }

        if (aiState == AIState.Wander && agent.remainingDistance < 0.1f)
        {
            SetState(AIState.Idle);
            Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime));
        }

        if (playerDistance < detectDistance)
        {
            SetState(AIState.Attack);
        }
    }

    void WanderToNewLocation()
    {
        if (aiState != AIState.Idle) return;

        SetState(AIState.Wander);

        Vector3 targetLocation = GetWanderLocation();

        if (!NavMesh.SamplePosition(targetLocation, out NavMeshHit hit, maxWanderDistance, NavMesh.AllAreas)) return;

        agent.areaMask &= ~(1 << NavMesh.GetAreaFromName("HighCostArea"));
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
}
