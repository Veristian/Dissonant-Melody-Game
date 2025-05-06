using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;
    public bool Idle;
    public bool Jump;
    public bool Walk;
    public bool Run;
    public bool Dash;
    public bool Fall;
    public bool teleport;
    public bool teleport_hold;

    [SerializeField] private float timeToIdleAnim;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        LevelManager.Instance.OnDeathEvent.AddListener(() => Death());

    }

    private void Update()
    {
        SetAnimation("Walk", Walk);
        SetAnimation("Run", Run);
        SetAnimation("Jump", Jump);
        SetAnimation("Dash", Dash);
        SetAnimation("Fall", Fall);
        SetAnimation("Teleport_Hold", teleport_hold);
        if (Idle)
        {
            if (timer > timeToIdleAnim)
            {
                SetAnimation("Idle", false);
                SetAnimation("Jumping_Rope", true);
            }
            else
            {
                SetAnimation("Idle", true);
                SetAnimation("Jumping_Rope", false);
            }
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            SetAnimation("Jumping_Rope", false);
            SetAnimation("Idle", false);

        }
        
    }

    private void SetAnimation(string trigger, bool action)
    {
        _animator.SetBool(trigger, action);
    }
    public void TeleportStart()
    {
        _animator.SetTrigger("Teleport_Start");
    }

    private void Death()
    {
        _animator.SetTrigger("Death");
    }
    private void TriggerDeath()
    {
        _animator.SetTrigger("Spawn");
        LevelManager.Instance.OnPlayerSpawn.Invoke();
    }

    private void TriggerSpawn()
    {
        LevelManager.Instance.OnPlayerSpawned.Invoke();
    }

    
   
}
