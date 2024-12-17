using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity
{
    // Start is called before the first frame update
    public int HP { get; set; }
    public int Defense { get; set; }
    public int Damage { get; set; }

    public Entity(int hp = 50, int defense = 10, int damage = 5)
    {
        HP = hp;
        Defense = defense;
        Damage = damage;
    }

    // Nhận sát thương
    public void TakeDamage(int incomingDamage)
    {
        int damageTaken = Mathf.Max(0, incomingDamage - Defense); // Trừ sát thương dựa trên phòng thủ
        HP -= damageTaken;
        HP = Mathf.Max(HP, 0); // Đảm bảo HP không âm
    }

}
