using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; 
    [SerializeField] private float smoothTime = 0.3f;
    [SerializeField] private float minHeight = 20f; 

    private Vector3 offset;
    private Vector3 velocity;

    private void Awake()
    {
        offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
            if(target!= null)
        {
            Vector3 targetPosition = target.position + offset;


            float minY = Mathf.Max(targetPosition.y - minHeight, transform.position.y);
            targetPosition.y = minY;

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
           
        }
        
    
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }
}
