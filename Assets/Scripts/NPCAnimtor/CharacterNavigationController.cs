using UnityEngine;

public class CharacterNavigationController : MonoBehaviour
{
    [Header("Cài Đặt Di Chuyển")]
    public float movementSpeed = 1f;       // Tốc độ di chuyển của nhân vật
    public float rotationSpeed = 120f;    // Tốc độ xoay của nhân vật
    public float stopDistance = 2.5f;     // Khoảng cách dừng lại khi đến gần điểm đến

    [Header("Cài Đặt Điểm Đến")]
    public Vector3 destination;           // Vị trí điểm đến
    public bool reachedDestination;       // Kiểm tra xem đã đến điểm đến hay chưa

    private void Update()
    {
        // Tính khoảng cách tới điểm đến
        float distanceToDestination = Vector3.Distance(transform.position, destination);

        // Kiểm tra xem đã đến điểm đến chưa
        if (distanceToDestination <= stopDistance)
        {
            reachedDestination = true;
            return; // Ngừng di chuyển
        }
        else
        {
            reachedDestination = false;
        }

        // Di chuyển nhân vật về phía điểm đến
        Vector3 direction = (destination - transform.position).normalized;
        transform.position += direction * movementSpeed * Time.deltaTime;

        // Xoay nhân vật hướng về phía điểm đến
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // Vẽ gizmo trong Editor để hiển thị vị trí điểm đến
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(destination, 0.5f);
    }

    public void SetDestination(Vector3 destiantion)
    {
        this.destination = destiantion;
        reachedDestination = false ;
    }
}
