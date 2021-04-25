using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyPanel : MonoBehaviour
{
    private Text _moneyText;
    private Text _moneyPerBarrel;
    private PlayerStatus _playerStatus;

    // Start is called before the first frame update
    void Start()
    {
        _moneyText = gameObject.transform.Find("MoneyText").GetComponent<Text>();
        _moneyPerBarrel = gameObject.transform.Find("MoneyPerBarrel").GetComponent<Text>();
        _playerStatus = GameObject.FindObjectOfType<PlayerStatus>();
        _moneyPerBarrel.text = string.Format("${0:0.##} per barrel", _playerStatus.MoneyPerBarrel);
    }

    public void UpdateMoney(double amount)
    {
        _moneyText.text = string.Format("${0:N2}", amount);
    }

    public void UpdatePricePerBarrel()
    {
        _moneyPerBarrel.text = string.Format("${0:0.##} per barrel", _playerStatus.MoneyPerBarrel);
    }
}
