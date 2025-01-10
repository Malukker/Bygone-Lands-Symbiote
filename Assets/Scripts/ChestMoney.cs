using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestMoney : MonoBehaviour
{
    private SpriteRenderer skin;
    public GameObject hud;
    public int reward;
    private bool claimed;
    public GameObject effect;

    void Start()
    {
        skin = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!claimed && collision.CompareTag("Player"))
        {
            skin.color = Color.grey;
            hud.SendMessage("MoneyUp", reward);
            Instantiate(effect, transform.position, transform.rotation);
            claimed = true;
        }
    }
}
