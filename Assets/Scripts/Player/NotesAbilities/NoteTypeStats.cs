using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Note Stats")]

public class NoteTypeStats : ScriptableObject
{
    [Header("general")]
    public Sprite noteImage;
    public Sprite noteSuccessImage;
    public Sprite noteFailImage;

    public enum Type 
    {
        Whole,
        Half,
        Quarter,
        Eighth

    }

    public Type type;

    public enum Ability
    {
        Dash,
        Float,
        Hop,
        Shoot,
        Teleport,
        None

    }

    public Ability ability;

    public bool isRest = false;

    [Tooltip("Length of note in percentage based on the time of one bar")]
    public float noteLength {get; private set; }
    
    [Tooltip("Lower limit of clicks based on percentage of one bar")]
    [Range(0,0.125f)]public float PressedLowerBound;
    [Tooltip("Upper limit of clicks based on percentage of one bar")]

    [Range(0,1f)]public float PressedUpperBound;
    [Tooltip("Lower limit of clicks based on percentage of one bar")]

    [Range(0,1f)]public float ReleaseLowerBound;
    [Tooltip("Upper limit of clicks based on percentage of one bar")]

    [Range(0,0.125f)]public float ReleaseUpperBound;

    public AudioClip noteSuccessSound;
    public AudioClip noteFailSound;





    private void OnValidate()
    {
        CalculateValues();
    }

    private void Awake()
    {
        CalculateValues();
    }

    private void CalculateValues()
    {
        if (isRest)
        {
            ability = NoteTypeStats.Ability.None;
        }
        //Calculate Note Length
    
        switch (type)
        {
            case Type.Eighth:
                noteLength = 0.125f;
                break;
            case Type.Half:
                noteLength = 0.5f;
                break;
            case Type.Quarter:
                noteLength = 0.25f;
                break;
            case Type.Whole:
                noteLength = 1f;
                break;
            
        }

        // Cannot Exceed the length of the note
        PressedUpperBound = Mathf.Clamp(PressedUpperBound, 0, noteLength);
        ReleaseLowerBound = Mathf.Clamp(ReleaseLowerBound, 0, noteLength);

        //Cannot exceed the length of an eighth note (0.125f)
        PressedLowerBound = Mathf.Clamp(PressedLowerBound, 0, 0.125f);
        ReleaseUpperBound = Mathf.Clamp(ReleaseUpperBound, 0, 0.125f);

    }

}
