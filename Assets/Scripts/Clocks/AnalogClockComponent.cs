using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalogClockComponent : MonoBehaviour
{
    [SerializeField] private GetTimeComponent _timeComponent;
    [Space]
    [SerializeField] private RectTransform _hourHand;
    [SerializeField] private RectTransform _minuteHand;
    [SerializeField] private RectTransform _secondHand;

    private float _currentHours;
    private float _currentMinutes;
    private float _currentSeconds;

    private float _hourDegree = 360 / 12f;
    private float _minuteDegree = 360 / 60f;
    private float _secondDegree = 360 / 60f;

    private bool _isActive;
    private bool _isFreeze;

    private DragComponent _hourDragComponent;
    private DragComponent _minuteDragComponent;

    private void Start()
    {
        _timeComponent.OnChange += SetupApiTime;
    }
    private void Update()
    {
        if (_isActive) UpdateTime();
    }

    private void SetupApiTime(int hour, int minute, int second, int millisecond)
    {
        SetTime(hour, minute, second);

        _currentHours = hour;
        _currentMinutes = minute;
        _currentSeconds = second + (float)millisecond / 1000;

        _isActive = true;
    }

    private void SetTime(float hour, float minute, float second)
    {
        _hourHand.transform.eulerAngles = new Vector3(0, 0, -(Mathf.Repeat(hour, 12) + minute / 60) * _hourDegree);
        _minuteHand.transform.eulerAngles = new Vector3(0, 0, -(minute + second / 60 ) * _minuteDegree);
        _secondHand.transform.eulerAngles = new Vector3(0, 0, -second * _secondDegree);
    }

    private void UpdateTime()
    {
        _currentSeconds += Time.deltaTime;

        if (Math.Truncate(_currentSeconds) >= 60)
        {
            _currentMinutes++;
            _currentSeconds -= 60;
        }
        if (_currentMinutes >= 60)
        {
            _currentHours = (int)Mathf.Repeat(++_currentHours, 12);
            _currentMinutes = 0;
        }

        if (_isFreeze) return;

        SetTime(_currentHours, _currentMinutes, _currentSeconds);
    }

    public void SetAlarmMode(bool active)
    {
        _isFreeze = active;
        _secondHand.gameObject.SetActive(!active);

        if (_hourDragComponent == null) _hourDragComponent = _hourHand.GetComponent<DragComponent>();
        if (_minuteDragComponent == null) _minuteDragComponent = _minuteHand.GetComponent<DragComponent>();

        _hourDragComponent.Activate(active);
        _minuteDragComponent.Activate(active);
    }

    private void OnDestroy()
    {
        _timeComponent.OnChange -= SetupApiTime;
    }
}
