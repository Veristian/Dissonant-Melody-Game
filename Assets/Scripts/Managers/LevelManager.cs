using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : Singleton<LevelManager>
{
    public UnityEvent OnDeathEvent;
    public UnityEvent OnPlayerSpawned;
    public UnityEvent OnPlayerSpawn;

    public UnityEvent OnCheckpointReachedEvent;
    public UnityEvent SpawnNewPlayer;

    public Checkpoint currentCheckpoint;
    public Checkpoint nextCheckpoint;

    public PlayerMovement player;

    [SerializeField] 
    [Range(0.5f,1.5f)]private float secondsToRespawn;
    
    //reset
    private Button[] buttons;
    private DisappearingPlatform[] disappearingPlatforms;
    private NoteProjectile[] noteProjectiles;

    private void Start()
    {
        if (OnDeathEvent == null)
        {
            OnDeathEvent = new UnityEvent();
        }
        if (OnCheckpointReachedEvent == null)
        {
            OnCheckpointReachedEvent = new UnityEvent();
        }
        if (OnPlayerSpawned == null)
        {
            OnPlayerSpawned = new UnityEvent();
        }
        if (OnPlayerSpawn == null)
        {
            OnPlayerSpawn = new UnityEvent();
        }
        if (SpawnNewPlayer == null)
        {
            SpawnNewPlayer = new UnityEvent();
        }
        buttons = FindObjectsByType<Button>(0);
        disappearingPlatforms = FindObjectsByType<DisappearingPlatform>(0);
        noteProjectiles = new NoteProjectile[0];
        noteProjectiles = FindObjectsByType<NoteProjectile>(0);

        OnDeathEvent.AddListener(() => PlayerDeath());
        OnDeathEvent.AddListener(() => ResetButton());
        OnDeathEvent.AddListener(() => ResetPlatform());
        OnDeathEvent.AddListener(() => DeleteShots());
        

        OnPlayerSpawned.AddListener(() => PlayerCanMoveAfterSpawn());
    }

    private void PlayerDeath()
    {
        if (currentCheckpoint != null && player != null)
        {
            player.Death = true;
            StartCoroutine(Teleport(secondsToRespawn));
        }
    }

    private IEnumerator Teleport(float seconds)
    {
        yield return new WaitForSeconds(seconds); 
        player.transform.position = currentCheckpoint.transform.position;
    }

    private void PlayerCanMoveAfterSpawn()
    {
        player.Death = false;
    }

    private void ResetButton()
    {
        foreach(Button button in buttons)
        {
            button.Activated = false;
            button.OffSprite();
            button.StopAllCoroutines();
        }
    }
    private void ResetPlatform()
    {
        foreach(DisappearingPlatform disappearingPlatform in disappearingPlatforms)
        {
            disappearingPlatform.ActivationState = disappearingPlatform.startState;
        }
    }
    private void DeleteShots()
    {
        foreach(NoteProjectile noteProjectile in noteProjectiles)
        {
            Destroy(noteProjectile);
        }
    }



    
    
    
}
