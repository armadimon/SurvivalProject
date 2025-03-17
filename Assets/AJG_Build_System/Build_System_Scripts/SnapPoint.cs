using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    public float offsetDistance = 1.0f;
    public List<Quaternion> validRotations = new List<Quaternion>();
    private Quaternion baseRotation;

    private bool isAttached = false; // 중복 실행 방지 플래그
    private Transform parentObject; // 부모 오브젝트 자동 설정
    // private Rigidbody _rigidbody;
    
    private void Start()
    {
        // _rigidbody = transform.root.GetComponent<Rigidbody>();
        baseRotation = transform.rotation;
        List<Vector3> allRotations = new List<Vector3>()
        {
            new Vector3(0, 0, 90),
            new Vector3(0, 0, -90),
            new Vector3(90, 0, 0),
            new Vector3(-90, 0, 0),
            new Vector3(0, 90, 0),
            new Vector3(0, -90, 0)
        };

        Vector3 BaseDirection = Vector3.zero;

        if (name.Contains("Forward"))
        {
            transform.localPosition = new Vector3(0, 0, offsetDistance);
            BaseDirection = Vector3.forward;
        }
        else if (name.Contains("Backward"))
        {
            transform.localPosition = new Vector3(0, 0, -offsetDistance);
            BaseDirection = Vector3.back;
        }
        else if (name.Contains("Left"))
        {
            transform.localPosition = new Vector3(-offsetDistance, 0, 0);
            BaseDirection = Vector3.left;
        }
        else if (name.Contains("Right"))
        {
            transform.localPosition = new Vector3(offsetDistance, 0, 0);
            BaseDirection = Vector3.right;
        }


        List<Vector3> directions = new List<Vector3>();

        if (Mathf.Abs(BaseDirection.x) > 0.9f) // X축 정렬이면 YZ 평면 사용
        {
            directions.Add(Vector3.up);
            directions.Add(Vector3.down);
            directions.Add(Vector3.forward);
            directions.Add(Vector3.back);
        }
        else if (Mathf.Abs(BaseDirection.y) > 0.9f) // Y축 정렬이면 XZ 평면 사용
        {
            directions.Add(Vector3.right);
            directions.Add(Vector3.left);
            directions.Add(Vector3.forward);
            directions.Add(Vector3.back);
        }
        else // Z축 정렬이면 XY 평면 사용
        {
            directions.Add(Vector3.right);
            directions.Add(Vector3.left);
            directions.Add(Vector3.up);
            directions.Add(Vector3.down);
        }

        validRotations.Clear();
        validRotations.Add(Quaternion.LookRotation(Vector3.up, BaseDirection));
        foreach (Vector3 dir in directions)
        {
            Quaternion rotation = Quaternion.LookRotation(BaseDirection, dir);
            validRotations.Add(rotation);
        }
        parentObject = transform.root; // 최상위 부모 오브젝트 가져오기
    }

    public void ApplyRotation(int index)
    {
        if (validRotations.Count > 0)
        {
            Quaternion rotationToApply = validRotations[index];
            transform.rotation = baseRotation * rotationToApply;
        }
    }
    // private void OnTriggerEnter(Collider other)
    // {
    //     LayerMask snapLayer = LayerMask.GetMask("SnapPoint");
    //     if ((snapLayer.value & (1 << other.gameObject.layer)) == 0)
    //     {
    //         Debug.Log(other.gameObject.name + " has no snap point");
    //         // 스냅 대상 레이어가 아니면 무시
    //         return;
    //     }
    //     if (isAttached)
    //         return; // 이미 붙여졌으면 처리 안함
    //
    //     SnapPoint otherSnap = other.GetComponent<SnapPoint>();
    //     if (otherSnap != null)
    //     {
    //         AttachObjects(otherSnap);
    //         isAttached = true; // 붙였으므로 플래그를 true로 설정
    //     }
    // }
    // private void OnTriggerExit(Collider other)
    // {
    //
    //         isAttached = false;
    // }
    
    private void AttachObjects(SnapPoint targetSnap)
    {
        // 1️⃣ 부모 오브젝트 가져오기
        Transform objectToMove = transform.root; // 내가 들고 있는 오브젝트 A 
        Transform targetObject = targetSnap.transform.root; // B의 부모 오브젝트
        
        // 3️⃣ SnapPoint끼리 위치 정렬
        Vector3 offset = targetSnap.transform.position - transform.position;
        objectToMove.position += offset;

        // 4️⃣ SnapPoint 방향 정렬 (A가 B의 SnapPoint와 마주보도록 회전)
        Quaternion rotationOffset = Quaternion.FromToRotation(transform.forward, -targetSnap.transform.forward);
        objectToMove.rotation = rotationOffset * objectToMove.rotation;
    }
}

