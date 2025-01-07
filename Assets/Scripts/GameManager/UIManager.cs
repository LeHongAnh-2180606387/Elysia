using System.Collections;
using Systems.Account.Enum;
using Systems.Account.Manager;
using Systems.Hero.Manager;
using Systems.Scriptable.Events;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public float timeWaiting;

    // UI Elements
    public TextMeshProUGUI labPointHP;
    public TextMeshProUGUI labPointPower;
    public TextMeshProUGUI labPointEnergy;
    public TextMeshProUGUI labPointDefense;
    public TextMeshProUGUI labPointMoney;
    public GameObject statusPanel;

    public TextMeshProUGUI labHealthBar;
    public TextMeshProUGUI labEnergyBar;
    public TextMeshProUGUI labShieldBar;
    public Slider sliderHealthBar;
    public Slider sliderEnergyBar;
    public Slider sliderShieldBar;

    private Scene currentScene;
    public GUIManager guiManager;

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene();
        Observer.Instance.Notify("onSceneName", currentScene.name);

        InitializeSceneSpecificUI();
        StartCoroutine(ShowNameUser());

        if (currentScene.name == "City" || currentScene.name == "Training" || currentScene.name == "Room")
        {
            GameManager.Instance.PlayGame();
        }
        else if (currentScene.name == "Loading Scence")
        {
            StartCoroutine(SwapSceneAfterLoading(timeWaiting));
        }
    }

    private void FixedUpdate()
    {
        if (IsGameplayScene())
        {
            UpdatePlayerStats();
        }
    }

    private void Update()
    {
        if (IsGameplayScene() && guiManager != null)
        {
            UpdateUIElements();
        }
    }

    private void LateUpdate()
    {
        if (IsGameplayScene())
        {
            UpdateLabelsAndSliders();
        }
    }

    private bool IsGameplayScene()
    {
        return currentScene.name == "City" || currentScene.name == "Training" || currentScene.name == "Room";
    }

    private void InitializeSceneSpecificUI()
    {
        if (IsGameplayScene())
        {
            labHealthBar = FindUIElement<TextMeshProUGUI>("labHealthBar");
            labEnergyBar = FindUIElement<TextMeshProUGUI>("labEnergyBar");
            labShieldBar = FindUIElement<TextMeshProUGUI>("labEXPBar");

            sliderHealthBar = FindUIElement<Slider>("sliderHealthBar");
            sliderEnergyBar = FindUIElement<Slider>("sliderEnergyBar");
            sliderShieldBar = FindUIElement<Slider>("sliderEXPBar");
        }
    }

    private T FindUIElement<T>(string name) where T : Component
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null)
        {
            return obj.GetComponent<T>();
        }
        Debug.LogWarning($"UI Element '{name}' not found.");
        return null;
    }

    private void UpdatePlayerStats()
    {
        var playerData = PlayerDataManager.Instance.playerData;
        sliderHealthBar.value = PlayerDataManager.Instance.health / (float)playerData.maxHealth;
        sliderEnergyBar.value = PlayerDataManager.Instance.energy / (float)playerData.maxEnergy;
        sliderShieldBar.value = PlayerDataManager.Instance.shield / (float)playerData.maxShield;
    }

    private void UpdateUIElements()
    {
        sliderHealthBar.value = guiManager.currentHealth / guiManager.maxHealth;
        sliderEnergyBar.value = guiManager.currentStamina / guiManager.maxStamina;
        sliderShieldBar.value = guiManager.currentXP; // Assuming Shield is being used as XP

        labHealthBar.text = guiManager.currentHealth.ToString();
        labEnergyBar.text = guiManager.currentStamina.ToString();
        labShieldBar.text = guiManager.currentXP.ToString();
    }

    private void UpdateLabelsAndSliders()
    {
        if (labPointHP != null)
        {
            var playerData = PlayerDataManager.Instance.playerData;

            labPointHP.text = guiManager.maxHealth.ToString();
            labPointPower.text = playerData.attack.ToString();
            labPointEnergy.text = guiManager.maxStamina.ToString();
            labPointDefense.text = playerData.defense.ToString();
            labPointMoney.text = playerData.coin.ToString();
        }
    }

    // Scene Management
    public void SwapScene(string sceneName)
    {
        GameManager.Instance.afterScene = sceneName;
        SceneManager.LoadScene(sceneName);
    }

    public void SwapSceneWithPosition(string sceneName, Vector3 position)
    {
        PlayerDataManager.Instance.UpdatePlayerPositionRotation(position);
        SwapScene(sceneName);
    }

    public void SwapSceneWithoutSaveBefore(string sceneName)
    {
        GameManager.Instance.afterScene = sceneName;
        SceneManager.LoadScene(sceneName);
    }

    public void SwapSceneWithoutSaveAfter(string sceneName)
    {
        GameManager.Instance.beforeScene = currentScene.name;
        SceneManager.LoadScene(sceneName);
    }

    public void SwapSceneLoading(string sceneName)
    {
        GameManager.Instance.afterScene = sceneName;
        SceneManager.LoadScene("Loading Scence");
    }

    public IEnumerator SwapSceneAfterLoading(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadSceneAsync(GameManager.Instance.afterScene);
    }

    public IEnumerator ShowNameUser()
    {
        yield return new WaitForSeconds(1f);
        var userLabel = FindUIElement<TextMeshProUGUI>("labNameUser");
        if (userLabel != null)
        {
            userLabel.text = AccountManager.Instance.accountData.name;
        }
    }
    public void SwapSceneAtPosition(string nameAfterScene, Vector3 position)
    {
        PlayerDataManager.Instance.UpdatePlayerPositionRotation(position);
        GameManager.Instance.afterScene = nameAfterScene;
        SceneManager.LoadScene(nameAfterScene);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Exiting game in Unity Editor.");
        EditorApplication.isPlaying = false;
#else
        Debug.Log("Exiting game.");
        Application.Quit();
#endif
    }
}
