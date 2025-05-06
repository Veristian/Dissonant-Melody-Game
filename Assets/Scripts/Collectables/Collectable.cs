using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    bool canBePicked = true;
    protected SpriteRenderer _spriteRenderer;
    protected Collider2D _collider2D;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider2D = GetComponent<Collider2D>();
    }

    // Contains the logic of the colletable 
    private void CollectLogic()
    {
        if (!canBePicked)
        {
            return;
        }        
       
        Collect(); 
        DisableCollectable();       
    }

    #region Overrideable
    // Override to add custom colletable behaviour
    protected virtual void Collect()
    {
        //what happens when collected
    }

    // Disable the spriteRenderer and collider of the Collectable Override to change logic
    protected virtual void DisableCollectable()
    {
        _collider2D.enabled = false;
        _spriteRenderer.enabled = false;

        //to add custom disable behavior        
    }
    
    // Override to add custom reset behaviour
    protected virtual void ResetCollectable()
    {
        //To reset collectable if needed
    }
    #endregion

    #region Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CollectLogic();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ResetCollectable();
    }

    #endregion
}

