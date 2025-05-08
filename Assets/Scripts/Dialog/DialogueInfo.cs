using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

[Serializable]
public class DialogueLine
{
    [HideInInspector] public int lineNumber; // Number of the dialogue line
    public enum characterName
    {
        Fenny,
        Ryu
    };
    public characterName character; // Character who is speaking the dialogue line
    [TextArea] public string dialogueText = ""; // Text of the dialogue line
    [Range(0.01f,0.5f)] public float diaogueSpeed = 0.05f; // Time to display the dialogue before moving to the next one
    public float waitTime = 3f; // Time to wait before displaying the next dialogue line
    public bool continueFromPrevious = false; // If true, the next dialogue line will be connected to this one

}
[CreateAssetMenu(menuName = "Dialogue")]
public class DialogueInfo : ScriptableObject
{
    public DialogueLine[] dialogueLines; // Array of dialogue lines
    private void OnEnable()
    {
        for (int i = 0; i < dialogueLines.Length; i++)
        {
            dialogueLines[i].lineNumber = i; // Assign line numbers to each dialogue line
        }
    }

}
