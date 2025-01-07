using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SA
{
    public class UIManager : MonoBehaviour
    {
        public float lerpSpeed; // Tốc độ chuyển động mượt mà cho các thanh trượt (sliders).
        public Slider health; // Thanh trượt cho sức khỏe.
        public Slider h_vis; // Thanh trượt hiển thị sức khỏe.
        public Slider focus; // Thanh trượt cho sự tập trung.
        public Slider f_vis; // Thanh trượt hiển thị sự tập trung.
        public Slider stamina; // Thanh trượt cho sức bền.
        public Slider s_vis; // Thanh trượt hiển thị sức bền.

        public Text souls; // Text hiển thị số linh hồn hiện tại.
        public Text itemCount; // Text hiển thị số lượng vật phẩm hiện tại.
        public float sizeMultiplier; // Hệ số nhân để điều chỉnh kích thước thanh trượt.
        int curSouls; // Số linh hồn hiện tại (biến nội bộ).
        int curItemCount; // Số lượng vật phẩm hiện tại (biến nội bộ).

        public GameObject interactCard; // Card hiển thị các hành động tương tác.
        public Text ac_action_type; // Text hiển thị loại hành động tương tác.

        int ac_index; // Chỉ số cho các thẻ thông báo.
        public List<AnnounceCard> announceCard; // Danh sách các thẻ thông báo.

        //-----------------------------------------------------------------

        void Start()
        {
            // Đóng thẻ tương tác và thẻ thông báo khi bắt đầu.
            CloseInteractCard();
            CloseAnnounceCard();
        }

        public void InitSouls(int v)
        {
            // Khởi tạo số linh hồn.
            curSouls = v;
        }

        public void InitSlider(StatSlider t, int value)
        {
            // Khởi tạo thanh trượt và thanh hiển thị với giá trị mới.
            Slider s = null;
            Slider v = null;

            // Chọn thanh trượt và thanh hiển thị dựa trên loại thống kê.
            switch (t)
            {
                case StatSlider.health:
                    s = health;
                    v = h_vis;
                    break;
                case StatSlider.focus:
                    s = focus;
                    v = f_vis;
                    break;
                case StatSlider.stamina:
                    s = stamina;
                    v = s_vis;
                    break;
                default:
                    break;
            }

            // Cập nhật giá trị tối đa cho thanh trượt và thanh hiển thị.
            s.maxValue = value;
            v.maxValue = value;
            RectTransform r = s.GetComponent<RectTransform>();
            RectTransform r_v = v.GetComponent<RectTransform>();

            // Điều chỉnh chiều rộng của thanh trượt.
            float value_actual = value * sizeMultiplier;
            value_actual = Mathf.Clamp(value_actual, 0, 1000);

            // Đặt chiều rộng đã điều chỉnh cho thanh trượt.
            r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value_actual);
            r_v.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value_actual);
        }

        public void Tick(CharacterStats stats, float delta, StateManager states)
        {
            // Cập nhật giá trị thanh trượt và text dựa trên trạng thái nhân vật và thời gian delta.
            health.value = Mathf.Lerp(health.value, stats._health, delta * lerpSpeed * 2);
            focus.value = Mathf.Lerp(focus.value, stats._focus, delta * lerpSpeed * 2);
            stamina.value = stats._stamina;

            curSouls = Mathf.RoundToInt(Mathf.Lerp(curSouls, stats._souls, delta * lerpSpeed * 10));
            souls.text = curSouls.ToString();
            itemCount.text = states.inventoryManager.curConsumable.itemCount.ToString();

            h_vis.value = Mathf.Lerp(h_vis.value, stats._health, delta * lerpSpeed);
            f_vis.value = Mathf.Lerp(f_vis.value, stats._focus, delta * lerpSpeed);
            s_vis.value = Mathf.Lerp(s_vis.value, stats._stamina, delta * lerpSpeed);
        }

        public void AffectAll(int h, int f, int s)
        {
            // Cập nhật tất cả các thanh trượt với giá trị mới.
            InitSlider(StatSlider.health, h);
            InitSlider(StatSlider.focus, f);
            InitSlider(StatSlider.stamina, s);
        }

        public void OpenInteractCard(UIActionType type)
        {
            // Mở thẻ tương tác với loại hành động được chỉ định.
            switch (type)
            {
                case UIActionType.pickup:
                    ac_action_type.text = "Pick Up: Space";
                    break;
                case UIActionType.interact:
                    ac_action_type.text = "Interact: Space";
                    break;
                case UIActionType.open:
                    ac_action_type.text = "Open: Space";
                    break;
                case UIActionType.talk:
                    ac_action_type.text = "Talk: Space";
                    break;
                default:
                    break;
            }

            interactCard.SetActive(true);
        }

        public void CloseInteractCard()
        {
            // Đóng thẻ tương tác.
            interactCard.SetActive(false);
        }

        public void AddAnnounceCard(Item i)
        {
            // Thêm thẻ thông báo với thông tin vật phẩm mới.
            announceCard[ac_index].itemName.text = i.itemName;
            announceCard[ac_index].icon.sprite = i.icon;
            announceCard[ac_index].gameObject.SetActive(true);
            ac_index++;
            if (ac_index > 4)
            {
                ac_index = 0;
            }
        }

        public void CloseAnnounceCard()
        {
            // Đóng tất cả các thẻ thông báo.
            for (int i = 0; i < announceCard.Count; i++)
            {
                announceCard[i].gameObject.SetActive(false);
            }
        }

        public static UIManager singleton; // Biến singleton cho lớp UIManager.

        void Awake()
        {
            // Khởi tạo singleton.
            singleton = this;
        }
    }

    public enum StatSlider
    {
        health, focus, stamina
    }

    public enum UIActionType
    {
        pickup, interact, open, talk
    }
}
