using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LivesCount : MonoBehaviour
{
    private int lives = 4;
    public TextMeshProUGUI counter;

    public void LivesDown(int n)
    {
        lives -= n;
        UpdateLives();
    }
    void UpdateLives()
    {
        counter.text = lives.ToString();
    }
}
