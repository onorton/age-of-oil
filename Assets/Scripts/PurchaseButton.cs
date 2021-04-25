using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PurchaseButton : MonoBehaviour
{
    public Purchasable _purchasable;
    private Text _purchaseButtonText;

    public AudioClip PurchaseSound;
    private AudioSource _audioSource;

    private Button _button;

    public string PrefixedTest;

    public bool CanAfford { get; set; }

    void Awake()
    {
        _purchaseButtonText = transform.Find("Text").GetComponent<Text>();
        FindObjectOfType<PlayerStatus>().moneyChanged.AddListener(UpdateButtonStatus);
        _purchasable.priceChanged.AddListener(SetText);
        _button = GetComponent<Button>();

        _audioSource = GameObject.FindObjectOfType<AudioSource>();
        if (PurchaseSound == null)
        {
            PurchaseSound = (AudioClip)Resources.Load("Sounds/Purchase");
        }
        SetText();
    }

    void UpdateButtonStatus(double amount)
    {
        CanAfford = amount >= _purchasable.Price;
        _button.interactable = CanAfford;
    }

    private void SetText()
    {
        var priceFormatted = string.Format("{0:N2}", _purchasable.Price);
        _purchaseButtonText.text = $"{PrefixedTest} (${priceFormatted})";
    }

    public void Purchase()
    {
        _purchasable.Purchase();
        _audioSource.PlayOneShot(PurchaseSound);
    }
}
