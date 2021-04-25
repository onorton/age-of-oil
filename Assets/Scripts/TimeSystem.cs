using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSystem : MonoBehaviour
{
    // In-game seconds corresponding to real time seconds
    public const int TimeScale = 7200;
    private bool _paused = true;
    private Text _dateText;
    // In-game time
    private DateTime _currentTime;
    void Start()
    {
        _currentTime = new DateTime(1855, 6, 1);
        _dateText = transform.Find("DateText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_paused)
        {
            _currentTime = _currentTime.AddSeconds(TimeScale * Time.deltaTime);
        }
        _dateText.text = _currentTime.ToString("ddd, d MMMM yyyy");

    }

    public void Resume()
    {
        _paused = false;
        Time.timeScale = 1.0f;
    }

    public void Pause()
    {
        _paused = true;
        Time.timeScale = 0.0f;
    }
}
