using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Menus")]
    public GameObject ControlsMenu;
    public GameObject AyarlarMenu;
    [SerializeField] GameObject TutorialMenu;

    [Header("Settings")]
    public Slider sensitivitySlider;

    private void Start()
    {
        // Kaydedilmiþ sensitivity varsa yükle
        float savedSensitivity = PlayerPrefs.GetFloat("sensitivity", 400f);
        sensitivitySlider.value = savedSensitivity;
    }

    public void SaveSensitivity()
    {
        float value = sensitivitySlider.value;
        PlayerPrefs.SetFloat("sensitivity", value);
        PlayerPrefs.Save();

       
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");     
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Controls()
    {
        ControlsMenu.SetActive(true);
    }

    public void Ayarlar()
    {
        AyarlarMenu.SetActive(true);
    }
    public void Tutorial()
    {
        TutorialMenu.SetActive(true);
    }

    public void BackToMenu()
    {
        ControlsMenu.SetActive(false);
        AyarlarMenu.SetActive(false);
        TutorialMenu.SetActive(false);
    }

    public void DifficultySelectButton(float level)
    {
        PlayerPrefs.SetInt("difficulty", (int)level);
        PlayerPrefs.Save();

       
    }

}
