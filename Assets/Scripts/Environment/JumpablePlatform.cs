using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class JumpablePlatform : Environment
{
    [SerializeField] private PlatformEffector2D platformEffector2D;
    [SerializeField] private float secondsToActivation = 0.5f;
    [SerializeField] private LayerMask playerLayer;

    private LayerMask nullMask;
    protected override void PlayerOnTop()
    {
        playerMovement.jumpablePlatform = this;
        playerMovement.OnDropablePlatform = true;
        
    }
    protected override void PlayerExitCollider()
    {
        playerMovement.jumpablePlatform = null;
        playerMovement.OnDropablePlatform = false;
    }

    public void LetPlayerDrop()
    {
        platformEffector2D.colliderMask = nullMask;
        StartCoroutine(ResetEffector(secondsToActivation));
    }
    private IEnumerator ResetEffector(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        platformEffector2D.colliderMask = playerLayer;
    }
}
