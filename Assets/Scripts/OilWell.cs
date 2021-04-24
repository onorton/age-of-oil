using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OilWell : MonoBehaviour
{
    // Barrels in realtime seconds
    private int _oilPerSecond = 10;
    private PlayerStatus _playerStatus;
    private GameObject _statusUi;
    private GameObject _statusPanel;
    private int currentDepth = 200;
    private int maxDepth = 200;

    public UnityEvent<double, int> triedToFindOil;

    // Start is called before the first frame update
    void Start()
    {
        _playerStatus = GameObject.FindObjectOfType<PlayerStatus>();
        _statusUi = transform.Find("StatusUI").gameObject;
        _statusPanel = _statusUi.transform.Find("StatusPanel").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        triedToFindOil.Invoke(_oilPerSecond * Time.deltaTime, currentDepth);
    }

    void OnMouseEnter()
    {
        _statusUi.SetActive(true);
        Vector3 spotAboveWell = transform.position + new Vector3(0.0f, 5.0f, 0.0f);
        Vector2 wellScreenPosition = Camera.main.WorldToScreenPoint(transform.position);

        Vector2 canvasPosition;
        var rectTransform = _statusPanel.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, wellScreenPosition, null, out canvasPosition);
        rectTransform.localPosition = canvasPosition;
    }

    void OnMouseExit()
    {
        _statusUi.SetActive(false);
    }
}
