using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RyuuAnimation : MonoBehaviour
{
    private Animator _animator;
    public GameObject Ryuu;
    public bool Appearance;
    // Start is called before the first frame update
    void Start()
    {
        _animator = Ryuu.GetComponent<Animator>();
        
        if (Appearance)
        {
            _animator.SetBool("Idle", true);
        }
    }

    public void RyuuAppear()
    {
        _animator.SetTrigger("Appear");
    }

    public void RyuuIdle()
    {
        _animator.SetBool("Idle", true);
    }

    public void RyuuDisappear()
    {
        _animator.SetBool("Idle", false);
        _animator.SetTrigger("Disappear");
    }

    public void RyuuInvisible()
    {
        Ryuu.SetActive(false);
    }


}
