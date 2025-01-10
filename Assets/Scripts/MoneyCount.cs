using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyCount : MonoBehaviour
{
    [HideInInspector]public int money = 0;

    public TextMeshProUGUI counter;

    public void MoneyUp(int n)
    {
        money += n;
        UpdateMoney();
    }

    public void MoneyDown(int n)
    {
        money -= n;
        UpdateMoney();
    }

    void UpdateMoney()
    {
        counter.text = money.ToString();
    }
}
