using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LimitedButton : MonoBehaviour
{
    public double Value { get; set; }
    public double Limit { get; set; }

    private Button _button;
    void Start()
    {
        _button = GetComponent<Button>();

    }

    // Update is called once per frame
    void Update()
    {
        _button.interactable = Value < Limit && (GetComponent<PurchaseButton>()?.CanAfford ?? true);
    }
}
