using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    // Lớp ActionManager chịu trách nhiệm quản lý các hành động của nhân vật trong game.
    public class ActionManager : MonoBehaviour
    {
        // Lưu chỉ số của hành động hiện tại
        public int actionIndex;

        // Danh sách các hành động có thể thực hiện
        public List<Action> actionSlots = new List<Action>();

        // Tham chiếu đến StateManager, quản lý trạng thái của nhân vật
        StateManager states;

        // Constructor: Khởi tạo danh sách actionSlots với 4 hành động dựa trên enum ActionInput
        ActionManager()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = new Action();
                a.input = (ActionInput)i;
                actionSlots.Add(a);
            }
        }

        // Phương thức khởi tạo: nhận tham chiếu đến StateManager và gọi UpdateActionsOneHanded
        public void Init(StateManager st)
        {
            states = st;
            UpdateActionsOneHanded(); // Chỉ cập nhật một lần khi bắt đầu game. Cần gọi lại khi thay đổi vũ khí.
        }

        // Làm trống tất cả các slot hành động trong actionSlots
        void EmptyAllSlots()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = StaticFunctions.GetAction((ActionInput)i, actionSlots);
                // Đặt lại các thuộc tính của hành động
                a.targetAnim = null;
                a.audio_ids = null;
                a.steps = null;
                a.mirror = false;
                a.type = ActionType.attack;
                a.canBeParried = true;
                a.changeSpeed = true;
                a.animSpeed = 1;
                a.canBackStab = false;
            }
        }

        // Cập nhật các hành động khi nhân vật đang cầm vũ khí một tay
        public void UpdateActionsOneHanded()
        {
            EmptyAllSlots(); // Đầu tiên, làm trống các slot hành động

            // Sao chép hành động từ vũ khí tay phải
            StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.rb, ActionInput.rb, actionSlots);
            StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.rt, ActionInput.rt, actionSlots);

            if (states.inventoryManager.hasLeftHandWeapon)
            {
                // Nếu có vũ khí tay trái, sao chép hành động từ tay trái vào các slot phù hợp
                StaticFunctions.DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.rb, ActionInput.lb, actionSlots, true);
                StaticFunctions.DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.rt, ActionInput.lt, actionSlots, true);
            }
            else
            {
                // Nếu không có vũ khí tay trái, sao chép hành động từ tay phải vào các slot tay trái
                StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.lb, ActionInput.lb, actionSlots);
                StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.lt, ActionInput.lt, actionSlots);
            }
        }

        // (Bỏ qua đoạn code đã comment)

        // Cập nhật các hành động khi nhân vật đang cầm vũ khí hai tay
        public void UpdateActionsTwoHanded()
        {
            EmptyAllSlots(); // Làm trống các slot hành động
            Weapon w = states.inventoryManager.rightHandWeapon.instance;

            for (int i = 0; i < w.two_handedActions.Count; i++)
            {
                Action a = StaticFunctions.GetAction(w.two_handedActions[i].input, actionSlots);
                // Sao chép hành động hai tay vào actionSlots
                a.steps = w.two_handedActions[i].steps;
                a.type = w.two_handedActions[i].type;
            }
        }

        // Xác định hành động tương ứng với input của người chơi từ StateManager
        public ActionInput GetActionInput(StateManager st)
        {
            if (st.rb)
                return ActionInput.rb;
            if (st.lb)
                return ActionInput.lb;
            if (st.rt)
                return ActionInput.rt;
            if (st.lt)
                return ActionInput.lt;

            return ActionInput.rb;
        }

        // Trả về hành động trong actionSlots tương ứng với input của người chơi
        public Action GetActionSlot(StateManager st)
        {
            ActionInput a_input = GetActionInput(st);
            return StaticFunctions.GetAction(a_input, actionSlots);
        }

        // Trả về hành động từ actionSlots dựa trên input cụ thể
        public Action GetActionFromInput(ActionInput a_input)
        {
            return StaticFunctions.GetAction(a_input, actionSlots);
        }
    }

    // Định nghĩa các loại input của người chơi (nhấn các nút trên tay cầm)
    public enum ActionInput
    {
        rb, lb, rt, lt
    }

    // Định nghĩa các loại hành động (tấn công, đỡ đòn, phép thuật, parry)
    public enum ActionType
    {
        attack, block, spells, parry
    }

    // Định nghĩa các loại phép thuật (pyromancy, miracles, sorcery)
    public enum SpellClass
    {
        pyromancy, miracles, sorcery
    }

    // Định nghĩa các loại phép thuật theo cách hoạt động (projectile, buff, looping)
    public enum SpellType
    {
        projectile, buff, looping
    }

    // Lớp Action: mô tả chi tiết của một hành động
    [System.Serializable]
    public class Action
    {
        public ActionInput input;     
        public ActionType type;      
        public SpellClass spellClass;   
        public string targetAnim;       
        public string audio_ids;       
        public List<ActionSteps> steps; 
        public bool mirror = false;     
        public bool canBeParried = true;
        public bool changeSpeed = false;
        public float animSpeed = 1;    
        public bool canParry = false;   
        public bool canBackStab = false;
        public float staminaCost;       
        public float fpCost;            

        // Lấy một bước hành động dựa trên chỉ số hiện tại
        public ActionSteps GetActionStep(ref int indx)
        {
            if (steps.Count > 0)
            {
                if (indx > steps.Count - 1)
                    indx = 0;
                ActionSteps retVal = steps[indx];

                if (indx > steps.Count - 1)
                    indx = 0;
                else
                    indx++;

                return retVal;
            }
            return null;
        }

        // Các thuộc tính ẩn (không hiển thị trong Unity Inspector)
        [HideInInspector]
        public float parryMultiplier;
        [HideInInspector]
        public float backstabMultiplier;

        public bool overrideDamageAnim; // Xác định xem có ghi đè animation khi nhận damage không
        public string damageAnim;       
    }

    // Lớp ActionSteps: mô tả các bước của một hành động
    [System.Serializable]
    public class ActionSteps
    {
        public List<ActionAnim> branches = new List<ActionAnim>(); // Các nhánh của hành động

        // Lấy một nhánh của hành động dựa trên input
        public ActionAnim GetBranch(ActionInput inp)
        {
            for (int i = 0; i < branches.Count; i++)
            {
                if (branches[i].input == inp)
                    return branches[i];
            }

            return branches[0];
        }
    }

    // Lớp ActionAnim: mô tả animation của một nhánh hành động
    [System.Serializable]
    public class ActionAnim
    {
        public ActionInput input;      
        public string targetAnim;      
        public string audio_ids;       
    }

    // Lớp SpellAction: mô tả hành động khi sử dụng phép thuật
    [System.Serializable]
    public class SpellAction
    {
        public ActionInput input;      
        public string targetAnim;      
        public string throwAnim;       
        public float castTime;       
        public float focusCost;        
        public float staminaCost;      
    }
}
