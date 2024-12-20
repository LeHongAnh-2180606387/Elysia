using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class PickableItemsManager : MonoBehaviour
    {
        public List<WorldInteraction> interactions = new List<WorldInteraction>(); // Danh sách các tương tác với thế giới
        public List<PickableItem> pick_items = new List<PickableItem>(); // Danh sách các vật phẩm có thể nhặt
        public PickableItem itemCandidate; // Vật phẩm hiện tại đang được chọn
        public WorldInteraction interactionCandidate; // Tương tác hiện tại đang được chọn

        int frameCount; // Số khung hình đã trôi qua
        public int frameCheck = 15; // Số khung hình để kiểm tra

        // Phương thức được gọi mỗi khung hình
        public void Tick()
        {
            if (frameCount < frameCheck)
            {
                frameCount++;
                return;
            }
            frameCount = 0;

            // Kiểm tra vật phẩm có thể nhặt
            for (int i = 0; i < pick_items.Count; i++)
            {
                float distance = Vector3.Distance(pick_items[i].transform.position, transform.position);

                if (distance < 2)
                {
                    itemCandidate = pick_items[i];
                }
                else
                {
                    if (itemCandidate == pick_items[i])
                        itemCandidate = null;
                }
            }

            // Kiểm tra tương tác với thế giới
            for (int i = 0; i < interactions.Count; i++)
            {
                float d = Vector3.Distance(interactions[i].transform.position, transform.position);
                if (d < 2)
                {
                    interactionCandidate = interactions[i];
                }
                else
                {
                    if (interactionCandidate == interactions[i])
                        interactionCandidate = null;
                }
            }
        }

        // Nhặt vật phẩm hiện tại
        public void PickCandidate(StateManager states)
        {
            if (itemCandidate == null)
                return;

            for (int i = 0; i < itemCandidate.items.Length; i++)
            {
                PickItemContainer c = itemCandidate.items[i];

                AddItem(c.itemId, c.itemType, states);
            }

            if (pick_items.Contains(itemCandidate))
                pick_items.Remove(itemCandidate);

            Destroy(itemCandidate.gameObject); // Xóa vật phẩm khỏi thế giới
            itemCandidate = null;
        }

        // Thêm vật phẩm vào kho
        void AddItem(string id, ItemType type, StateManager states)
        {
            InventoryManager inv = states.inventoryManager;
            switch (type)
            {
                case ItemType.weapon:
                    for (int k = 0; k < inv.r_r_weapons.Count; k++)
                    {
                        if (id == inv.r_r_weapons[k].name)
                        {
                            Item b = ResourceManager.singleton.GetItem(id);
                            UIManager.singleton.AddAnnounceCard(b);
                            return;
                        }
                    }
                    inv.WeaponToRuntimeWeapon(ResourceManager.singleton.GetWeapon(id));
                    inv.WeaponToRuntimeWeapon(ResourceManager.singleton.GetWeapon(id), true);
                    break;
                case ItemType.item:
                    for (int j = 0; j < inv.r_consum.Count; j++)
                    {
                        if (id == inv.r_consum[j].name)
                        {
                            inv.r_consum[j].itemCount++;
                            Item b = ResourceManager.singleton.GetItem(id);
                            UIManager.singleton.AddAnnounceCard(b);
                            return;
                        }
                    }
                    inv.ConsumableToRuntimeConsumable(ResourceManager.singleton.GetConsumable(id));
                    break;
                case ItemType.spell:
                    for (int k = 0; k < inv.r_spells.Count; k++)
                    {
                        if (id == inv.r_spells[k].name)
                        {
                            Item b = ResourceManager.singleton.GetItem(id);
                            UIManager.singleton.AddAnnounceCard(b);
                            return;
                        }
                    }
                    inv.SpellToRuntimeSpell(ResourceManager.singleton.GetSpell(id));
                    break;
            }

            // Thêm vật phẩm vào kho nếu không tìm thấy
            Item i = ResourceManager.singleton.GetItem(id);
            UIManager.singleton.AddAnnounceCard(i);
        }
    }
}
