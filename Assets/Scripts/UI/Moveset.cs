using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Moveset : MonoBehaviour
{
    private Animator _animator;
    public bool open;
    public bool openIdle;
    public bool closeIdle;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (!open)
            {
                open = true;
                _animator.SetTrigger("Open");
                SetAnimation("Open_Idle", false);
                SetAnimation("Close_Idle", false);
                OpenMoveset();
            }

            else if (open)
            {
                open = false;
                _animator.SetTrigger("Close");
                SetAnimation("Open_Idle", false);
                SetAnimation("Close_Idle", false);
                CloseMoveset();
            }
        }
    }

    public void OpenMoveset()
    {
        SetAnimation("Open_Idle", true);
        SetAnimation("Close_Idle", false);
    }

    public void CloseMoveset()
    {
        SetAnimation("Close_Idle", true);
        SetAnimation("Open_Idle", false);
    }
    private void SetAnimation(string trigger, bool action)
    {
        _animator.SetBool(trigger, action);
    }
}
