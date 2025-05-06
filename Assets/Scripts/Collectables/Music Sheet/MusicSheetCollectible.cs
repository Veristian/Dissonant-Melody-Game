using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSheetCollectible : Collectable
{
    [Header("Notes")]
    public float maxTime = 4f;
    public float bpm = 120f;
    public NoteTypeStats[] noteTypeStatsList;

    [Header("Collectable")]
    public float cooldown = 2f;
    private float totalLength;
    private float prevMaxTime;
    private float prevBpm;

    [Header("Sprite")]
    [SerializeField] private Sprite sprite1;
    [SerializeField] private Sprite sprite2;
    [SerializeField] private Sprite sprite3;
    [SerializeField] private Sprite sprite4;
    [SerializeField] private Sprite sprite5;
    private SpriteRenderer spriteRenderer;



    #region Validation
    private void OnValidate()
    {
        if (noteTypeStatsList.Length > 0)
        {
            CalculateTotalNoteTime();
            ValidateTime();
        }
       
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ApplySprite();
    }

    private void ApplySprite()
    {
        if(noteTypeStatsList.Length <= 1)
        {
            spriteRenderer.sprite = sprite1;
        }
        else if(noteTypeStatsList.Length <= 2)
        {
            spriteRenderer.sprite = sprite2;

        }
        else if(noteTypeStatsList.Length <= 3)
        {
            spriteRenderer.sprite = sprite3;
        }
        else if(noteTypeStatsList.Length <= 4)
        {
            spriteRenderer.sprite = sprite4;
        }
        else
        {
            spriteRenderer.sprite = sprite5;
        }
        
    }

    private void ValidateTime()
    {
        if (bpm <= 0) bpm = 1;
        if (maxTime <= 0) maxTime = 1;

        if (maxTime != prevMaxTime)
        {
            bpm = 60f * (totalLength * 4) / maxTime; // Update bpm from maxTime
        }
        else if (bpm != prevBpm)
        {
            maxTime = 60f * (4 / totalLength) / bpm; // Update maxTime from bpm
        }

        prevMaxTime = maxTime;
        prevBpm = bpm;
    }

    private void CalculateTotalNoteTime()
    {
        totalLength = 0;
        foreach (NoteTypeStats noteStats in noteTypeStatsList)
        {
            totalLength += noteStats.noteLength;
        }

    }
    #endregion

    #region Override
    protected override void Collect()
    {
        NoteManager.Instance.ResetPlayerProp();
        NoteManager.Instance.noteTypeStatsList = noteTypeStatsList;
        NoteManager.Instance.maxTime = maxTime;
        NoteManager.Instance.bpm = bpm;
        NoteManager.Instance.totalLength = totalLength;
        NoteManager.Instance.CalculateTime();
    }

    protected override void DisableCollectable()
    {
        //_collider2D.enabled = false;
        //_spriteRenderer.enabled = false;
        StartCoroutine(CooldownRoutine(cooldown));

    }

    protected override void ResetCollectable()
    {

        //_collider2D.enabled = true;
        //_spriteRenderer.enabled = true;

    }

    #endregion

    private IEnumerator CooldownRoutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ResetCollectable();
    }
}
