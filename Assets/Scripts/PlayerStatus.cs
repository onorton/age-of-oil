using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStatus : MonoBehaviour
{
    // Start is called before the first frame update
    private double money = 20000;
    private double moneyPerBarrel = 0.1;

    public UnityEvent<double> moneyChanged;

    void Start()
    {
        moneyChanged.Invoke(money);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SellOil(double barrels)
    {
        money += barrels * moneyPerBarrel;
        moneyChanged.Invoke(money);
    }

    public void ChangeAmount(double amount)
    {
        money += amount;
        moneyChanged.Invoke(money);
    }


}
