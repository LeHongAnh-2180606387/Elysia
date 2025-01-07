using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;
public class InputDialogueEditorManager : MonoBehaviour
{
    void Update()
    {
        if (ConversationManager.Instance != null && ConversationManager.Instance.IsConversationActive)
        {
            if (Input.GetKeyDown(KeyCode.W))
                ConversationManager.Instance.SelectNextOption();
            if (Input.GetKeyDown(KeyCode.S))
                ConversationManager.Instance.SelectPreviousOption();
            if (Input.GetKeyDown(KeyCode.F))
                ConversationManager.Instance.PressSelectedOption();
        }
    }
}
