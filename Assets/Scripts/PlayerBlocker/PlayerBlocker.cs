using UnityEngine;

public class PlayerBlocker : MonoBehaviour
{
    public GameObject blockerArea;  // GameObject dùng để chặn

    private void Start()
    {
        if (blockerArea != null)
        {
            blockerArea.SetActive(false);  // Bắt đầu không chặn
        }
    }

    private void Update()
    {
        // Kiểm tra nếu người chơi gần khu vực chặn thì bật GameObject chặn
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f); // Kiểm tra trong phạm vi bán kính 5 đơn vị
        foreach (var col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                if (blockerArea != null && !blockerArea.activeSelf)
                {
                    blockerArea.SetActive(true);  // Bật khu vực chặn khi người chơi vào
                }
                return;
            }
        }

        // Nếu không còn người chơi trong phạm vi, tắt khu vực chặn
        if (blockerArea != null && blockerArea.activeSelf)
        {
            blockerArea.SetActive(false);
        }
    }
}
