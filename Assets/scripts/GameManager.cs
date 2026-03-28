using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [Header("UI")]
    public GameObject playerUIRoot;
    public GameObject shopUIRoot;

    GameObject currentPlayer;
    CameraController cameraController;
    GameObject currentShopNPC;
    RaycastHit hit;
    GameObject currentEnemy;
    

    [Header("Player")]
    public GameObject playerPrefab;
    public Transform PlayerSpawnPoint;
   

    [Header("Enemy")]
    public GameObject enemyPrefab;
    public Transform enemySpawnPoint;

    [Header("Shop")]
    public GameObject PrefabShopNPC;
    public GameObject[] SpawnPoints;

    [Header("Items - FIXED")]
    public Transform[] FixedItemSpawnPoints;
    public GameObject[] FixedItems;

    [Header("Items - RANDOM")]
    public Transform[] RandomItemSpawnPoints;
    public GameObject[] RandomItemList;

    [Header("Plank")]
    public GameObject plankspawnet;
    public GameObject gorunmezplankdespawnet;    

    [Header("Background noises")]
    [SerializeField] AudioSource ambientSource;
    [SerializeField] AudioSource randomSource;
    [SerializeField] AudioClip[] randomClips;

    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        

        SpawnPlayer();
        SpawnFixedItems();
        SpawnRandomItems();
        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpawnShop());
        StartCoroutine(BackgroundNoises());
    }

    IEnumerator BackgroundNoises()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f, 15f));

            
            if (!randomSource.isPlaying && Random.value > 0.4f)
            {
                int index = Random.Range(0, randomClips.Length);
                randomSource.PlayOneShot(randomClips[index]);
            }
        }
    }

    void SpawnPlayer()
    {
        currentPlayer = Instantiate(playerPrefab, PlayerSpawnPoint.position, PlayerSpawnPoint.rotation);

        if (cameraController != null)
        {
            cameraController.playerBody = currentPlayer.transform;
            cameraController.MouseSensitivity = PlayerPrefs.GetFloat("sensitivity", 400f);
        }
    }

    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(5f);

        currentEnemy = Instantiate(enemyPrefab, enemySpawnPoint.position, enemySpawnPoint.rotation);

        EnemyController ec = currentEnemy.GetComponent<EnemyController>();

        // Child objede Animator varsa bunu al
        ec.animator = currentEnemy.GetComponentInChildren<Animator>();

        if (ec.animator == null)
            Debug.LogError("Animator bulunamadı! Prefab kontrol et.");

        // 1 frame bekle
        yield return null;

        
    }

    void SpawnFixedItems()
    {
        if (FixedItemSpawnPoints.Length != FixedItems.Length)
        {
            Debug.LogError("FixedItemSpawnPoints ve FixedItems sayısı eşit olmalı!");
            return;
        }

        for (int i = 0; i < FixedItemSpawnPoints.Length; i++)
        {
            if (FixedItems[i] != null && FixedItemSpawnPoints[i] != null)
            {
                Instantiate(FixedItems[i], FixedItemSpawnPoints[i].position, FixedItemSpawnPoints[i].rotation);
            }
        }
    }
    void SpawnRandomItems()
    {
        if (RandomItemList.Length == 0 || RandomItemSpawnPoints.Length == 0) return;

        var tempList = new System.Collections.Generic.List<GameObject>(RandomItemList);

        foreach (Transform spawn in RandomItemSpawnPoints)
        {
            if (tempList.Count == 0) break;

            int rand = Random.Range(0, tempList.Count);
            GameObject selectedItem = tempList[rand];
            Instantiate(selectedItem, spawn.position, spawn.rotation);

            tempList.RemoveAt(rand);
        }
    }
    IEnumerator SpawnShop()
    {
        while (true)
        {
            if (currentShopNPC != null) Destroy(currentShopNPC);

            int index = Random.Range(0, SpawnPoints.Length);
            Transform spawn = SpawnPoints[index].transform;

            currentShopNPC = Instantiate(PrefabShopNPC, spawn.position, spawn.rotation);

            yield return new WaitForSeconds(60f);
        }
    }
    

    // ================= SHOP UI ================
    public void OpenShopUI()
    {
        shopUIRoot.SetActive(true);    // player UI KAPANMAZ
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
    }
    public void CloseShopUI()
    {
        shopUIRoot.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }   
    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
  
}
