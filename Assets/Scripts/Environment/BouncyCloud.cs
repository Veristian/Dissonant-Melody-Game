using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyCloud : Environment
{
    [Header("Settings")]
    [SerializeField] private float bounceStartingVelocity = 20;
    [SerializeField] private float upwardVelocity = 20;
    private Vector2 direction;
    [SerializeField] private GameObject intendedDirectionPointer;
    [SerializeField] [Range(0,1)]private float useIntendedDirectionThreshold = 0.1f;

    private Vector3 intendedDirection;
    private void Start()
    {
        unityEvent.AddListener(() => playerMovement.ApplyVelocity( new Vector2 (0,upwardVelocity) + bounceStartingVelocity * direction, true));
        if (intendedDirectionPointer != null)
        {
            intendedDirection = (intendedDirectionPointer.transform.position - transform.position).normalized;

        }
    }
    protected override void CollideWithPlayer()
    {
        direction = (playerMovement.transform.position - transform.position).normalized;
        if (Mathf.Abs(Vector3.Dot(direction,intendedDirection)) >= (1 - useIntendedDirectionThreshold) && intendedDirectionPointer != null)
        {
            direction = intendedDirection;
        }
        
        unityEvent.Invoke();
    }
    
    protected override void CollideWithBullet()
    {
        direction = (noteProjectile.transform.position - transform.position).normalized;
        direction = (playerMovement.transform.position - transform.position).normalized;
        if (Mathf.Abs(Vector3.Dot(direction,intendedDirection)) >= (1 - useIntendedDirectionThreshold) && intendedDirectionPointer != null)
        {
            direction = intendedDirection;
        }
        noteProjectile.GetComponent<Rigidbody2D>().velocity = direction * noteProjectile.GetComponent<Rigidbody2D>().velocity.magnitude;
    }
    
}
