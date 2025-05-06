using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    public GameObject dialogueBox; // Reference to the dialogue box UI element
    public TMPro.TextMeshProUGUI characterNameText; // Reference to the character name text UI element
    public TMPro.TextMeshProUGUI dialogueText; // Reference to the dialogue text UI element

    private Queue<DialogueLine> dialogueQueue; // Queue to hold the dialogue lines

    private void Start()
    {
        dialogueQueue = new Queue<DialogueLine>();
        dialogueBox.SetActive(false); // Hide the dialogue box at the start
    }

    public void StartDialogue(DialogueInfo dialogueInfo)
    {
        dialogueQueue.Clear(); // Clear any existing dialogue lines in the queue

        foreach (DialogueLine line in dialogueInfo.dialogueLines)
        {
            dialogueQueue.Enqueue(line); // Enqueue each line from the DialogueInfo scriptable object
        }

        ShowNextLine(); // Show the first line of dialogue
    }

    public void ShowNextLine()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue(); // End the dialogue if there are no more lines
            return;
        }

        DialogueLine line = dialogueQueue.Dequeue(); // Dequeue the next line from the queue
        characterNameText.text = line.characterName; // Set the character name text
        StartCoroutine(TypeSentence(line)); // Start typing out the sentence with a coroutine
    }

    private IEnumerator TypeSentence(DialogueLine line)
    {
        dialogueBox.SetActive(true); // Show the dialogue box

        string fullText = line.dialogueText; // Get the full text of the line
        string currentText;
        if (line.continueFromPrevious)
        {
            currentText = dialogueText.text;
        }
        else
        {
            currentText = ""; // Initialize an empty string for the current text
        }

        foreach (char letter in fullText.ToCharArray())
        {
            currentText += letter; // Add each letter to the current text
            dialogueText.text = currentText; // Update the displayed text
            yield return new WaitForSeconds(line.diaogueSpeed); // Wait for a short time before displaying the next letter
        }

        yield return new WaitForSeconds(line.waitTime); // Wait for a specified time before showing the next line
        ShowNextLine(); // Show the next line if connectDialogue is true

    }

    private void EndDialogue()
    {
        dialogueBox.SetActive(false); // Hide the dialogue box
        dialogueQueue.Clear(); // Clear the dialogue queue
    }
}
