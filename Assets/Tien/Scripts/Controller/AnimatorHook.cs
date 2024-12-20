using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class AnimatorHook : MonoBehaviour
    {

        Animator anim;               
        StateManager states;          
        EnemyStates eStates;          
        Rigidbody rigid;              

        public float rm_multi;        
        bool rolling;                
        float roll_t;                 
        float delta;                  
        AnimationCurve roll_curve;    

        HandleIK ik_handler;          
        public bool useIK;           
        public AvatarIKGoal currentHand; 

        public bool killDelta;       

        //-----------------------------------------------------------------------
        // Hàm khởi tạo, được gọi khi khởi động
        public void Init(StateManager st, EnemyStates eSt)
        {
            states = st;
            eStates = eSt;

            // Nếu có StateManager (người chơi), gán các biến tương ứng
            if (st != null)
            {
                anim = st.anim;
                rigid = st.rigid;
                roll_curve = states.roll_curve;
                delta = states.delta;
            }
            // Nếu có EnemyStates (địch), gán các biến tương ứng
            if (eSt != null)
            {
                anim = eSt.anim;
                rigid = eSt.rigid;
                delta = eSt.delta;
            }

            // Khởi tạo IK handler
            ik_handler = gameObject.GetComponent<HandleIK>();
            if (ik_handler != null)
                ik_handler.Init(anim);
        }

        // Khởi tạo cho hành động lăn
        public void InitForRoll()
        {
            rolling = true;
            roll_t = 0;
        }

        // Đóng hành động lăn, đặt lại các biến liên quan
        public void CloseRoll()
        {
            if (rolling == false)
                return;

            rm_multi = 1;
            rolling = false;
        }

        // Được gọi mỗi khi Animator được di chuyển
        void OnAnimatorMove()
        {
            // Xử lý IK nếu có
            if (ik_handler != null)
            {
                ik_handler.OnAnimatorMoveTick((currentHand == AvatarIKGoal.LeftHand));
            }

            // Nếu cả states và eStates đều null, thoát khỏi hàm
            if (states == null && eStates == null)
                return;

            // Nếu không có rigidbody, thoát khỏi hàm
            if (rigid == null)
                return;

            // Nếu đang trong trạng thái "onEmpty" hoặc "canMove", không xử lý chuyển động
            if (states != null)
            {
                if (states.onEmpty)
                    return;
                delta = states.delta;
            }

            if (eStates != null)
            {
                if (eStates.canMove)
                    return;
                delta = eStates.delta;
            }

            // Đặt lại lực cản của rigidbody khi chơi animation
            rigid.drag = 0;

            // Nếu rm_multi là 0, đặt lại thành 1
            if (rm_multi == 0)
                rm_multi = 1;

            // Xử lý chuyển động vật lý khi không lăn
            if (rolling == false)
            {
                Vector3 delta2 = anim.deltaPosition;
                if (killDelta)
                {
                    killDelta = false;
                    delta2 = Vector3.zero;
                }

                Vector3 v = (delta2 * rm_multi) / delta;
                v.y = rigid.velocity.y;

                if (eStates)
                    eStates.agent.velocity = v;
                else
                    rigid.velocity = v;
            }
            else
            { // Xử lý khi đang lăn
                roll_t += delta / 0.6f;
                if (roll_t > 1)
                {
                    roll_t = 1;
                }

                if (states == null)
                    return;

                float zValue = states.roll_curve.Evaluate(roll_t);
                Vector3 v1 = Vector3.forward * zValue;
                Vector3 relative = transform.TransformDirection(v1);
                Vector3 v2 = (relative * rm_multi);

                v2.y = rigid.velocity.y;
                rigid.constraints = RigidbodyConstraints.FreezePositionY;
                rigid.velocity = v2 * 3.2f;
            }
        }

        // Xử lý IK khi animator chạy IK
        void OnAnimatorIK()
        {
            if (ik_handler == null)
                return;

            if (!useIK)
            {
                if (ik_handler.weight > 0)
                {
                    ik_handler.IKTick(currentHand, 0);
                }
                else
                {
                    ik_handler.weight = 0;
                }
            }
            else
            {
                ik_handler.IKTick(currentHand, 1);
            }
        }

        // Được gọi mỗi khung hình để xử lý IK sau khi tất cả các hành động khác đã hoàn thành
        void LateUpdate()
        {
            if (ik_handler != null)
                ik_handler.LateTick();
        }

        // Cho phép tấn công
        public void OpenAttack()
        {
            if (states)
                states.canAttack = true;
        }

        // Cho phép di chuyển
        public void OpenCanMove()
        {
            if (states)
            {
                states.canMove = true;
            }
        }

        // Mở collider gây sát thương
        public void OpenDamageColliders()
        {
            if (states)
                states.inventoryManager.OpenAllDamageColliders();
            if (eStates)
                eStates.OpenDamageCollier();

            OpenParryFlag();
        }

        // Đóng collider gây sát thương
        public void CloseDamageColliders()
        {
            if (states)
            {
                states.inventoryManager.CloseAllDamageColliders();
            }
            if (eStates)
            {
                eStates.CloseDamageCollider();
            }
            CloseParryFlag();
        }

        // Mở cờ parry (cho phép parry)
        public void OpenParryFlag()
        {
            if (states)
            {
                states.parryIsOn = true;
            }

            if (eStates)
            {
                eStates.parryIsOn = true;
            }
        }

        // Đóng cờ parry (không cho phép parry)
        public void CloseParryFlag()
        {
            if (states)
            {
                states.parryIsOn = false;
            }

            if (eStates)
            {
                eStates.parryIsOn = false;
            }
        }

        // Mở collider cho parry
        public void OpenParryCollider()
        {
            if (states == null)
                return;

            states.inventoryManager.OpenParryCollider();
        }

        // Đóng collider cho parry
        public void CloseParryCollider()
        {
            if (states == null)
                return;

            states.inventoryManager.CloseParryCollider();
        }

        // Đóng hiệu ứng hạt (particle) của spell
        public void CloseParticle()
        {
            if (states)
            {
                if (states.inventoryManager.currentSpell.currentParticle != null)
                    states.inventoryManager.currentSpell.currentParticle.SetActive(false);
            }
        }

        // Khởi tạo ném projectile cho spell
        public void InitiateThrowForProjectile()
        {
            if (states)
            {
                states.ThrowProjectile();
            }
        }

        // Khởi tạo IK cho khiên
        public void InitIKForShield(bool isLeft)
        {
            ik_handler.UpdateIKTargets((isLeft) ? IKSnapShotType.shield_l : IKSnapShotType.shield_r, isLeft);
        }

        // Khởi tạo IK cho spell dạng thở (breath spell)
        public void InitIKForBreathSpell(bool isLeft)
        {
            ik_handler.UpdateIKTargets((isLeft) ? IKSnapShotType.breath_l : IKSnapShotType.breath_r, isLeft);
        }

        // Mở kiểm soát xoay hướng
        public void OpenRotationControl()
        {
            if (states)
            {
                states.canRotate = true;
            }
            if (eStates)
                eStates.rotateToTarget = true;
        }

        // Đóng kiểm soát xoay hướng
        public void CloseRotationControl()
        {
            if (states)
                states.canRotate = false;
            if (eStates)
                eStates.rotateToTarget = false;
        }

        // Tiêu thụ vật phẩm hiện tại
        public void ConsumeCurrentItem()
        {
            if (states)
                if (states.inventoryManager.curConsumable)
                {
                    states.inventoryManager.curConsumable.itemCount--;
                    ItemEffectManager.singleton.CastEffect(states.inventoryManager.curConsumable.instance.consumableEffect, states);
                }
        }

        // Phát âm thanh hiệu ứng
        public void PlaySoundEffect()
        {
            if (states)
            {
                states.audio_source.PlayOneShot(states.audio_clip);
            }
            if (eStates)
            {
                // Xử lý âm thanh cho địch nếu cần thiết
            }
        }
    }
}
