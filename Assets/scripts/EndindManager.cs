using UnityEngine;
using UnityEngine.SceneManagement;

public class EndindManager : MonoBehaviour
{
    public void EndGameAndgobacktomainmenu()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu");
    }
}
