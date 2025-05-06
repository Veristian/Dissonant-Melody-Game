using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DialogueLine
{
    public string characterName;
    [TextArea] public string dialogueText;
    [Range(0.01f,0.5f)] public float diaogueSpeed = 0.05f; // Time to display the dialogue before moving to the next one
    public float waitTime = 3f; // Time to wait before displaying the next dialogue line
    public bool continueFromPrevious = false; // If true, the next dialogue line will be connected to this one
}
[CreateAssetMenu(menuName = "Dialogue")]
public class DialogueInfo : ScriptableObject
{
    public DialogueLine[] dialogueLines; // Array of dialogue lines

}
