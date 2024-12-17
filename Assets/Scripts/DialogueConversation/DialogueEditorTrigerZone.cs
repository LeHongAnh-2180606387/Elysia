using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class DialogueEditorTrigerZone : MonoBehaviour
{
    [SerializeField]
    public NPCConversation conversation;
    private bool hasTriggered = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            //EventsManager.StartConversationDialogue(conversation);
        }
    }
}
