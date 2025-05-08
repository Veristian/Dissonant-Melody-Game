using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    public GameObject dialogueBox; // Reference to the dialogue box UI element
    public TMPro.TextMeshProUGUI characterNameText; // Reference to the character name text UI element
    public TMPro.TextMeshProUGUI dialogueText; // Reference to the dialogue text UI element

    private Queue<DialogueLine> dialogueQueue; // Queue to hold the dialogue lines
    public DialogueEvents[] dialogueEvents;
    private float currentDialogueSpeed; // Current speed of the dialogue
    [HideInInspector] public bool inDialogue; // Flag to check if in dialogue
    [HideInInspector] public bool betweenDialogue; // Flag to check if between dialogues
    private void Start()
    {
        dialogueQueue = new Queue<DialogueLine>();
        dialogueBox.SetActive(false); // Hide the dialogue box at the start
    }

    private void Update()
    {
        if (InputManager.SkipWasPressed && dialogueBox.activeSelf && inDialogue) // Check if the jump button was pressed and the dialogue box is active
        {
            currentDialogueSpeed = 0;
        }
        if (InputManager.SkipWasPressed && dialogueBox.activeSelf && betweenDialogue) // Check if the jump button was pressed and we are between dialogues
        {
            StopCoroutine(TypeSentence(null)); // Stop the typing coroutine
            betweenDialogue = false;
            ShowNextLine(); // Show the next line of dialogue
        }
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
        betweenDialogue = false; // Reset the betweenDialogue flag
        if (dialogueQueue.Count == 0)
        {
            
            EndDialogue(); // End the dialogue if there are no more lines
            return;
        }

        DialogueLine line = dialogueQueue.Dequeue(); // Dequeue the next line from the queue
                
        if (line.character == DialogueLine.characterName.Fenny)
        {
            characterNameText.text = "Fenny"; // Set the color for Fenny's name
        }
        else if (line.character == DialogueLine.characterName.Ryu)
        {
            characterNameText.text = "Ryu"; // Set the color for Ryu's name
        }
        foreach (DialogueEvents dialogueEvent in dialogueEvents)
        {
            if (dialogueEvent.Index == line.lineNumber && dialogueEvent.OnDialogueStart != null) // Check if the line number matches the event index
            {
                dialogueEvent.OnDialogueStart.Invoke(); // Invoke the start event
            }
        }
        StartCoroutine(TypeSentence(line)); // Start typing out the sentence with a coroutine
    }

    private IEnumerator TypeSentence(DialogueLine line)
    {
        inDialogue = true;
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
        
        currentDialogueSpeed = line.diaogueSpeed; // Set the current dialogue speed
        foreach (char letter in fullText.ToCharArray())
        {
            currentText += letter; // Add each letter to the current text
            dialogueText.text = currentText; // Update the displayed text
            yield return new WaitForSeconds(currentDialogueSpeed); // Wait for a short time before displaying the next letter
        }
        betweenDialogue = true; // Set the flag to indicate that we are between dialogues
        foreach (DialogueEvents dialogueEvent in dialogueEvents)
        {
            if (dialogueEvent.Index == line.lineNumber && dialogueEvent.OnDialogueEnd != null) // Check if the line number matches the event index
            {
                dialogueEvent.OnDialogueEnd.Invoke(); // Invoke the start event
            }
        }
        yield return new WaitForSeconds(line.waitTime); // Wait for a specified time before showing the next line
        inDialogue = false; // Reset the inDialogue flag
        if (betweenDialogue)
        {
            ShowNextLine(); // Show the next line if connectDialogue is true
        }


    }

    private void EndDialogue()
    {
        dialogueBox.SetActive(false); // Hide the dialogue box
        dialogueQueue.Clear(); // Clear the dialogue queue
    }
}
