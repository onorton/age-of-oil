using System;
using System.Collections.Generic;
using System.Linq;

public class Upgrade : Purchasable
{
    public string Name { get; set; }
    public Effect[] Effects { get; set; } = new Effect[] { };
    public string[] Predecessors = new string[] { };

}

[Serializable]
public class UpgradeData
{
    public string Name;
    public string Description;
    public int Price;
    public Effect[] Effects;
    public string[] Predecessors;

}

[Serializable]
public class UpgradesData
{
    public UpgradeData[] Upgrades;
}