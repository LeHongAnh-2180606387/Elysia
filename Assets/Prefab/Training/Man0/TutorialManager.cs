using UnityEngine;
using TMPro;
using System.Collections; // Đảm bảo rằng bạn đã thêm namespace này để sử dụng Coroutine

public class TutorialManager : MonoBehaviour
{
    public TextMeshProUGUI[] tutorialSteps; // Mảng chứa các TextMesh Pro cho từng bước hướng dẫn
    public GameObject secondCutscene; // GameObject chứa cutscene thứ 2
    private int currentStep = 0; // Bước hiện tại
    private int clickCount = 0; // Đếm số lần click chuột trong các bước combo

    void Start()
    {
        // Ẩn tất cả các TextMesh Pro, chỉ hiển thị bước đầu tiên
        for (int i = 0; i < tutorialSteps.Length; i++)
        {
            tutorialSteps[i].gameObject.SetActive(false);
        }
        tutorialSteps[0].gameObject.SetActive(true); // Hiển thị bước đầu tiên
    }

    void Update()
    {
        switch (currentStep)
        {
            case 0: // Bước 1: W, A, S, D để di chuyển
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
                    Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
                {
                    CompleteStep();
                }
                break;

            case 1: // Bước 2: Nhấn Space để nhảy
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    CompleteStep();
                }
                break;

            case 2: // Bước 3: Ctrl để ngồi
                if (Input.GetKeyDown(KeyCode.LeftControl))
                {
                    CompleteStep();
                }
                break;

            case 3: // Bước 4: Ctrl trái + chuột trái để đấm ngồi
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
                {
                    CompleteStep();
                }
                break;

            case 4: // Bước 5: Ctrl trái + chuột phải để đá ngồi
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(1))
                {
                    CompleteStep();
                }
                break;

            case 5: // Bước 6: Click chuột trái 3 lần để đánh combo đấm
                if (Input.GetMouseButtonDown(0))
                {
                    clickCount++;
                    if (clickCount == 3)
                    {
                        clickCount = 0; // Reset đếm
                        CompleteStep();
                    }
                }
                break;

            case 6: // Bước 7: Click chuột phải 3 lần để đánh combo đá
                if (Input.GetMouseButtonDown(1))
                {
                    clickCount++;
                    if (clickCount == 3)
                    {
                        clickCount = 0; // Reset đếm
                        CompleteStep();
                    }
                }
                break;

            case 7: // Bước 8: Click 2 lần chuột trái + 1 lần chuột phải để thực hiện combo phối hợp
                if (Input.GetMouseButtonDown(0) && clickCount < 2)
                {
                    clickCount++;
                }
                else if (Input.GetMouseButtonDown(1) && clickCount == 2)
                {
                    clickCount = 0; // Reset đếm
                    CompleteStep();
                }
                break;
        }
    }

    void CompleteStep()
    {
        // Ẩn TextMesh Pro hiện tại
        tutorialSteps[currentStep].gameObject.SetActive(false);

        // Chuyển sang bước tiếp theo
        currentStep++;

        // Hiển thị TextMesh Pro của bước tiếp theo (nếu có)
        if (currentStep < tutorialSteps.Length)
        {
            tutorialSteps[currentStep].gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Hoàn thành tất cả các bước!");
            StartCoroutine(TriggerSecondCutsceneWithDelay()); // Gọi Coroutine với delay
        }
    }

    // Coroutine để chờ 2 giây trước khi kích hoạt cutscene thứ 2
    IEnumerator TriggerSecondCutsceneWithDelay()
    {
        yield return new WaitForSeconds(2f); // Chờ 2 giây
        TriggerSecondCutscene(); // Kích hoạt cutscene thứ 2
    }

    void TriggerSecondCutscene()
    {
        Debug.Log("Kích hoạt cutscene thứ 2...");
        if (secondCutscene != null)
        {
            secondCutscene.SetActive(true); // Kích hoạt cutscene
        }
    }
}
