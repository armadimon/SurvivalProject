using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public BuildObject buildObject;
    public Collider collider;
    public bool isActive { get; set; }
    
    public float upHeight = 1.5f;
    public float speed = 5f;
    public float resetDelay = 2f;
    private Vector3 initialPosition;
    private bool isUp = false;
    public LayerMask layerMask;
    
    private void Start()
    {
        isActive = true;
    }
    
        public void Activate()
        {
            isActive = true;
            if (!isUp)
            {
                isUp = true;
                StopAllCoroutines();
                StartCoroutine(MoveSpike(initialPosition + Vector3.up * upHeight, true));
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if ((layerMask.value & (1 << other.gameObject.layer)) != 0)
            {
                AIEntity entity = other.gameObject.GetComponent<AIEntity>();
                if (entity != null)
                {
                    entity.TakeDamage(10);
                }
            }
        }
        
        private IEnumerator MoveSpike(Vector3 targetPosition, bool goingUp)
        {
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
                yield return null;
            }

            if (goingUp)
            {
                yield return new WaitForSeconds(resetDelay);
                StartCoroutine(MoveSpike(initialPosition, false));
            }
            else
            {
                isUp = false;
            }
        }
        
        void OnEnable()
        {
            if (buildObject != null)
            {
                buildObject.OnSetChanged += OnSpikeTrap;
            }
            else
            {
                Debug.LogError("build object is null");
            }
        }

        void OnDisable()
        {
            if (buildObject != null)
            {
                buildObject.OnSetChanged -= OnSpikeTrap;
            }
        }

        public void OnSpikeTrap()
        {
            initialPosition = buildObject.transform.position;
            collider.enabled = true;
        }
    
}
