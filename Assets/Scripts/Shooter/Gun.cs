using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject weaponObject; // Object của súng
    public bool hasWeapon = false;  // Biến kiểm tra người chơi có súng không
    private bool isWeaponActive = false; // Trạng thái bật/tắt súng

    void Update()
    {
        // Kiểm tra khi nhấn phím '1'
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleWeapon();
        }
    }

    void ToggleWeapon()
    {
        if (hasWeapon)
        {
            // Nếu đã có súng, bật hoặc tắt súng
            isWeaponActive = !isWeaponActive;
            weaponObject.SetActive(isWeaponActive);

            if (isWeaponActive)
            {
                Debug.Log("Bạn đã lấy súng ra.");
            }
            else
            {
                Debug.Log("Bạn đã cất súng.");
            }
        }
        else
        {
            // Nếu chưa có súng, nhặt súng
            hasWeapon = true;
            isWeaponActive = true;
            weaponObject.SetActive(true);
            Debug.Log("Bạn đã nhặt súng.");
        }
    }
}
