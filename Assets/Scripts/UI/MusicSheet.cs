using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEditor;
public class MusicSheet : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GameObject imagePrefab;
    [SerializeField] private GameObject progressBarPrefab;
    [SerializeField] private GameObject debuStartLine;
    [SerializeField] private GameObject debugEndLine;

    private GameObject[] debugLines;

    private GameObject progressBar;
    private RectTransform progressBarTransform;
    private Vector3 progressBarStartTransform;
    private NoteInfo[] noteInfos;
    private NoteTypeStats[] noteStats;
    private GameObject[] noteImageList;
    [Header("Settings")]
    [SerializeField] private Vector2 Origin;
    [SerializeField] private float startPadding = 10;
    [SerializeField] private float noteDistanceScale = 1;   
    [SerializeField] private bool Debug;   

    [Header("Note Step")]
    [SerializeField] private float Dash;
    [SerializeField] private float Hop;
    [SerializeField] private float Shoot;
    [SerializeField] private float Float;
    [SerializeField] private float Teleport;
    [SerializeField] private float None;

    private void Awake()
    {
        NoteManager.Instance.musicSheet = this;
    }

    private void Update()
    {
        if (NoteManager.Instance.start && progressBar != null)
        {
            progressBarTransform.position = new Vector3(progressBarStartTransform.x + NoteManager.Instance.Progress*noteDistanceScale, progressBarStartTransform.y);

        }
    }
  

    public void ChangeImage(int i,NoteInfo[] noteInfo, NoteTypeStats[] noteTypeStats)
    {
        switch (noteInfo[i].type)
        {
            case NoteInfo.Type.Click:
                switch (noteInfo[i].startState)
                {
                    case NoteInfo.State.Fail:
                        noteImageList[i].GetComponent<Image>().sprite = noteTypeStats[i].noteFailImage;
                        break;
                    case NoteInfo.State.Success:
                        noteImageList[i].GetComponent<Image>().sprite = noteTypeStats[i].noteSuccessImage;
                        break;
                    default:
                        break;

                }
                break;
            case NoteInfo.Type.Hold:
                switch (noteInfo[i].endState)
                {
                    case NoteInfo.State.Fail:
                        noteImageList[i].GetComponent<Image>().sprite = noteTypeStats[i].noteFailImage;
                        break;
                    case NoteInfo.State.Success:
                        noteImageList[i].GetComponent<Image>().sprite = noteTypeStats[i].noteSuccessImage;
                        break;
                    default:
                        break;

                }
                break;

            
        }
    }
    public void drawNotes()
    {
        noteDistanceScale = NoteManager.Instance.totalLength * 200;
        noteInfos = NoteManager.Instance.notesTimeBoundary;
        noteStats = NoteManager.Instance.noteTypeStatsList;
        noteImageList = new GameObject[noteInfos.Length];
        debugLines = new GameObject[noteInfos.Length*4];
        if (progressBarPrefab != null)
        {
            progressBar = Instantiate(progressBarPrefab, transform);
            progressBarTransform = progressBar.GetComponent<RectTransform>();
            progressBarTransform.position += new Vector3(Origin.x + startPadding, Origin.y);
            progressBarStartTransform = progressBarTransform.position;
        }


        if (imagePrefab != null)
        {
            for (int i = 0; i < noteInfos.Length; i++)
            {
                noteImageList[i] = Instantiate(imagePrefab, transform);
                noteImageList[i].GetComponent<RectTransform>().position += new Vector3(Origin.x + startPadding + noteInfos[i].noteLocation*noteDistanceScale, Origin.y);
                noteImageList[i].GetComponent<Image>().sprite = noteStats[i].noteImage;
                if (Debug)
                {
                    debugLines[i*4] = Instantiate(debuStartLine, transform);
                    debugLines[i*4].GetComponent<RectTransform>().position += new Vector3(Origin.x + startPadding + (noteInfos[i].pressedStartBoundary)*noteDistanceScale, Origin.y);
                    
                    debugLines[i*4+1] = Instantiate(debuStartLine, transform);
                    debugLines[i*4+1].GetComponent<RectTransform>().position += new Vector3(Origin.x + startPadding + (noteInfos[i].pressedEndBoundary)*noteDistanceScale, Origin.y);

                    if (noteInfos[i].releaseEndBoundary != 0)
                    {
                        debugLines[i*4+2] = Instantiate(debugEndLine, transform);
                        debugLines[i*4+2].GetComponent<RectTransform>().position += new Vector3(Origin.x + startPadding + (noteInfos[i].releaseStartBoundary)*noteDistanceScale, Origin.y);
                        
                        debugLines[i*4+3] = Instantiate(debugEndLine, transform);
                        debugLines[i*4+3].GetComponent<RectTransform>().position += new Vector3(Origin.x + startPadding + (noteInfos[i].releaseEndBoundary)*noteDistanceScale, Origin.y);

                    }

                }
                switch (noteStats[i].ability)
                {
                    case NoteTypeStats.Ability.Dash:
                        noteImageList[i].GetComponent<RectTransform>().position += new Vector3(0, Dash);
                        break;
                    case NoteTypeStats.Ability.Float:
                        noteImageList[i].GetComponent<RectTransform>().position += new Vector3(0, Float);
                        break;
                    case NoteTypeStats.Ability.Hop:
                        noteImageList[i].GetComponent<RectTransform>().position += new Vector3(0, Hop);
                        break;
                    case NoteTypeStats.Ability.Teleport:
                        noteImageList[i].GetComponent<RectTransform>().position += new Vector3(0, Teleport);
                        break;
                    case NoteTypeStats.Ability.Shoot:
                        noteImageList[i].GetComponent<RectTransform>().position += new Vector3(0, Shoot);
                        break;
                    case NoteTypeStats.Ability.None:
                        noteImageList[i].GetComponent<RectTransform>().position += new Vector3(0, None);
                        break;
                }

            }
        }

        
    }

    public void deleteNotes()
    {
        if (noteImageList != null)
        {
            foreach (GameObject notes in noteImageList)
            {
                Destroy(notes);
                
            }
            noteImageList = new GameObject[0];

        }
        if (debugLines != null)
        {
            foreach (GameObject notes in debugLines)
            {
                Destroy(notes);
                
            }
            debugLines = new GameObject[0];

        }
        if (progressBar != null)
        {
            Destroy(progressBar);
        }
    }
}
