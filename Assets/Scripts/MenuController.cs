using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public static MenuController instance;
    
    // Outlets
    public GameObject pauseMenu;
    public GameObject gameOverScreen;
    public GameObject UI;
    
    // Methods
    void Awake()
    {
        instance = this;
        Hide();
    }

    public void ShowPauseMenu()
    {
        SwitchMenu(pauseMenu);
        gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
        if (PlayerController.instance != null)
        {
            PlayerController.instance.isPaused = false;
        }
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("platform test");
    }

    void SwitchMenu(GameObject someMenu)
    {
        pauseMenu.SetActive(false);
        gameOverScreen.SetActive(false);

        someMenu.SetActive(true);
    }

    public void ShowGameOver()
    {
        SwitchMenu(gameOverScreen);
        UI.SetActive(false);
        Time.timeScale = 0;
        gameObject.SetActive(true);
    }
}
