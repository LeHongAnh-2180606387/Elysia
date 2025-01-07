
using System;
using System.Collections;
using Systems.Hero.Model;
using Systems.SaveLoad.Manager;
using Systems.Scriptable.Events;

using UnityEngine;
using UnityEngine.SceneManagement;
namespace Systems.Hero.Manager
{
    public class PlayerDataManager : PersistentSingleton<PlayerDataManager>
    {

        public PlayerData playerData;
        public int health;
        public int shield;
        public int energy;

        public bool isModified = false;
        private Vector3 lastPosition;

        [SerializeField] public GameObject prefabPlayerHero; // Tham chiếu tới Hero của người chơi
        public GameObject playerHero;
        protected override void Awake()
        {
            base.Awake();
        }
        private void Start()
        {

        }

        private void Update()
        {
            if (playerData != null && playerHero != null && GameManager.Instance.isPlayingInMap)
            {
                Vector3 currentPosition = playerHero.transform.position;
                Quaternion currentRotation = playerHero.transform.rotation;
                if (currentPosition != lastPosition)
                {
                    lastPosition = currentPosition;
                    UpdatePlayerPositionRotation(currentPosition, currentRotation);
                }
                //Chỉ lưu các Scene đối tượng Player có thể tham gia, không lưu Scene UI
                if (playerData.scence != SceneManager.GetActiveScene().name)
                {
                    playerData.scence = SceneManager.GetActiveScene().name;
                    SaveLoadManager.Instance.saveLoadLocalService.trackableService.UpdateChange("playerData", true);
                }
            }
        }
        private void OnEnable()
        {
            //UIManager
            Observer.Instance.AddObserver("onUpgradePlayerScene", OnUpgradePlayerScene);
            //GameManager
            Observer.Instance.AddObserver("onPlayGame", OnPlayGame);
            Observer.Instance.AddObserver("onLeaveGame", OnLeaveGame);
        }
        private void OnDisable()
        {
            //UIManager
            Observer.Instance.RemoveListener("onUpgradePlayerScene", OnUpgradePlayerScene);
            //GameManager
            Observer.Instance.RemoveListener("onPlayGame", OnPlayGame);
            Observer.Instance.RemoveListener("onLeaveGame", OnLeaveGame);
        }

        private void OnUpgradePlayerScene(object[] obj)
        {
            playerData.scence = (string)obj[0];
            Debug.Log($"PlayerDataManager: {playerData.scence}");
            if (isModified == true) return;
            isModified = true;
            SaveLoadManager.Instance.saveLoadLocalService.trackableService.UpdateChange("playerData", true);
        }

        public void UpdatePlayerPositionRotation(Vector3 position, Quaternion rotation = default)
        {
            playerData.position = position;
            playerData.rotation = rotation;
            if (isModified == true) return;
            isModified = true;
            SaveLoadManager.Instance.saveLoadLocalService.trackableService.UpdateChange("playerData", true);
        }


        private void OnPlayGame(object[] obj)
        {
            // playerHero = GameObject.FindWithTag("PlayerHero");            
            // Set position and rotation first
            Debug.Log($"OnPlayGame");
            StartCoroutine(ActivatePlayerHeroWithDelay());
        }

        private IEnumerator ActivatePlayerHeroWithDelay()
        {
            // Đợi 3 giây
            yield return new WaitForSeconds(3f);
            
            // Đang chờ sử lý thuộc tính tương tác với Item
            health = playerData.maxHealth;
            shield = playerData.maxShield;
            energy = playerData.maxEnergy;
            
            if (playerHero == null)
            {
                if (playerData.level == 0 && playerData.scence == "Training")
                {
                    // Training Scene là Scene cho người mới chơi
                    // Đã sửa nhưng chưa thành công
                    Debug.Log($"NewPlayer: {playerData.level} - {playerData.scence}");

                    ScenePositionSpawn scenePositionSpawn = new ScenePositionSpawn();
                    Debug.Log($"Position Spawn: {scenePositionSpawn.getTrainingNewPlayerPosition()}");
                    
                    playerData.position = scenePositionSpawn.getTrainingNewPlayerPosition();
                    playerHero = Instantiate(prefabPlayerHero, scenePositionSpawn.getTrainingNewPlayerPosition(), Quaternion.identity);

                    GameObject.FindGameObjectWithTag("Custence 0").SetActive(true);
                }

                if (playerData.level == 0 && playerData.scence != "Training")
                {
                    Debug.Log($"Test");
                    Debug.Log($"PlayerDataManger: {playerData.position}");
                    playerHero = Instantiate(prefabPlayerHero, playerData.position, playerData.rotation);
                }
                else if (playerData.scence != null)
                {
                    Debug.Log($"PlayerDataManger: {playerData.position}");
                    playerHero = Instantiate(prefabPlayerHero, playerData.position, playerData.rotation);
                }
            }
            if (playerData != null && playerHero != null)
            {
                // Bật GameObject sau khi cập nhật vị trí
                playerHero.SetActive(true);

                // Cập nhật vị trí từ dữ liệu đã lưu
                playerHero.transform.position = playerData.position;
                playerHero.transform.rotation = playerData.rotation;
            }

        }

        private void OnLeaveGame(object[] obj)
        {
            // if (playerHero != null)
            // {
            //     playerHero.SetActive(false); // Tắt Hero khi rời khỏi trò chơi
            // }    
            Destroy(playerHero);
        }

    }
}
