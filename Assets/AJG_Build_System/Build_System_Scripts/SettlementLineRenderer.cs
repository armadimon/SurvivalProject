using UnityEngine;
using System.Collections.Generic;

public class SettlementLineRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int segments = 30;
    public float raycastHeightOffset = 30f;
    public float raycastDistance = 30f;
    public float boundaryOffset = 0.1f; // 경계선이 콜라이더에서 약간 떨어지도록 하는 값

    public List<Vector3> collisionPoints = new List<Vector3>();
    public bool hasCollided = false;

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        FindCollisionPointsFromRaycast();
        UpdateLineRenderer();
    }

    void FindCollisionPointsFromRaycast()
    {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            Debug.LogError("이 오브젝트에 SphereCollider 컴포넌트가 없습니다.");
            return;
        }

        float radius = sphereCollider.radius * transform.lossyScale.x; // 로컬 스케일 고려

        for (int i = 0; i < segments; i++)
        {
            float angle = i * (360f / segments);
            float radians = angle * Mathf.Deg2Rad;

            // 원 둘레의 점 계산
            float x = radius * Mathf.Cos(radians);
            float z = radius * Mathf.Sin(radians);
            Vector3 circlePoint = transform.position + new Vector3(x, 0f, z);

            // 레이캐스트 시작 지점 (일정 높이 위)
            Vector3 raycastStartPoint = circlePoint + Vector3.up * raycastHeightOffset;

            // 아래 방향으로 레이캐스트
            if (Physics.Raycast(raycastStartPoint, Vector3.down, out RaycastHit hit, raycastDistance, LayerMask.GetMask("Ground")))
            {
                collisionPoints.Add(hit.point + hit.normal * boundaryOffset);
            }
        }
    }

    void UpdateLineRenderer()
    {
        if (collisionPoints.Count > 1)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = collisionPoints.Count;
            lineRenderer.SetPositions(collisionPoints.ToArray());
        }
        else if (collisionPoints.Count == 1)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, collisionPoints[0]);
            lineRenderer.SetPosition(1, collisionPoints[0]);
        }
        else
        {
            lineRenderer.enabled = false;
            lineRenderer.positionCount = 0;
        }
    }
}