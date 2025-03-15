using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SnapPoint : MonoBehaviour
{
    public float offsetDistance = 1.0f;
    public List<Quaternion> validRotations = new List<Quaternion>();

    private Quaternion baseRotation;
    private void Start()
    {
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
    }

    public void ApplyRotation(int index)
    {
        if (validRotations.Count > 0)
        {
            Quaternion rotationToApply = validRotations[index];
            transform.rotation = baseRotation * rotationToApply;
        }
    }
}
