using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBox : Environment
{
    protected override void PlayerEnterTrigger()
    {
        playerMovement.KnockDirection = new Vector3(playerMovement.transform.position.x - _collider2D.ClosestPoint(transform.position).x,playerMovement.transform.position.y - _collider2D.ClosestPoint(transform.position).y).normalized;
        LevelManager.Instance.player = playerMovement;
        LevelManager.Instance.OnDeathEvent.Invoke();
        
    }
}
