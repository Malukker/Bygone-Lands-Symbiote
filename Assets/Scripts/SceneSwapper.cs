using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapper : MonoBehaviour
{
    enum Scenes 
    { 
        Level1,
        MainMenu
    }

    [SerializeField] private Scenes _scenes;

    private void OnEnable()
    {
        switch (_scenes)
        {
            case Scenes.Level1:
                SceneManager.LoadScene("Level 1");
                break;

            case Scenes.MainMenu:
                SceneManager.LoadScene("MainMenu");
                break;
        }
        
    }
}
