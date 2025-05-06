using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class NoteInfo
{
    public enum Type
    {
        Click,
        Hold,

    }
    public Type type;
    public enum State
    {
        Idle,
        Success,
        Fail,

    }
    public State startState = State.Idle;
    public State endState = State.Idle;

    public float noteLocation = 0;
    public float pressedStartBoundary = 0;
    public float pressedEndBoundary = 0;
    public float releaseStartBoundary = 0;
    public float releaseEndBoundary = 0;

}
