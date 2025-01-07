using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SA
{
    // Quản lý đối thoại giữa người chơi và NPC, bao gồm hiển thị và cập nhật văn bản đối thoại.
    public class DialogueManager : MonoBehaviour
    {
        // Các thành phần UI và biến lưu trữ trạng thái đối thoại.
        public Text dialogueText; // Text component để hiển thị văn bản đối thoại.
        public GameObject textObj; // GameObject chứa Text component.
        Transform origin; // Vị trí của NPC.
        NPCDialogue npc_dialogue; // Đối tượng lưu trữ các đối thoại của NPC.
        public bool dialogueActive; // Trạng thái của đối thoại.
        bool updateDialog; // Cờ kiểm tra xem đối thoại có cần cập nhật không.
        int textIndex; // Chỉ số của văn bản hiện tại trong đối thoại.
        public Transform playerObject; // Vị trí của người chơi.

        // Khởi tạo DialogueManager với vị trí của người chơi.
        public void Init(Transform po)
        {
            playerObject = po;
        }

        public NPCStates[] npcStates;
        Dictionary<string, int> npc_ids = new Dictionary<string, int>();
        NPCStates npc_state;


        // Khởi tạo đối thoại với NPC, truyền vào đối tượng NPC và ID của nó.
        public void InitDialogue(Transform o, string id)
        {
            origin = o; // Vị trí của NPC.
            npc_dialogue = ResourceManager.singleton.GetNPCDialogue(id); // Lấy đối thoại của NPC từ ResourceManager.
            npc_state = GetNPCStates(id); // Lấy trạng thái của NPC từ ID.
            dialogueActive = true; // Kích hoạt đối thoại.
            textObj.SetActive(true); // Hiển thị đối thoại.
            updateDialog = false; // Đặt cờ cập nhật đối thoại thành false.
            textIndex = 0; // Đặt chỉ số văn bản thành 0.
        }

        // Cập nhật đối thoại trong mỗi khung hình dựa trên đầu vào của người chơi.
        public void Tick(bool a_input)
        {
            if (!dialogueActive) // Nếu đối thoại không hoạt động, thoát khỏi phương thức.
                return;
            if (origin == null) // Nếu không có vị trí của NPC, thoát khỏi phương thức.
                return;

            // Tính khoảng cách giữa người chơi và NPC. Nếu vượt quá ngưỡng, đóng đối thoại.
            float distance = Vector3.Distance(playerObject.transform.position, origin.transform.position);
            if (distance > 3.5)
            {
                CloseDialogue();
            }

            // Nếu đối thoại cần cập nhật, cập nhật văn bản đối thoại.
            if (!updateDialog)
            {
                updateDialog = true;
                dialogueText.text = npc_dialogue.dialogue[npc_state.dialogueIndex].dialogueText[textIndex];
            }

            // Xử lý đầu vào của người chơi để chuyển tiếp đối thoại.
            if (a_input)
            {
                updateDialog = false;
                textIndex++;

                // Kiểm tra nếu đã đến cuối văn bản. Nếu cần, chuyển đến đoạn đối thoại tiếp theo hoặc đóng đối thoại.
                if (textIndex > npc_dialogue.dialogue[npc_state.dialogueIndex].dialogueText.Length - 1)
                {
                    if (npc_dialogue.dialogue[npc_state.dialogueIndex].increaseIndex)
                    {
                        npc_state.dialogueIndex++;

                        if (npc_state.dialogueIndex > npc_dialogue.dialogue.Length - 1)
                        {
                            npc_state.dialogueIndex = npc_dialogue.dialogue.Length - 1;
                        }
                    }
                    CloseDialogue();
                }
            }
        }

        // Đóng đối thoại và ẩn UI.
        void CloseDialogue()
        {
            dialogueActive = false;
            textObj.SetActive(false);
        }

        // Singleton pattern to access DialogueManager from other classes.
        public static DialogueManager singleton;
        private void Awake()
        {
            singleton = this; // Đặt singleton thành instance hiện tại của DialogueManager.
            textObj.SetActive(false); // Đảm bảo UI đối thoại không hiển thị khi bắt đầu.
            for (int i = 0; i < npcStates.Length; i++)
            {
                npc_ids.Add(npcStates[i].npc_id, i); // Thêm các NPC vào từ điển để tra cứu nhanh.
            }
        }

        // Lấy trạng thái của NPC dựa trên ID.
        public NPCStates GetNPCStates(string id)
        {
            int index = -1;
            npc_ids.TryGetValue(id, out index); // Tìm kiếm ID trong từ điển.

            if (index == -1) // Nếu không tìm thấy ID, trả về null.
                return null;

            return npcStates[index]; // Trả về trạng thái của NPC tương ứng với ID.
        }
    }

    // Lớp lưu trữ trạng thái của NPC, bao gồm ID và chỉ số đối thoại hiện tại.
    [System.Serializable]
    public class NPCStates
    {
        public string npc_id; // ID của NPC.
        public int dialogueIndex; // Chỉ số đối thoại hiện tại của NPC.
    }
}
