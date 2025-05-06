using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingPlatform : Environment
{
    [Header("Settings")]
    [SerializeField] private bool auto;
    [SerializeField] private float autoSwitchTime;

    [SerializeField] private bool disappearAutomatically;
    [SerializeField] private float disappearTime;
    [SerializeField] private bool appearAutomatically;
    [SerializeField] private float appearTime;

    [SerializeField] public bool startState;

    [SerializeField] private float startTimeOffset;
    [SerializeField] private LayerMask playerLayer;

    public bool ActivationState {private get;set;}

    [Header("Sprites")]
    
    [SerializeField] private Sprite OnSprite;
    [SerializeField] private Sprite OffSprite;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    private float timer;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        ActivationState = startState;
        Appear(ActivationState);
        timer = startTimeOffset;
    }

    private void FixedUpdate()
    {
        if (auto || disappearAutomatically || appearAutomatically)
        {
            timer += Time.fixedDeltaTime;
        }
        else
        {
            timer = startTimeOffset;
        }
        
        if (auto)
        {
            if (timer > autoSwitchTime)
            {
                timer = 0;
                ActivationState = !ActivationState;
            }

        }
        else
        {

            if (disappearAutomatically && ActivationState)
            {
                if (timer > disappearTime)
                {
                    timer = 0;
                    ActivationState = false;
                }
            }
            if (appearAutomatically && !ActivationState)
            {
                if (timer > appearTime)
                {
                    timer = 0;
                    ActivationState = true;
                }
            }
        }
        Appear(ActivationState);
    }
    private void Appear(bool state)
    {
        if (state)
        {
            if (!Physics2D.OverlapBox(boxCollider.bounds.center, boxCollider.bounds.size, 0f, playerLayer))
            {
                spriteRenderer.sprite = OnSprite;
                boxCollider.enabled = true;
            }

        }
        else
        {
            spriteRenderer.sprite = OffSprite;
            boxCollider.enabled = false;

        }
    }

    #region Trigger Events
    public void TurnAuto(bool state)
    {
        auto = state;
    }
    public void TurnDisappear(bool state)
    {
        disappearAutomatically = state;
    }
    public void TurnAppear(bool state)
    {
        appearAutomatically = state;
    }
    public void SetState(bool state)
    {
        ActivationState = state;
    }
    public void FlipState()
    {
        ActivationState = !ActivationState;
    }
    #endregion


    


    
}
