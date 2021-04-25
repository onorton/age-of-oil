using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class Region : MonoBehaviour
{
    private OilLevels _levels = new OilLevels();
    private PlayerStatus _playerStatus;
    private GameObject _statusUi;
    private GameObject _statusPanel;
    private GameObject _oilWellPrefab;

    public Purchasable _purchasableOilWell;

    public Purchasable _prospect;
    private Material _highlighted;
    private Material _normal;
    private SpriteRenderer _renderer;

    public string _regionName = "Westlands";

    private int oilWellCost = 20000;
    private int prospectingCost = 2000;

    // Start is called before the first frame update
    void Start()
    {
        _playerStatus = GameObject.FindObjectOfType<PlayerStatus>();
        _statusUi = transform.Find("StatusUI").gameObject;
        _statusUi.SetActive(false);
        _statusPanel = _statusUi.transform.Find("StatusPanel").gameObject;
        _oilWellPrefab = (GameObject)Resources.Load("Prefabs/Oil Well");
        var name = _statusPanel.transform.Find("Header/Name").GetComponent<Text>();
        name.text = _regionName;

        _purchasableOilWell = transform.Find("PurchasableOilWell").GetComponent<Purchasable>();
        _purchasableOilWell.activated.AddListener(PurchaseWell);
        _purchasableOilWell.SetPrice(oilWellCost);

        _prospect = transform.Find("Prospect").GetComponent<Purchasable>();
        _prospect.activated.AddListener(Prospect);
        _prospect.SetPrice(prospectingCost);

        _normal = (Material)Resources.Load("Materials/Normal");
        _highlighted = (Material)Resources.Load("Materials/Highlighted");
        _renderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
    }

    public void PurchaseWell()
    {
        _statusUi.SetActive(false);
        oilWellCost *= 2;
        _purchasableOilWell.SetPrice(oilWellCost);
        var oilWell = Instantiate<GameObject>(_oilWellPrefab, transform.Find("Container"));
        oilWell.GetComponent<OilWell>().triedToFindOil.AddListener(FindingOil);
        // Disable until placed;
        var placer = oilWell.AddComponent<Placer>();
        placer.DisabledComponentsUntilPlaced = new List<Behaviour> { oilWell.GetComponent<OilWell>() };
        var bounds = transform.GetComponent<BoxCollider2D>().bounds;

        bounds.extents = new Vector3(bounds.extents.x - 0.5f, bounds.extents.y - 0.5f, bounds.extents.z);
        placer.Bounds = bounds;
    }

    public void Prospect()
    {
        _statusPanel.transform.Find("ProspectingResults").GetComponent<Text>().text = _levels.Prospect(_playerStatus.ProspectingAccuracy);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_statusPanel.GetComponent<RectTransform>());
    }

    private void FindingOil(double amount, int maximumDepth)
    {
        var barrelsFound = Math.Min(amount, _levels.BarrelsUpToGivenDepth(maximumDepth));
        _levels.RemoveBarrelsUpToGivenDepth(barrelsFound, maximumDepth);
        _playerStatus.SellOil(barrelsFound);
    }

    void OnMouseDown()
    {
        // Only activate if region in front
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
        {
            Vector2 wellScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 canvasPosition;
            var rectTransform = _statusPanel.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, wellScreenPosition, null, out canvasPosition);
            rectTransform.localPosition = canvasPosition;
            _statusUi.SetActive(true);
        }
    }

    void OnMouseOver()
    {
        if (!_statusUi.activeSelf && !EventSystem.current.IsPointerOverGameObject())
        {
            _renderer.material = _highlighted;
        }
        else
        {
            _renderer.material = _normal;
        }
    }

    void OnMouseExit()
    {
        _renderer.material = _normal;
    }
}
