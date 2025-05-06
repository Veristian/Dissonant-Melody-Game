using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : Environment
{
    public Transform checkpointLocation {get; set;}   
    public GameObject nextCheckpoint;
    private float closestDistance;
    private float distance;
    protected override void PlayerEnterTrigger()
    {
        LevelManager.Instance.currentCheckpoint = this;

        if (nextCheckpoint == null)
        {
            closestDistance = Mathf.Infinity;

            foreach (Checkpoint cp in FindObjectsOfType<Checkpoint>())
            {

                if (cp.transform.position.x > transform.position.x)
                {
                    distance = cp.transform.position.x - transform.position.x;

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        nextCheckpoint = cp.gameObject;
                    }
                }
            }

            if (nextCheckpoint != null)
            {
                LevelManager.Instance.nextCheckpoint = nextCheckpoint.GetComponent<Checkpoint>();
            }
            else
            {
                LevelManager.Instance.nextCheckpoint = null;
            }

        }
        else
        {
            LevelManager.Instance.nextCheckpoint = nextCheckpoint.GetComponent<Checkpoint>();
        }
    }
    

    
}
