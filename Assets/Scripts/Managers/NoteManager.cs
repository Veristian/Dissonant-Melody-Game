using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class NoteManager : Singleton<NoteManager>
{
    [Header("Refered From")]
    public NoteTypeStats[] noteTypeStatsList;
    public float maxTime;
    public float bpm = 120f;

    public PlayerMovement player;
    [Header("Refered to")]
    public NoteInfo[] notesTimeBoundary;
    public bool start = false;
    public bool Finish = true;

    [Tooltip("Progress of note completion in unit time")]
    public float Progress = 0;

    public float totalLength;

    private float timeTotal;

    public int noteIncrement = 0;

    private UnityEvent drawNoteEvent;
    private UnityEvent deleteNoteEvent;
    private UnityEvent ChangeNoteEvent;

    public MusicSheet musicSheet { private get; set; }

    //events
    public UnityEvent dashEvent = new UnityEvent();
    private AudioSource audioSource;

    [SerializeField] private AudioClip metronomeSound;


    private void Start()
    {
        if (drawNoteEvent == null)
        {
            drawNoteEvent = new UnityEvent();
        }
        if (deleteNoteEvent == null)
        {
            deleteNoteEvent = new UnityEvent();
        }
        if (ChangeNoteEvent == null)
        {
            ChangeNoteEvent = new UnityEvent();
        }

        drawNoteEvent.AddListener(() => musicSheet.drawNotes());
        deleteNoteEvent.AddListener(() => musicSheet.deleteNotes());
        ChangeNoteEvent.AddListener(() => musicSheet.ChangeImage(noteIncrement, notesTimeBoundary, noteTypeStatsList));
        ChangeNoteEvent.AddListener(() => PlayNoteSound(noteIncrement, notesTimeBoundary, noteTypeStatsList));

        LevelManager.Instance.OnDeathEvent.AddListener(() => NoteManagerResetState());
        audioSource = GetComponent<AudioSource>();

    }
    private void Update()
    {

        if (!start)
        {
            StartNoteCheck();
        }
        if (start)
        {
            AbilityChecks();
        }
    }

    #region NoteHandling
    public void CalculateTime()
    {
        deleteNoteEvent.Invoke();
        ResetNote();
        float multiplier = maxTime / totalLength;
        notesTimeBoundary = new NoteInfo[noteTypeStatsList.Length];
        timeTotal = 0;
        int count = 0;

        foreach (NoteTypeStats noteStats in noteTypeStatsList)
        {
            // Ensure each NoteInfo instance is initialized
            notesTimeBoundary[count] = new NoteInfo();
            notesTimeBoundary[count].noteLocation = timeTotal;
            if (noteStats.type == NoteTypeStats.Type.Eighth || noteStats.type == NoteTypeStats.Type.Quarter)
            {
                notesTimeBoundary[count].type = NoteInfo.Type.Click;
                notesTimeBoundary[count].pressedStartBoundary = timeTotal - noteStats.PressedLowerBound * multiplier;
                notesTimeBoundary[count].pressedEndBoundary = timeTotal + noteStats.PressedUpperBound * multiplier;
                timeTotal += noteStats.noteLength * multiplier;

            }

            else if (noteStats.type == NoteTypeStats.Type.Whole || noteStats.type == NoteTypeStats.Type.Half)
            {
                notesTimeBoundary[count].type = NoteInfo.Type.Hold;
                notesTimeBoundary[count].pressedStartBoundary = timeTotal - noteStats.PressedLowerBound * multiplier;
                notesTimeBoundary[count].pressedEndBoundary = timeTotal + noteStats.PressedUpperBound * multiplier;

                timeTotal += noteStats.noteLength * multiplier;

                notesTimeBoundary[count].releaseStartBoundary = timeTotal - noteStats.ReleaseLowerBound * multiplier;
                notesTimeBoundary[count].releaseEndBoundary = timeTotal + noteStats.ReleaseUpperBound * multiplier;


            }
            count += 1;
        }
        Finish = false;

        drawNoteEvent.Invoke();
    }

    private void ResetNote()
    {
        start = false;
        Progress = 0;
        noteIncrement = 0;
    }

    public void ResetPlayerProp()
    {
        player.usingAbility = false;
        player.dashTime = 0;
        player.isDashing = false;
        player.isHopping = false;
        player.hasHopped = true;
        player.isTeleporting = false;
        player.WasTeleporting = true;
        player.MaxFallSpeed = player.moveStats.MaxFallSpeed;

    }

    private void StartNoteCheck()
    {
        //will only start when ability button is pressed and the current song has not finished
        if (InputManager.AbilityWasPressed && !Finish)
        {
            start = true;
            StartCoroutine(PlayMetronome());

        }
    }

    private IEnumerator PlayMetronome()
    {
        while (true)
        {
            audioSource.PlayOneShot(metronomeSound);
            yield return new WaitForSeconds(1.0f / (bpm / 60.0f));
        }

    }

    private void ResetAll()
    {
        StopAllCoroutines();
        Finish = true;
        notesTimeBoundary = null;
        ResetNote();
    }

    private void RemoveAllNotes()
    {
        noteTypeStatsList = new NoteTypeStats[0];
    }

    private void PlayNoteSound(int noteIncrement, NoteInfo[] notesTimeBoundary, NoteTypeStats[] noteTypeStatsList)
    {
        switch (notesTimeBoundary[noteIncrement].type)
        {
            case NoteInfo.Type.Click:
                switch (notesTimeBoundary[noteIncrement].startState)
                {
                    case NoteInfo.State.Success:
                        if (noteTypeStatsList[noteIncrement].noteSuccessSound)
                        {
                            audioSource.PlayOneShot(noteTypeStatsList[noteIncrement].noteSuccessSound);
                        }
                        break;
                    case NoteInfo.State.Fail:
                        if (noteTypeStatsList[noteIncrement].noteFailSound)
                        {
                            audioSource.PlayOneShot(noteTypeStatsList[noteIncrement].noteFailSound);
                        }
                        break;
                }
                break;
            case NoteInfo.Type.Hold:

                switch (notesTimeBoundary[noteIncrement].endState)
                {
                    case NoteInfo.State.Success:
                        audioSource.clip = null;
                        audioSource.Stop();
                        break;
                    case NoteInfo.State.Fail:
                        audioSource.clip = null;
                        audioSource.Stop();
                        break;
                    case NoteInfo.State.Idle:
                        switch (notesTimeBoundary[noteIncrement].startState)
                        {
                            case NoteInfo.State.Success:
                                if (noteTypeStatsList[noteIncrement].noteSuccessSound)
                                {
                                    audioSource.clip = noteTypeStatsList[noteIncrement].noteSuccessSound;
                                }
                                break;
                            case NoteInfo.State.Fail:
                                if (noteTypeStatsList[noteIncrement].noteFailSound)
                                {
                                    audioSource.clip = noteTypeStatsList[noteIncrement].noteFailSound;
                                }
                                break;
                        }
                        audioSource.Play();
                        break;
                }
                break;
        }
    }

    #endregion

    #region Ability

    private void AbilityChecks()
    {


        //increment time
        Progress += Time.deltaTime;

        if (noteIncrement < notesTimeBoundary.Length)
        {
            if (noteTypeStatsList[noteIncrement].isRest)
            {
                if (Progress > notesTimeBoundary[noteIncrement].pressedEndBoundary)
                {
                    notesTimeBoundary[noteIncrement].startState = NoteInfo.State.Success;
                    ChangeNoteEvent.Invoke();
                    noteIncrement++;
                    return;
                }
            }
            switch (notesTimeBoundary[noteIncrement].type)
            {
                case NoteInfo.Type.Click:
                    //if click before then fail
                    if (Progress < notesTimeBoundary[noteIncrement].pressedStartBoundary)
                    {
                        if (InputManager.AbilityWasPressed)
                        {
                            notesTimeBoundary[noteIncrement].startState = NoteInfo.State.Fail;
                            ChangeNoteEvent.Invoke();
                            noteIncrement++;
                        }
                    }

                    //if click after ready then succeed and before pass
                    else if (Progress > notesTimeBoundary[noteIncrement].pressedStartBoundary && Progress <= notesTimeBoundary[noteIncrement].pressedEndBoundary)
                    {
                        if (InputManager.AbilityWasPressed)
                        {
                            notesTimeBoundary[noteIncrement].startState = NoteInfo.State.Success;

                            switch (noteTypeStatsList[noteIncrement].ability)
                            {

                                //Dashing
                                case NoteTypeStats.Ability.Dash:
                                    player.usingAbility = true;
                                    player.dashTime = 0;
                                    player.isDashing = true;

                                    break;

                                //Hopping
                                case NoteTypeStats.Ability.Hop:
                                    player.usingAbility = true;
                                    player.isHopping = true;
                                    player.hasHopped = false;
                                    break;
                                //Shooting
                                case NoteTypeStats.Ability.Shoot:
                                    player.usingAbility = true;
                                    player.isShooting = true;

                                    break;
                            }

                            ChangeNoteEvent.Invoke();
                            noteIncrement++;
                        }
                    }
                    //if not click then fail
                    else if (Progress > notesTimeBoundary[noteIncrement].pressedEndBoundary)
                    {
                        notesTimeBoundary[noteIncrement].startState = NoteInfo.State.Fail;
                        ChangeNoteEvent.Invoke();
                        noteIncrement++;
                    }

                    break;

                case NoteInfo.Type.Hold:
                    //if click before then fail
                    if (Progress < notesTimeBoundary[noteIncrement].pressedStartBoundary && notesTimeBoundary[noteIncrement].startState == NoteInfo.State.Idle)
                    {
                        if (InputManager.AbilityWasPressed)
                        {
                            notesTimeBoundary[noteIncrement].startState = NoteInfo.State.Fail;
                            notesTimeBoundary[noteIncrement].endState = NoteInfo.State.Fail;
                            ChangeNoteEvent.Invoke();
                            noteIncrement++;
                        }
                    }
                    //if click after ready then succeed
                    else if (Progress >= notesTimeBoundary[noteIncrement].pressedStartBoundary && Progress <= notesTimeBoundary[noteIncrement].pressedEndBoundary && notesTimeBoundary[noteIncrement].startState == NoteInfo.State.Idle)
                    {
                        if (InputManager.AbilityWasPressed)
                        {

                            notesTimeBoundary[noteIncrement].startState = NoteInfo.State.Success;
                            switch (noteTypeStatsList[noteIncrement].ability)
                            {
                                //Float
                                case NoteTypeStats.Ability.Float:
                                    player.MaxFallSpeed = player.moveStats.FloatMaxFallSpeed;
                                    ChangeNoteEvent.Invoke();

                                    break;

                                //Teleport
                                case NoteTypeStats.Ability.Teleport:
                                    player.usingAbility = true;
                                    player.isTeleporting = true;
                                    player.WasTeleporting = true;
                                    ChangeNoteEvent.Invoke();

                                    break;

                            }

                        }
                    }
                    //if not click then fail
                    else if (Progress > notesTimeBoundary[noteIncrement].pressedEndBoundary && notesTimeBoundary[noteIncrement].startState != NoteInfo.State.Success)
                    {
                        notesTimeBoundary[noteIncrement].startState = NoteInfo.State.Fail;
                        notesTimeBoundary[noteIncrement].endState = NoteInfo.State.Fail;
                        ChangeNoteEvent.Invoke();
                        noteIncrement++;
                    }

                    //if release early then fail
                    else if (Progress < notesTimeBoundary[noteIncrement].releaseStartBoundary && notesTimeBoundary[noteIncrement].endState == NoteInfo.State.Idle)
                    {
                        if (InputManager.AbilityWasReleased)
                        {
                            notesTimeBoundary[noteIncrement].endState = NoteInfo.State.Fail;
                            switch (noteTypeStatsList[noteIncrement].ability)
                            {
                                //Float
                                case NoteTypeStats.Ability.Float:
                                    player.MaxFallSpeed = player.moveStats.MaxFallSpeed;
                                    break;

                                //Teleport
                                case NoteTypeStats.Ability.Teleport:
                                    player.isTeleporting = false;
                                    break;

                            }
                            ChangeNoteEvent.Invoke();
                            noteIncrement++;
                        }
                    }
                    //if release after available success
                    else if (Progress >= notesTimeBoundary[noteIncrement].releaseStartBoundary && Progress < notesTimeBoundary[noteIncrement].releaseEndBoundary && notesTimeBoundary[noteIncrement].endState == NoteInfo.State.Idle)
                    {
                        if (InputManager.AbilityWasReleased)
                        {
                            notesTimeBoundary[noteIncrement].endState = NoteInfo.State.Success;
                            switch (noteTypeStatsList[noteIncrement].ability)
                            {
                                //Float
                                case NoteTypeStats.Ability.Float:
                                    player.MaxFallSpeed = player.moveStats.MaxFallSpeed;
                                    break;

                                //Teleport
                                case NoteTypeStats.Ability.Teleport:
                                    player.isTeleporting = false;
                                    break;

                            }
                            ChangeNoteEvent.Invoke();
                            noteIncrement++;
                        }
                    }
                    //if not release until the end
                    else if (Progress >= notesTimeBoundary[noteIncrement].releaseEndBoundary && notesTimeBoundary[noteIncrement].endState == NoteInfo.State.Idle)
                    {
                        notesTimeBoundary[noteIncrement].endState = NoteInfo.State.Success;
                        switch (noteTypeStatsList[noteIncrement].ability)
                        {
                            //Float
                            case NoteTypeStats.Ability.Float:
                                player.MaxFallSpeed = player.moveStats.MaxFallSpeed;
                                break;

                            //Teleport
                            case NoteTypeStats.Ability.Teleport:
                                player.isTeleporting = false;

                                break;

                        }
                        ChangeNoteEvent.Invoke();
                        noteIncrement++;

                    }
                    break;

            }
        }
        if (Progress > notesTimeBoundary[notesTimeBoundary.Length - 1].releaseEndBoundary && Progress > notesTimeBoundary[notesTimeBoundary.Length - 1].pressedEndBoundary)
        {
            NoteManagerResetState();
            return;
        }


    }

    public void NoteManagerResetState()
    {
        ResetAll();
        deleteNoteEvent.Invoke();
    }

    #endregion

}
