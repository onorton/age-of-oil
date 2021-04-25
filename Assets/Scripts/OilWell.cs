using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;

public class OilWell : MonoBehaviour
{
    // Barrels in game time days
    private double _barrelsPerDay = 50.0;
    private PlayerStatus _playerStatus;
    private GameObject _statusUi;
    private GameObject _statusPanel;

    // Depths in feet (just for visual purposes)
    private int _currentDepth = 100;
    private int _maxDepth = 300;


    // Chance per second
    private double _blowoutChance = 0.01;
    // Per 100 feet
    private double _blowoutChanceFactor = 1.2;
    public Sprite BlowoutSprite;
    private AudioSource _audioSource;
    private AudioClip _blowoutSound;

    private double _timePassed = 0;

    public UnityEvent<double, int> triedToFindOil;

    private GameObject _upgradePrefab;

    private IDictionary<string, Upgrade> _upgrades;


    void Awake()
    {
        _statusUi = transform.Find("StatusUI").gameObject;
        _statusPanel = _statusUi.transform.Find("StatusPanel").gameObject;

        _audioSource = GameObject.FindObjectOfType<AudioSource>();
        _blowoutSound = (AudioClip)Resources.Load("Sounds/Blowout");

        _upgradePrefab = (GameObject)Resources.Load("Prefabs/Upgrade");
        var upgradeData = JsonUtility.FromJson<UpgradesData>(File.ReadAllText($"{Application.streamingAssetsPath}/wellUpgrades.json"));
        CreateUpgrades(upgradeData.Upgrades);
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerStatus = GameObject.FindObjectOfType<PlayerStatus>();
        _playerStatus.upgradeApplied.AddListener(UpdateStats);

        _statusUi.SetActive(false);
        UpdateStats();

        var depthIncreaseUpgrade = transform.Find("DepthIncreaser").GetComponent<Upgrade>();
        depthIncreaseUpgrade.activated.AddListener(() => ApplyUpgrade(depthIncreaseUpgrade));
        depthIncreaseUpgrade.SetPrice(300);
        depthIncreaseUpgrade.Name = "Increase Depth";
        depthIncreaseUpgrade.Effects = new Effect[] { new Effect { EffectType = EffectType.IncreaseCurrentDepth, Amount = 100 }, new Effect { EffectType = EffectType.ChangeBlowoutChance, Amount = _blowoutChanceFactor } };
    }

