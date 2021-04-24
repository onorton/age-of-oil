using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PurchaseButton : MonoBehaviour
{
    public Purchasable _purchasable;
    private Text _purchaseButtonText;

    private Button _button;

    public string PrefixedTest;

    void Start()
    {
        _purchaseButtonText = transform.Find("Text").GetComponent<Text>();
        FindObjectOfType<PlayerStatus>().moneyChanged.AddListener(UpdateButtonStatus);
        _purchasable.priceChanged.AddListener(SetText);
        _button = GetComponent<Button>();
    }

    void UpdateButtonStatus(double amount)
    {
        _button.interactable = amount >= _purchasable.Price;
    }

    private void SetText()
    {
        _purchaseButtonText.text = $"{PrefixedTest} (${_purchasable.Price})";
    }

    public void Purchase()
    {
        _purchasable.Purchase();
    }
}
