using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

public abstract class Environment : MonoBehaviour
{
    protected UnityEvent unityEvent;
    protected PlayerMovement playerMovement;
    protected NoteProjectile noteProjectile;

    protected Collider2D _collider2D;
    protected Collision2D _collision2D;


    private void Awake()
    {
        if (unityEvent == null)
        {
            unityEvent = new UnityEvent();
        }
 
    }
    protected virtual void CollideWithPlayer()
    {

    }

    protected virtual void CollideWithBullet()
    {

    }

    protected virtual void PlayerEnterTrigger()
    {
        
    }
    protected virtual void PlayerExitTrigger()
    {
        
    }

    protected virtual void PlayerOnTop()
    {

    }
    protected virtual void PlayerExitCollider()
    {

    }
    protected virtual void BulletEnterTrigger()
    {
        
    }
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponentInParent<PlayerMovement>())
        {
            _collision2D = collision;
            playerMovement = _collision2D.gameObject.GetComponentInParent<PlayerMovement>();
            CollideWithPlayer();
        }
        else if (collision.gameObject.GetComponent<NoteProjectile>())
        {
            _collision2D = collision;
            noteProjectile = collision.gameObject.GetComponent<NoteProjectile>();
            CollideWithBullet();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponentInParent<PlayerMovement>())
        {
            if (Vector2.Dot(collision.GetContact(0).normal, transform.up) == -1)
            {
                _collision2D = collision;
                playerMovement = _collision2D.gameObject.GetComponentInParent<PlayerMovement>();
                PlayerOnTop();
                
            }

        }
         

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponentInParent<PlayerMovement>())
        {
            _collision2D = collision;
            playerMovement = _collision2D.gameObject.GetComponentInParent<PlayerMovement>();
            PlayerExitCollider();
            
    
        }
         

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerMovement>())
        {
            _collider2D = other;
            playerMovement = _collider2D.gameObject.GetComponentInParent<PlayerMovement>();
            PlayerEnterTrigger();
        }
        else if (other.gameObject.GetComponent<NoteProjectile>())
        {
            _collider2D = other;
            noteProjectile = other.gameObject.GetComponent<NoteProjectile>();
            BulletEnterTrigger();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerMovement>())
        {
            _collider2D = other;
            playerMovement = _collider2D.gameObject.GetComponentInParent<PlayerMovement>();
            PlayerExitTrigger();
        }
     
    }


}