    private void CreateUpgrades(UpgradeData[] upgrades)
    {
        var itemsTransform = _statusPanel.transform.Find("UpgradesPanel/Items");

        _upgrades = new Dictionary<string, Upgrade>();

        foreach (var upgrade in upgrades)
        {
            var upgradeUi = Instantiate(_upgradePrefab, itemsTransform);
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

    // Update is called once per frame
    void Update()
    {
        var newTime = _timePassed + Time.deltaTime;
        if (Math.Truncate(newTime * TimeSystem.TimeScale / (24 * 3600)) > Math.Truncate(_timePassed * TimeSystem.TimeScale / (24 * 3600)))
        {
            var r = new System.Random();
            _timePassed = newTime;
            if (r.NextDouble() < BlowoutChance())
            {
                Blowout();
            }
        }
        _timePassed = newTime;

        triedToFindOil.Invoke(_barrelsPerDay * Time.deltaTime * TimeSystem.TimeScale / (24 * 3600), _currentDepth);
    }

    void UpdateStats()
    {
        var formattedExtractionRate = string.Format("{0:0.###}", _barrelsPerDay);
        _statusPanel.transform.Find("Information/BarrelsPerSecond").GetComponent<Text>().text = $"Extraction Rate (barrels/day): {formattedExtractionRate}";

        var formattedBlowoutChance = string.Format("{0:0.###}%", BlowoutChance() * 100);
        if (_playerStatus.CanSeeBlowoutChance)
        {
            _statusPanel.transform.Find("Information/BlowoutChance").GetComponent<Text>().text = $"Blowout Chance (per day): {formattedBlowoutChance}";
        }
        else
        {
            _statusPanel.transform.Find("Information/BlowoutChance").GetComponent<Text>().text = $"";
        }

        _statusPanel.transform.Find("Information/CurrentDepth").GetComponent<Text>().text = $"Current Depth: {_currentDepth}ft";
        _statusPanel.transform.Find("Information/MaximumDepth").GetComponent<Text>().text = $"Maximum Depth: {_maxDepth}ft";
        _statusPanel.transform.Find("Information/IncreaseDepthButton").GetComponent<LimitedButton>().Value = _currentDepth;
        _statusPanel.transform.Find("Information/IncreaseDepthButton").GetComponent<LimitedButton>().Limit = _maxDepth;
    }

    void Blowout()
    {
        var renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = BlowoutSprite;
        if (!_audioSource.isPlaying)
        {
            _audioSource.PlayOneShot(_blowoutSound);
        }
        Destroy(this);
    }


    private double BlowoutChance()
    {
        return _blowoutChance * _playerStatus.OverallBlowoutChanceFactor;
    }

    void ApplyUpgrade(Upgrade upgrade)
    {
        foreach (var effect in upgrade.Effects)
        {
            switch (effect.EffectType)
            {
                case EffectType.IncreaseCurrentDepth:
                    _currentDepth += (int)effect.Amount;
                    break;
                case EffectType.IncreaseMaximumDepth:
                    _maxDepth += (int)effect.Amount;
                    break;
                case EffectType.IncreasePumpRate:
                    _barrelsPerDay *= effect.Amount;
                    break;
                case EffectType.ChangeBlowoutChance:
                    _blowoutChance *= effect.Amount;
                    break;
                default:
                    throw new Exception("Not a valid enum value");

            }
        }
        _upgrades.Remove(upgrade.Name);
        // Apply any predecessors and destroy them;
        foreach (var p in upgrade.Predecessors)
        {
            if (_upgrades.TryGetValue(p, out var predecessor))
            {
                predecessor.Consume();
            }
        }
        UpdateStats();
        if (_upgrades.Count == 0)
        {
            var upgradesPanel = _statusPanel.transform.Find("UpgradesPanel");
            if (upgradesPanel != null)
            {
                Destroy(upgradesPanel.gameObject);
                var panelSizeDelta = _statusPanel.GetComponent<RectTransform>().sizeDelta;
                _statusPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(panelSizeDelta.x, panelSizeDelta.y / 2);
            }
        }
    }

    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0)
            && !EventSystem.current.IsPointerOverGameObject()
            && isActiveAndEnabled
            && FindObjectOfType<Placer>() == null
            && GameObject.FindGameObjectsWithTag("SingleModal").All(x => !x.activeSelf))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                _statusUi.SetActive(true);
                Vector3 spotAboveWell = transform.position + new Vector3(0.0f, 5.0f, 0.0f);
                Vector2 wellScreenPosition = Camera.main.WorldToScreenPoint(transform.position);

                Vector2 canvasPosition;
                var rectTransform = _statusPanel.GetComponent<RectTransform>();

                // Adjust so it is definitely within screen bounds
                var statusPanelWidth = rectTransform.sizeDelta.x;
                var statusPanelHeight = rectTransform.sizeDelta.y;
                var screenPositionX = wellScreenPosition.x;
                var screenPositionY = wellScreenPosition.y;
                screenPositionY = Math.Min(screenPositionY + statusPanelHeight / 2, Camera.main.scaledPixelHeight) - statusPanelHeight / 2;
                screenPositionY = Math.Max(screenPositionY - statusPanelHeight / 2, 0) + statusPanelHeight / 2;
                screenPositionX = Math.Min(screenPositionX + statusPanelWidth / 2, Camera.main.scaledPixelWidth) - statusPanelHeight / 2;
                screenPositionX = Math.Max(screenPositionX - statusPanelWidth / 2, 0) + statusPanelHeight / 2;
                wellScreenPosition = new Vector2(screenPositionX, screenPositionY);

                rectTransform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, wellScreenPosition, null, out canvasPosition);
                rectTransform.localPosition = canvasPosition;
            }
        }
    }
}
