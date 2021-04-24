using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyPanel : MonoBehaviour
{
    private Text _text;
    // Start is called before the first frame update
    void Start()
    {
        _text = gameObject.transform.Find("MoneyText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateMoney(double amount)
    {
        _text.text = string.Format("${0:N2}", amount);
    }
}
