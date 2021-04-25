using System;
using UnityEngine;
using UnityEngine.Events;

public class Purchasable : MonoBehaviour
{
    public int Price { get; set; }
    public bool DestroyOnNextConsumption { get; set; }

    public UnityEvent priceChanged;
    public UnityEvent activated;
    public UnityEvent destroyed;

    public void SetPrice(int price)
    {
        Price = price;
        priceChanged.Invoke();
    }

    private PlayerStatus _playerStatus;

    void Start()
    {
        _playerStatus = FindObjectOfType<PlayerStatus>();
    }

    public void Purchase()
    {
        _playerStatus.ChangeAmount(-Price);
        Consume();
    }

    public void Consume()
    {
        activated.Invoke();
        if (DestroyOnNextConsumption)
        {
            destroyed.Invoke();
        }
    }
}
