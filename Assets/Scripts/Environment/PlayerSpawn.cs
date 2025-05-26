using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : Checkpoint
{
    [SerializeField] private GameObject PlayerPrefab;

    private void Start()
    {
        PlayerMovement existingPlayer = FindObjectOfType<PlayerMovement>();

        if (existingPlayer == null)
        {
            // No player exists, so instantiate a new one at the spawn point.
            Instantiate(PlayerPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            // Player already exists, move them to the spawn point.
            existingPlayer.transform.position = transform.position;
        }
    }
}
