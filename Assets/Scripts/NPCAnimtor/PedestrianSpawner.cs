using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianSpawner : MonoBehaviour
{
    public GameObject[] pedestrianPrefabs;
    public int pedestriansToSpawn;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(1f);
        // Lưu tất cả các waypoint vào danh sách
        List<Transform> availableWaypoints = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            availableWaypoints.Add(transform.GetChild(i));
        }

        int count = 0;
        while (count < pedestriansToSpawn && availableWaypoints.Count > 0)
        {
            // Chọn ngẫu nhiên một prefab từ mảng
            GameObject selectedPrefab = pedestrianPrefabs[Random.Range(0, pedestrianPrefabs.Length)];

            // Chọn ngẫu nhiên một waypoint từ danh sách trống
            int randomIndex = Random.Range(0, availableWaypoints.Count);
            Transform selectedWaypoint = availableWaypoints[randomIndex];

            // Tạo một đối tượng từ prefab được chọn
            GameObject obj = Instantiate(selectedPrefab);
            obj.GetComponent<WaypointNavigator>().currentWaypoint = selectedWaypoint.GetComponent<Waypoint>();
            obj.transform.position = selectedWaypoint.position;

            // Loại bỏ waypoint đã sử dụng khỏi danh sách
            availableWaypoints.RemoveAt(randomIndex);

            yield return new WaitForEndOfFrame();
            count++;
        }
    }

}
