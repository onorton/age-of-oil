using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour
{
    // Start is called before the first frame update
    private double money = 80000;
    private double moneyPerBarrel = 10.0;

    public bool CanSeeBlowoutChance { get; set; }
    public double OverallBlowoutChanceFactor { get; set; } = 1.0;
    public double ProspectingAccuracy { get; set; } = 0.5;

    public UnityEvent<double> moneyChanged;
    public UnityEvent upgradeApplied;


    private GameObject _globalUpgradesItems;
    private GameObject _upgradePrefab;
    private Dictionary<string, Upgrade> _upgrades;

    void Awake()
    {
        _globalUpgradesItems = GameObject.Find("MainUI/RightRegion/UpgradesPanel/Items");

        _upgradePrefab = (GameObject)Resources.Load("Prefabs/Upgrade");
        var upgradeData = JsonUtility.FromJson<UpgradesData>(File.ReadAllText($"{Application.streamingAssetsPath}/playerUpgrades.json"));
        CreateUpgrades(upgradeData.Upgrades);
    }

    private void CreateUpgrades(UpgradeData[] upgrades)
    {
        _upgrades = new Dictionary<string, Upgrade>();

        foreach (var upgrade in upgrades)
        {
            var upgradeUi = Instantiate(_upgradePrefab, _globalUpgradesItems.transform);
            var upgradeComponent = upgradeUi.GetComponent<Upgrade>();
            upgradeComponent.Effects = upgrade.Effects;
            upgradeComponent.SetPrice(upgrade.Price);
            upgradeComponent.DestroyOnNextConsumption = true;
            upgradeComponent.activated.AddListener(() => ApplyUpgrade(upgradeComponent));
            upgradeComponent.Name = upgrade.Name;
            upgradeComponent.Predecessors = upgrade.Predecessors;
            upgradeUi.transform.Find("Name").GetComponent<Text>().text = upgradeComponent.Name;
            upgradeUi.transform.Find("Description").GetComponent<Text>().text = upgrade.Description;
            _upgrades[upgradeComponent.Name] = upgradeComponent;
        }
    }

    private void ApplyUpgrade(Upgrade upgrade)
    {
        foreach (var effect in upgrade.Effects)
        {
            switch (effect.EffectType)
            {
                case EffectType.EnableBlowoutChanceVisibility:
                    CanSeeBlowoutChance = true;
                    break;
                case EffectType.ChangeBlowoutChance:
                    OverallBlowoutChanceFactor -= effect.Amount;
                    break;
                case EffectType.ChangeProspectingAccuracy:
                    ProspectingAccuracy = Math.Min(effect.Amount * ProspectingAccuracy, 1.0);
                    break;
                default:
                    throw new Exception("Not a valid enum value");

            }
        }
        _upgrades.Remove(upgrade.Name);
        upgradeApplied.Invoke();

        if (_upgrades.Count == 0)
        {
            var upgradesPanel = _globalUpgradesItems.transform.parent.gameObject;
            if (upgradesPanel != null)
            {
                Destroy(upgradesPanel.gameObject);
            }
        }
    }

    void Start()
    {
        moneyChanged.Invoke(money);
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
