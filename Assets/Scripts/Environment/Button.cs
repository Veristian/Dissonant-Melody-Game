using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button : Environment
{
    public bool Activated {get; set;}
    [SerializeField]private bool CanActivateOnStep;
    public enum Type
    {
        Button,
        Lever
    }
    [SerializeField]private Type type;
    [SerializeField]private bool hasExitTime;
    [SerializeField]private float exitTime;

    public UnityEvent OnActivationEvent;
    public UnityEvent OnDeactivationEvent;

    [Header("Sprite")]
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;
    private SpriteRenderer sprite;


    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = offSprite;
        if (OnActivationEvent == null)
        {
            OnActivationEvent = new UnityEvent();
        }
        if (OnDeactivationEvent == null)
        {
            OnDeactivationEvent = new UnityEvent();
        }

        OnDeactivationEvent.AddListener(() => OffSprite());
        OnActivationEvent.AddListener(() => OnSprite());

    }

    public void OffSprite()
    {
        sprite.sprite = offSprite;
    }
    public void OnSprite()
    {
        sprite.sprite = onSprite;
    }


    public IEnumerator ButtonTimer(float seconds) 
    {
        yield return new WaitForSeconds(seconds);
        OnDeactivationEvent.Invoke();
        Activated = false;

    }

    protected override void BulletEnterTrigger()
    {
        switch (type)
        {
            case Type.Button:
                if (!Activated)
                {
                    OnActivationEvent.Invoke();
                    Activated = true;
                    StartCoroutine(ButtonTimer(exitTime));
                }
                break;

            case Type.Lever:
                if (Activated)
                {
                    OnDeactivationEvent.Invoke();
                    Activated = false;
                }
                else
                {
                    OnActivationEvent.Invoke();
                    Activated = true;

                }
                break;
        }
        Destroy(noteProjectile);
    }

    protected override void PlayerEnterTrigger()
    {
        if (CanActivateOnStep)
        {
            switch (type)
            {
                case Type.Button:
                    if (!Activated)
                    {
                        OnActivationEvent.Invoke();
                        Activated = true;                    
                        StartCoroutine(ButtonTimer(exitTime));

                    }
                    break;

                case Type.Lever:
                    if (Activated)
                    {
                        OnDeactivationEvent.Invoke();
                        Activated = false;
                    }
                    else
                    {
                        OnActivationEvent.Invoke();
                        Activated = true;

                    }
                    break;
            }

        }
    }

}
