using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_PlayerUI1 : MonoBehaviour
{
    public static UI_PlayerUI1 Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    RaycastHit hit;

    [SerializeField] GameObject pausePanel;
    public GameObject DeathScreen;
    [SerializeField] Transform childObjectText;
    [SerializeField] Transform childObjectText2;
    [SerializeField] Transform childObjectText3;
    [SerializeField] Slider staminaSlider;
    [SerializeField] GameObject ControlsMenu;
    [SerializeField] GameObject EnvanterSlotImage;
    [SerializeField] GameObject EnvanterSlotImage2;
    [SerializeField] GameObject EnvanterPos;


    bool isPaused = false;
    bool shopOpen = false;
    int layerMask;

    ShopController currentShop;

    
    void Start()
    {
        pausePanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        layerMask = ~LayerMask.GetMask("Player");
    }


    void Update()
    {
        if (shopOpen)
        {
            if (Input.GetKeyDown(KeyCode.X))
                CloseShop();
            return; 
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
            if (ControlsMenu.activeSelf == true)
            {
                ControlsMenu.SetActive(false);
            }
        }
            

        CrossHair();
        ShopInput();
        ItemInput();
        StaminaBar();
        InventoryVisible();
        SetEnvanterpos();        
    }


    // ================= ITEM (E) =================
    void ItemInput()
    {
        //ÖNCE BACKPACK TAKMAYI DENE
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (UI_itemController.instance.TryEquipSelectedBackpack())
                return; // takıldıysa pickup'a girme
        }

        // ================= NORMAL ITEM PICKUP =================
        if (Input.GetKeyDown(KeyCode.E))
        {

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 3f, layerMask))
            {
                WorldItem worldItem = hit.collider.GetComponent<WorldItem>();
                if (worldItem == null) return;

                bool added = UI_itemController.instance.TryAddItem(worldItem.data);

                if (added)
                {
                    Destroy(hit.collider.gameObject);
                }
                else
                {
                    Vector3 dropPos = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;                       
                    UI_itemController.instance.SwapWithSelected(worldItem.data, dropPos);
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }

    // ================= SHOP (Z / X) =================
    void ShopInput()
    {
        if (shopOpen) return; //zaten açıksa tekrar açma

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 3f, layerMask))
            {
                currentShop = hit.collider.GetComponentInParent<ShopController>();
                if (currentShop != null)
                    OpenShop(currentShop);
            }
        }
    }
    void OpenShop(ShopController shop)
    {
        shopOpen = true;
        currentShop = shop;

        GameManager.Instance.shopUIRoot.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
    }

    void CloseShop()
    {
        shopOpen = false;
        currentShop = null;

        GameManager.Instance.shopUIRoot.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }   

    // ================= PAUSE =================
    
    void TogglePause()
    {
        if (shopOpen) return; //shop açıkken pause olmaz

        isPaused = !isPaused;

        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;

        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;       
    }

    // ================= CROSSHAIR =================
    void CrossHair()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 2f))
        {
            if (hit.collider.GetComponent<WorldItem>())
                childObjectText.gameObject.SetActive(true);
            else
                childObjectText.gameObject.SetActive(false);

            if (hit.collider.GetComponentInParent<ShopController>())
                childObjectText2.gameObject.SetActive(true);
            else 
                childObjectText2.gameObject.SetActive(false);
            if (hit.collider.CompareTag("invisPlank"))
            {
                childObjectText3.gameObject.SetActive(true);
            }
            else
            {
                childObjectText3.gameObject.SetActive(false);
            }
        }
        else
        {
            childObjectText.gameObject.SetActive(false);
            childObjectText2.gameObject.SetActive(false);
            childObjectText3.gameObject.SetActive(false);
        }
    }
    void StaminaBar()
    {
        if (PlayerController.Instance == null) return;

        staminaSlider.value = PlayerController.Instance.stamina;
    }
    void InventoryVisible()
    {
        if (UI_itemController.instance.isBackpackEquipped == true)
        {
            EnvanterSlotImage.SetActive(true);
            EnvanterSlotImage2.SetActive(true);
        }
        if (UI_itemController.instance.isBackpackEquipped == false)
        {
            EnvanterSlotImage.SetActive(false);
            EnvanterSlotImage2.SetActive(false);
        }

    }
    void SetEnvanterpos()
    {
        var pos = EnvanterPos.transform.localPosition;
        pos.x = 0;       
        var Originalpos = EnvanterPos.transform.localPosition;
        Originalpos.x = -160;
        if (UI_itemController.instance.isBackpackEquipped == false)
        {
            EnvanterPos.transform.localPosition = pos;
        }
        if (UI_itemController.instance.isBackpackEquipped == true)
        {
            EnvanterPos.transform.localPosition = Originalpos;
        }
    }
    public void SpawnPlank()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 2f))
        {
            if (hit.collider != null && hit.collider.CompareTag("invisPlank"))
            {
                GameManager.Instance.plankspawnet.SetActive(true);
                GameManager.Instance.gorunmezplankdespawnet.SetActive(false);
            }
        }
    }

    public void GoBackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    public void OpenControls()
    {
        ControlsMenu.SetActive(true);
        
    }
    public void Resume()
    {
        pausePanel.SetActive(false);
        isPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }
    public void BackToPauseMenu()
    {
        ControlsMenu.SetActive(false);        
    }
    public void RestartGame()
    {
        GameManager.Instance.ResetGame();
    }    
}
