using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public void StartGame()
    {
        SceneManager.LoadScene("Intro Cutscene");
    }

    public void LaunchControlsPage()
    {
        SceneManager.LoadScene("Controls");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
