using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject gameHud;

    public void Pause()
    {
        pauseMenu.SetActive(true);
        gameHud.SetActive(false);
        Time.timeScale = 0f;
    }

    public void Continue()
    {
        pauseMenu.SetActive(false);
        gameHud.SetActive(true);
        Time.timeScale = 1f;
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
