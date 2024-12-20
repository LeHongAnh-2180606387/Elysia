using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    // Lớp này lưu trữ thông tin về các NPC và đối thoại của chúng.
    // Được kế thừa từ ScriptableObject, cho phép tạo ra và cấu hình các NPC có thể tương tác trong Unity Editor.
    public class NPCScriptableObject : ScriptableObject
    {
        // Mảng chứa thông tin đối thoại của các NPC.
        public NPCDialogue[] npcs;
    }

    // Lớp này đại diện cho một NPC cụ thể và các đối thoại của nó.
    [System.Serializable]
    public class NPCDialogue
    {
        // ID duy nhất của NPC để nhận diện.
        public string npc_id;

        // Danh sách các đối thoại của NPC.
        public Dialogue[] dialogue;
    }

    // Lớp này đại diện cho một đoạn đối thoại của NPC.
    [System.Serializable]
    public class Dialogue
    {
        // Mảng các văn bản đối thoại mà NPC nói.
        public string[] dialogueText;

        // Biến này cho biết liệu chỉ số đối thoại có nên tăng sau khi đoạn đối thoại hiện tại được trình bày.
        public bool increaseIndex;
    }
}
