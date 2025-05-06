using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MovingPlatform : Environment
{
    private bool playerOnTop;
    private bool resetOnFirstContact;
    private Vector3 previousLocation;
    private void Start()
    {
        previousLocation = transform.position;
    }
    private void FixedUpdate()
    {
        if (resetOnFirstContact)
        {
            resetOnFirstContact = false;
            previousLocation = transform.position;
        }
        if (playerOnTop)
        {
            playerMovement.gameObject.transform.position += (transform.position - previousLocation);
            previousLocation = transform.position;

        }

    }
    protected override void CollideWithPlayer()
    {
        resetOnFirstContact = true;
    }
    protected override void PlayerOnTop()
    {
        playerOnTop = true;
    }
    protected override void PlayerExitCollider()
    {
        playerOnTop = false;
    }



}
