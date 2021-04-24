using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Purchasable : MonoBehaviour
{
    public int Price { get; private set; }

    public UnityEvent priceChanged;

    public void SetPrice(int price)
    {
        Price = price;
        priceChanged.Invoke();
    }

    public Action Action { get; set; }

    private PlayerStatus _playerStatus;

    void Start()
    {
        _playerStatus = GameObject.FindObjectOfType<PlayerStatus>();
    }

    public void Purchase()
    {
        _playerStatus.ChangeAmount(-Price);
        Action();
    }
}
