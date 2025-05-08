using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DialogueEventAnimation : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlayAnimationTrigger(string animationName)
    {
        if (_animator == null)
        {
            Debug.LogError("Animator component is not assigned in the inspector!");
            return;
        }
        _animator.SetTrigger(animationName); // Set the trigger for the animation
    }
    public void PlayAnimationBool(string animationName)
    {
        if (_animator == null)
        {
            Debug.LogError("Animator component is not assigned in the inspector!");
            return;
        }
        _animator.SetBool(animationName, true); // Set the trigger for the animation
    }
    
     public void StopAnimationBool(string animationName)
    {
        if (_animator == null)
        {
            Debug.LogError("Animator component is not assigned in the inspector!");
            return;
        }
        _animator.SetBool(animationName, false); // Set the trigger for the animation
    }
}
