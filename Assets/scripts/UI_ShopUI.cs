using UnityEngine;

public class UI_ShopUI : MonoBehaviour
{
    public static UI_ShopUI Instance;

    [Header("SHOP ROOT")]
    public GameObject shopUIRoot;

    bool isOpen = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        shopUIRoot.SetActive(false);
    }

    public void OpenShop()
    {
        if (isOpen) return;

        isOpen = true;
        shopUIRoot.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;

        
    }

    public void CloseShop()
    {
        if (!isOpen) return;

        isOpen = false;
        shopUIRoot.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;

        
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}

