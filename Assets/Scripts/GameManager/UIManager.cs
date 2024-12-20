
using System.Collections;
using Systems.Account.Enum;
using Systems.Account.Manager;
using Systems.Account.Model;
using Systems.Hero.Manager;
using Systems.Hero.Model;
using Systems.Scriptable.Events;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    private GameObject nameUser;
    public float timeWaiting;
    Scene sceneNow;
    public TextMeshProUGUI labPointHP;
    public TextMeshProUGUI labPointPower;
    public TextMeshProUGUI labPointEnergy;
    public TextMeshProUGUI labPointDefense;
    public TextMeshProUGUI labPointMoney;
    public GameObject statusPanel; // Tham chiếu đến Panel của bảng trạng thái

    public TextMeshProUGUI labHealthBar;
    public TextMeshProUGUI labEnergyBar;
    public TextMeshProUGUI labShieldBar;
    public GameObject sliderHealthBar;
    public GameObject sliderEnergyBar;
    public GameObject sliderShieldBar;

    public float sliderValueHealthBar;
    public float sliderValueEnergyBar;
    public float sliderValueShieldBar;

    public void Start()
    {
        sceneNow = SceneManager.GetActiveScene();
        // StartCoroutine(StartConnectUIStatus());
        if (sceneNow.name == "City" || sceneNow.name == "Training" || sceneNow.name == "Room")
        {
            labHealthBar = GameObject.Find("labHealthBar").GetComponent<TextMeshProUGUI>();
            labEnergyBar = GameObject.Find("labEnergyBar").GetComponent<TextMeshProUGUI>();
            labShieldBar = GameObject.Find("labShieldBar").GetComponent<TextMeshProUGUI>();

            sliderHealthBar = GameObject.Find("sliderHealthBar");
            sliderEnergyBar = GameObject.Find("sliderEnergyBar");
            sliderShieldBar = GameObject.Find("sliderShieldBar");
        }

        StartCoroutine(ShowNameUser());
        sceneNow = SceneManager.GetActiveScene();
        Observer.Instance.Notify("onSceneName", sceneNow.name);

        // Bổ sung thêm các Scene có chức năng điều khiển người chơi
        if (sceneNow.name == "City" || sceneNow.name == "Training" || sceneNow.name == "Room")
        {
            GameManager.Instance.PlayGame();
        }

        // Debug.Log($"{sceneNow.name}");
        if (sceneNow.name == "Loading Scence")
            StartCoroutine(SwapSceneAfterLoading(timeWaiting));
    }
    public void FixedUpdate()
    {
        if (sceneNow.name == "City" || sceneNow.name == "Training" || sceneNow.name == "Room")
        {
            sliderValueHealthBar = (float)PlayerDataManager.Instance.health / PlayerDataManager.Instance.playerData.maxHealth;
            sliderValueEnergyBar = (float)PlayerDataManager.Instance.energy / PlayerDataManager.Instance.playerData.maxEnergy;
            sliderValueShieldBar = (float)PlayerDataManager.Instance.shield / PlayerDataManager.Instance.playerData.maxShield;
        }
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (sceneNow.name == "City" || sceneNow.name == "Training" || sceneNow.name == "Room")
            {
                // Chuyển đổi trạng thái hoạt động của bảng trạng thái
                statusPanel.SetActive(!statusPanel.activeSelf);

                if (labPointHP == null)
                {
                    labPointHP = GameObject.Find("labPointHP").GetComponent<TextMeshProUGUI>();
                    labPointPower = GameObject.Find("labPointPower").GetComponent<TextMeshProUGUI>();
                    labPointEnergy = GameObject.Find("labPointEnergy").GetComponent<TextMeshProUGUI>();
                    labPointDefense = GameObject.Find("labPointDefense").GetComponent<TextMeshProUGUI>();
                    labPointMoney = GameObject.Find("labPointMoney").GetComponent<TextMeshProUGUI>();
                }
            }

        }
    }
    public void LateUpdate()
    {
        if (sceneNow.name == "City" || sceneNow.name == "Training")
        {
            if (labHealthBar != null)
            {
                labHealthBar.text = PlayerDataManager.Instance.health.ToString();
                labEnergyBar.text = PlayerDataManager.Instance.energy.ToString();
                labShieldBar.text = PlayerDataManager.Instance.shield.ToString();
            }
            if (sliderHealthBar != null)
            {
                sliderHealthBar.GetComponent<Slider>().value = sliderValueHealthBar;
                sliderEnergyBar.GetComponent<Slider>().value = sliderValueEnergyBar;
                sliderShieldBar.GetComponent<Slider>().value = sliderValueShieldBar;
            }
            if (labPointHP != null)
            {
                labPointHP.text = PlayerDataManager.Instance.playerData.maxHealth.ToString();
                labPointPower.text = PlayerDataManager.Instance.playerData.attack.ToString();
                labPointEnergy.text = PlayerDataManager.Instance.playerData.maxEnergy.ToString();
                labPointDefense.text = PlayerDataManager.Instance.playerData.defense.ToString();
                labPointMoney.text = PlayerDataManager.Instance.playerData.coin.ToString();
            }
        }
    }
    // Basic UI
    public void SwapScene(string nameAfterScene)
    {
        GameManager.Instance.afterScene = nameAfterScene;
        SceneManager.LoadScene(nameAfterScene);
    }
    public void SwapSceneAtPosition(string nameAfterScene, Vector3 position)
    {
        PlayerDataManager.Instance.UpdatePlayerPositionRotation(position);
        GameManager.Instance.afterScene = nameAfterScene;
        SceneManager.LoadScene(nameAfterScene);
    }
    public void SwapSceneNoSaveBeforeScene(string nameAfterScene)
    {
        GameManager.Instance.afterScene = nameAfterScene;
        SceneManager.LoadScene(nameAfterScene);
    }
    public void SwapSceneNoSaveAfterScene(string nameAfterScene)
    {
        GameManager.Instance.beforeScene = sceneNow.name;
        SceneManager.LoadScene(nameAfterScene);
    }
    // Loading Scene UI
    public void SwapSceneLoading(string nameScenceAfterLoading)
    {
        GameManager.Instance.afterScene = nameScenceAfterLoading;
        SceneManager.LoadScene("Loading Scence");
    }
    public void SwapSceneInPlayerDataLoading()
    {
        string sceneName = PlayerDataManager.Instance.playerData.scence;
        if (sceneName == null)
        {
            //Training là Scene cho người mới chơi
            sceneName = "Training";
        }
        GameManager.Instance.afterScene = sceneName;
        SceneManager.LoadScene("Loading Scence");
    }
    IEnumerator SwapSceneAfterLoading(float timeWaiting = 0)
    {
        yield return new WaitForSeconds(timeWaiting);
        SceneManager.LoadSceneAsync(GameManager.Instance.afterScene);
    }
    // User UI
    public IEnumerator ShowNameUser()
    {
        yield return new WaitForSeconds(1f);
        nameUser = GameObject.Find("labNameUser");

        if (nameUser != null)
        {
            TextMeshProUGUI textMeshProUGUI = nameUser.GetComponent<TextMeshProUGUI>();
            if (textMeshProUGUI != null)
            {
                textMeshProUGUI.text = AccountManager.Instance.accountData.name;
            }
        }
    }
    // Play UI
    public void PlayAnonymousAccountLogin()
    {
        GameManager.Instance.beforeScene = sceneNow.name;
        GameManager.Instance.afterScene = "MenuGame";
        AccountManager.Instance.accountService.SetAttributeAnonymousAccount();
        SceneManager.LoadScene("MenuGame");
    }

    public void PlayAccountLogin()
    {
        GameManager.Instance.beforeScene = sceneNow.name;

        if (SignInResult.AccountType == AccountType.Player)
        {
            GameManager.Instance.afterScene = "MenuGame";
            // AccountManager.Instance.accountService.SetAttributeAccount();
            SceneManager.LoadScene("MenuGame");
        }
        else
        {
            GameManager.Instance.afterScene = "Login";
            SceneManager.LoadScene("Login");
        }
    }
    // Login UI
    public void LoginAnonymousAccount()
    {
        GameManager.Instance.afterScene = "MenuGame";
        AccountManager.Instance.accountService.SetAttributeAnonymousAccount();
        SceneManager.LoadScene("MenuGame");
    }
    public void LoginAccount()
    {
        //Kiểm tra xem đã đăng nhập thành công hay chưa
        if (false)
        {
            GameManager.Instance.afterScene = "MenuGame";
            AccountManager.Instance.accountService.SetAttributeAccount();
            SceneManager.LoadScene("MenuGame");
        }
    }
    // Back UI
    public void BackScene()
    {
        SceneManager.LoadScene(GameManager.Instance.beforeScene);
    }
    // Exit UI
    public void ExitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Thoát game trong Unity Editor!"); // Debug để kiểm tra
        EditorApplication.isPlaying = false; // Dừng chế độ Play
#else
        Debug.Log("Thoát game!"); // Debug để kiểm tra
        Application.Quit();
#endif
    }
}