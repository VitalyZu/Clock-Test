using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DigitalClockComponent : MonoBehaviour
{
    [SerializeField] private GetTimeComponent _timeComponent;
    [SerializeField] private AlarmComponent _alarmComponent;
    [Space]
    [SerializeField] private Text _hours;
    [SerializeField] private Text _minutes;
    [SerializeField] private Text _seconds;

    private AudioSource _audio;

    private float _currentSeconds;

    private bool _isActive;

    private bool _isAlarm;
    private int _alarmHour;
    private int _alarmMinute;

    public Text Hours => _hours;

    private void Start()
    {
        _timeComponent.OnChange += SetupApiTime;
        _audio = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if(_isActive) UpdateTime();
    }

    private void SetupApiTime(int hour, int minute, int second, int millisecond)
    {
        SetTime(hour, minute, second);
        _currentSeconds = second + (float)millisecond / 1000;

        _isActive = true;
    }

    private void SetTime(int hour, int minute, int second)
    {
        if (_isAlarm)
        {
            if (hour == _alarmHour & minute == _alarmMinute && second < 1)
            {
                _alarmComponent.OnDismiss();
                DisableAlarm();
                _audio.Play();
            }
        }

        _hours.text = hour.ToString("D2");
        _minutes.text = minute.ToString("D2");
        _seconds.text = second.ToString("D2");
    }

    private void UpdateTime()
    {
        var minutes = int.Parse(_minutes.text);
        var hours = int.Parse(_hours.text);

        _currentSeconds += Time.deltaTime;

        if (Math.Truncate(_currentSeconds) >= 60)
        {
            minutes++;
            _currentSeconds -= 60;
        }
        if (minutes >= 60)
        {
            hours = (int)Mathf.Repeat(++hours, 23);
            minutes = 0;
        }

        SetTime( hours, minutes, (int)Math.Truncate(_currentSeconds));
    }

    public void SetAlarm(int hour, int minute)
    {
        _isAlarm = true;
        _alarmHour = hour;
        _alarmMinute = minute;
    }
    public void DisableAlarm()
    {
        _isAlarm = false;
    }

    private void OnDestroy()
    {
        _timeComponent.OnChange -= SetupApiTime;
    }
}
