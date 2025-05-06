using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueInfo dialogueInfo; // Reference to the DialogueInfo scriptable object

    private void Awake()
    {
        gameObject.GetComponent<BoxCollider2D>().isTrigger = true; // Ensure the collider is set as a trigger
        gameObject.GetComponent<BoxCollider2D>().enabled = true; // Enable the trigger collider
        gameObject.GetComponent<SpriteRenderer>().enabled = false; // Enable the trigger collider
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Check if the player has entered the trigger
        {
            if (dialogueInfo == null) // Check if the dialogueInfo is not assigned
            {
                Debug.LogError("DialogueInfo is not assigned in the inspector!"); // Log an error message
                return; // Exit the method if dialogueInfo is not assigned
            }
            DialogueManager.Instance.StartDialogue(dialogueInfo); // Start the dialogue
            gameObject.GetComponent<BoxCollider2D>().enabled = false; // Disable the trigger object to prevent multiple triggers
        }
    }
    
}
