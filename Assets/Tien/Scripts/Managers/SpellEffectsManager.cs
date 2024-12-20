using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    // Quản lý các hiệu ứng phép thuật trong trò chơi.
    public class SpellEffectsManager : MonoBehaviour
    {
        // Dictionary lưu trữ các hiệu ứng phép thuật với tên hiệu ứng là khóa và chỉ số của hiệu ứng là giá trị.
        Dictionary<string, int> s_effects = new Dictionary<string, int>();

        // Sử dụng hiệu ứng phép thuật dựa trên id.
        public void UseSpellEffect(string id, StateManager states, EnemyStates eStates = null)
        {
            // Lấy chỉ số của hiệu ứng từ id.
            int index = GetEffect(id);

            // Kiểm tra nếu hiệu ứng không tồn tại.
            if (index == -1)
            {
                Debug.Log("Spell effect doesn't exist");
                return;
            }

            // Áp dụng hiệu ứng dựa trên chỉ số.
            switch (index)
            {
                case 0:
                    FireBreath(states);
                    break;
                case 1:
                    DarkShield(states);
                    break;
                case 2:
                    HealingSmall(states);
                    break;
                case 3:
                    Fireball(states);
                    break;
                case 4:
                    OnFire(states, eStates);
                    break;
            }
        }

        // Lấy chỉ số của hiệu ứng từ id.
        int GetEffect(string id)
        {
            int index = -1;

            // Kiểm tra nếu id tồn tại trong dictionary.
            if (s_effects.TryGetValue(id, out index))
            {
                return index;
            }

            return index;
        }

        // Cài đặt hiệu ứng "FireBreath".
        void FireBreath(StateManager states)
        {
            // Khởi đầu hiệu ứng "FireBreath".
            states.spellCast_start = states.inventoryManager.OpenBreathCollider;

            // Hiệu ứng lặp lại trong suốt quá trình.
            states.spellCast_loop = states.inventoryManager.EmitSpellParticle;
            states.spellCast_loop += states.SubstractFocusOverTime;

            // Dừng hiệu ứng "FireBreath".
            states.spellCast_stop = states.inventoryManager.CloseBreathCollider;
        }

        // Cài đặt hiệu ứng "DarkShield".
        void DarkShield(StateManager states)
        {
            // Khởi đầu hiệu ứng "DarkShield".
            states.spellCast_start = states.inventoryManager.OpenBlockCollider;

            // Hiệu ứng lặp lại trong suốt quá trình.
            states.spellCast_loop = states.inventoryManager.EmitSpellParticle;
            states.spellCast_loop += states.SubstractFocusOverTime;
            states.spellCast_loop += states.EffectBlocking;

            // Dừng hiệu ứng "DarkShield".
            states.spellCast_stop = states.inventoryManager.CloseBlockCollider;
            states.spellCast_stop += states.StopEffectBlocking;
        }

        // Cài đặt hiệu ứng "HealingSmall".
        void HealingSmall(StateManager states)
        {
            // Hiệu ứng lặp lại để hồi phục sức khỏe.
            states.spellCast_loop = states.AddHealth;
        }

        // Cài đặt hiệu ứng "Fireball".
        void Fireball(StateManager states)
        {
            // Hiệu ứng lặp lại để phát ra các hạt phép thuật.
            states.spellCast_loop = states.inventoryManager.EmitSpellParticle;
        }

        // Cài đặt hiệu ứng "OnFire".
        void OnFire(StateManager states, EnemyStates eStates)
        {
            // Hiệu ứng không có tác dụng với StateManager.
            if (states != null)
            {

            }

            // Áp dụng hiệu ứng "OnFire" cho EnemyStates.
            if (eStates != null)
            {
                eStates.spellEffect_loop = eStates.OnFire;
            }
        }

        // Singleton instance của SpellEffectsManager.
        public static SpellEffectsManager singleton;

        void Awake()
        {
            // Khởi tạo singleton.
            singleton = this;

            // Thêm các hiệu ứng vào dictionary.
            s_effects.Add("firebreath", 0);
            s_effects.Add("darkshield", 1);
            s_effects.Add("healingSmall", 2);
            s_effects.Add("fireball", 3);
            s_effects.Add("onFire", 4);
        }
    }
}
