using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    // Lớp chứa các hàm tĩnh để sao chép dữ liệu giữa các đối tượng khác nhau.
    public static class StaticFunctions // For Database transfer in Inventory
    {
        //********************Weapons****************************

        // Sao chép dữ liệu từ đối tượng Weapon 'from' sang đối tượng Weapon 'to'.
        public static void DeepCopyWeapon(Weapon from, Weapon to)
        {
            // Sao chép các thuộc tính của Item.
            to.itemName = from.itemName;
            to.itemDescription = from.itemDescription;
            to.icon = from.icon;

            // Sao chép các thuộc tính của Weapon.
            to.oh_idle = from.oh_idle;
            to.th_idle = from.th_idle;
            to.itemType = from.itemType;

            // Sao chép danh sách actions.
            to.actions = new List<Action>();
            for (int i = 0; i < from.actions.Count; i++)
            {
                Action a = new Action();
                DeepCopyActionToAction(a, from.actions[i]);
                to.actions.Add(a);
            }

            // Sao chép danh sách two_handedActions.
            to.two_handedActions = new List<Action>();
            for (int i = 0; i < from.two_handedActions.Count; i++)
            {
                Action a = new Action();
                DeepCopyActionToAction(a, from.two_handedActions[i]);
                to.two_handedActions.Add(a);
            }

            // Sao chép các thuộc tính còn lại của Weapon.
            to.parryMultiplier = from.parryMultiplier;
            to.backstabMultiplier = from.backstabMultiplier;
            to.LeftHandMirror = from.LeftHandMirror;
            to.modelPrefab = from.modelPrefab;
            to.l_model_pos = from.l_model_pos;
            to.l_model_eulers = from.l_model_eulers;
            to.r_model_pos = from.r_model_pos;
            to.r_model_eulers = from.r_model_eulers;
            to.model_scale = from.model_scale;
            to.weaponStats = new WeaponStats();
            DeepCopyWeaponStats(from.weaponStats, to.weaponStats);
        }

        // Sao chép dữ liệu từ đối tượng Action 'w_a' sang đối tượng Action 'a'.
        public static void DeepCopyActionToAction(Action a, Action w_a)
        {
            a.input = w_a.input;
            a.targetAnim = w_a.targetAnim;
            a.audio_ids = w_a.audio_ids;
            a.type = w_a.type;
            a.spellClass = w_a.spellClass;
            a.canBeParried = w_a.canBeParried;
            a.changeSpeed = w_a.changeSpeed;
            a.animSpeed = w_a.animSpeed;
            a.canBackStab = w_a.canBackStab;
            a.canParry = w_a.canParry;
            a.overrideDamageAnim = w_a.overrideDamageAnim;
            a.damageAnim = w_a.damageAnim;
            a.staminaCost = w_a.staminaCost;
            a.fpCost = w_a.fpCost;

            // Sao chép danh sách steps.
            DeepCopyStepsList(w_a, a);
        }

        // Sao chép danh sách steps từ đối tượng Action 'from' sang đối tượng Action 'to'.
        public static void DeepCopyStepsList(Action from, Action to)
        {
            to.steps = new List<ActionSteps>();
            for (int i = 0; i < from.steps.Count; i++)
            {
                ActionSteps step = new ActionSteps();
                DeepCopySteps(from.steps[i], step);
                to.steps.Add(step);
            }
        }

        // Sao chép dữ liệu từ đối tượng ActionSteps 'from' sang đối tượng ActionSteps 'to'.
        public static void DeepCopySteps(ActionSteps from, ActionSteps to)
        {
            to.branches = new List<ActionAnim>();
            for (int i = 0; i < from.branches.Count; i++)
            {
                ActionAnim a = new ActionAnim();
                a.input = from.branches[i].input;
                a.targetAnim = from.branches[i].targetAnim;
                a.audio_ids = from.branches[i].audio_ids;
                to.branches.Add(a);
            }
        }

        // Sao chép các thuộc tính của WeaponStats.
        public static void DeepCopyWeaponStats(WeaponStats from, WeaponStats to)
        {
            to.physical = from.physical;
            to.slash = from.slash;
            to.strike = from.strike;
            to.thrust = from.thrust;
            to.magic = from.magic;
            to.lightning = from.lightning;
            to.fire = from.fire;
            to.dark = from.dark;
        }

        //***********************Spells***************************

        // Sao chép dữ liệu từ đối tượng Spell 'from' sang đối tượng Spell 'to'.
        public static void DeepCopySpell(Spell from, Spell to)
        {
            // Sao chép các thuộc tính của Item.
            to.itemName = from.itemName;
            to.itemDescription = from.itemDescription;
            to.icon = from.icon;

            // Sao chép các thuộc tính của Spell.
            to.itemType = from.itemType;
            to.spellType = from.spellType;
            to.spellClass = from.spellClass;
            to.projectile = from.projectile;
            to.spell_effect = from.spell_effect;
            to.particle_prefab = from.particle_prefab;

            // Sao chép danh sách actions.
            to.actions = new List<SpellAction>();
            for (int i = 0; i < from.actions.Count; i++)
            {
                SpellAction a = new SpellAction();
                DeepCopySpellAction(from.actions[i], a);
                to.actions.Add(a);
            }
        }

        // Sao chép dữ liệu từ đối tượng SpellAction 'from' sang đối tượng SpellAction 'to'.
        public static void DeepCopySpellAction(SpellAction from, SpellAction to)
        {
            to.input = from.input;
            to.targetAnim = from.targetAnim;
            to.throwAnim = from.throwAnim;
            to.castTime = from.castTime;
            to.staminaCost = from.staminaCost;
            to.focusCost = from.focusCost;
        }

        //***********************Consumables***************************

        // Sao chép dữ liệu từ đối tượng Consumable 'from' sang đối tượng Consumable 'to'.
        public static void DeepCopyConsumable(Consumable to, Consumable from)
        {
            to.itemName = from.itemName;
            to.icon = from.icon;
            to.itemDescription = from.itemDescription;

            to.itemType = from.itemType;
            to.consumableEffect = from.consumableEffect;
            to.targetAnim = from.targetAnim;
            to.audio_id = from.audio_id;
            to.itemPrefab = from.itemPrefab;
            to.model_scale = from.model_scale;
            to.r_model_eulers = from.r_model_eulers;
            to.r_model_pos = from.r_model_pos;
        }

        //----------------------------------------------For ActionManager to StateManager--------------------------------------------------------------------------

        // Sao chép dữ liệu Action từ Weapon 'w' sang ActionInput 'assign' trong ActionManager.
        public static void DeepCopyAction(Weapon w, ActionInput inp, ActionInput assign, List<Action> actionList, bool isLeftHand = false)
        {
            // Lấy action từ actionList dựa trên inp.
            Action a = GetAction(assign, actionList);
            // Lấy action từ Weapon dựa trên inp.
            Action w_a = w.GetAction(w.actions, inp);
            if (w_a == null)
                return;

            // Sao chép dữ liệu action từ w_a sang a.
            DeepCopyStepsList(w_a, a);
            a.type = w_a.type;
            a.targetAnim = w_a.targetAnim;
            a.audio_ids = w_a.audio_ids;
            a.spellClass = w_a.spellClass;
            a.canBeParried = w_a.canBeParried;
            a.changeSpeed = w_a.changeSpeed;
            a.animSpeed = w_a.animSpeed;
            a.canBackStab = w_a.canBackStab;
            a.canParry = w_a.canParry;
            a.overrideDamageAnim = w_a.overrideDamageAnim;
            a.damageAnim = w_a.damageAnim;
            a.parryMultiplier = w.parryMultiplier;
            a.backstabMultiplier = w.backstabMultiplier;
            a.staminaCost = w_a.staminaCost;
            a.fpCost = w_a.fpCost;

            // Nếu là tay trái, đặt thuộc tính mirror là true.
            if (isLeftHand)
            {
                a.mirror = true;
            }
        }

        // Lấy Action từ danh sách dựa trên ActionInput.
        public static Action GetAction(ActionInput inp, List<Action> actionSlots)
        {
            for (int i = 0; i < actionSlots.Count; i++)
            {
                if (actionSlots[i].input == inp)
                    return actionSlots[i];
            }
            return null;
        }
    }
}
