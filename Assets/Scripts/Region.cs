using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Region : MonoBehaviour
{
    private OilLevels _levels = new OilLevels();
    private PlayerStatus _playerStatus;
    private GameObject _statusUi;
    private GameObject _statusPanel;
    private GameObject _oilWellPrefab;

    public Purchasable _purchasableOilWell;

    private Material _highlighted;
    private Material _normal;
    private SpriteRenderer _renderer;

    public string _regionName = "Westlands";

    private int oilWellCost = 20000;

    // Start is called before the first frame update
    void Start()
    {
        _playerStatus = GameObject.FindObjectOfType<PlayerStatus>();
        _statusUi = transform.Find("StatusUI").gameObject;
        _statusUi.SetActive(false);
        _statusPanel = _statusUi.transform.Find("StatusPanel").gameObject;
        _oilWellPrefab = (GameObject)Resources.Load("Prefabs/Oil Well");
        var name = _statusPanel.transform.Find("Name").GetComponent<Text>();
        name.text = _regionName;
        _purchasableOilWell = transform.Find("PurchasableOilWell").GetComponent<Purchasable>();
        _purchasableOilWell.Action = new Action(() => PurchaseWell());
        _purchasableOilWell.SetPrice(oilWellCost);
        _normal = (Material)Resources.Load("Materials/Normal");
        _highlighted = (Material)Resources.Load("Materials/Highlighted");
        _renderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
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

    public void PurchaseWell()
    {
        _statusUi.SetActive(false);
        oilWellCost *= 2;
        _purchasableOilWell.SetPrice(oilWellCost);
        var oilWell = Instantiate<GameObject>(_oilWellPrefab, transform.Find("Container"));
        oilWell.GetComponent<OilWell>().triedToFindOil.AddListener(FindingOil);
    }

    private void FindingOil(double amount, int maximumDepth)
    {
        var barrelsFound = Math.Min(amount, _levels.BarrelsUpToGivenDepth(maximumDepth));
        _levels.RemoveBarrelsUpToGivenDepth(barrelsFound, maximumDepth);
        _playerStatus.SellOil(barrelsFound);
    }

    // Highlight green using 
    void OnMouseOver()
    {
        if (!_statusUi.activeSelf)
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
