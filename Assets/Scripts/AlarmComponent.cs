using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class AlarmComponent : MonoBehaviour
{
    [SerializeField] private GameObject _alarmInput;
    [SerializeField] private GameObject _activeIcon;
    [SerializeField] private GameObject _submitBtn;
    [SerializeField] private GameObject _dismissBtn;
    [Space]
    [SerializeField] private TMP_InputField _hour;
    [SerializeField] private TMP_InputField _minute;
    [Space]
    [SerializeField] private DigitalClockComponent _digitalClock;
    [SerializeField] private AnalogClockComponent _analogClock;

    public void OnAlarmClick()
    {
        _alarmInput.SetActive(!_alarmInput.activeSelf);
        _analogClock.SetAlarmMode(_alarmInput.activeSelf);
    }

    public void OnSubmit()
    {
        if (string.IsNullOrEmpty(_hour.text) || string.IsNullOrEmpty(_minute.text)) return;

        var hour = int.Parse(_hour.text);
        var minute = int.Parse(_minute.text);

        if (hour < 0 || hour > 23 || minute < 0 || minute > 59) return;

        OnAlarmSubmit(hour, minute);
    }

    public void OnDismiss()
    {
        _digitalClock.DisableAlarm();

        _activeIcon.SetActive(false);

        _hour.interactable = true;
        _minute.interactable = true;
        _hour.text = "";
        _minute.text = "";

        _submitBtn.SetActive(true);
        _dismissBtn.SetActive(false);
    }

    private void OnAlarmSubmit(int hour, int minute)
    {
        _digitalClock.SetAlarm(hour, minute);
        _activeIcon.SetActive(true);

        _hour.interactable = false;
        _minute.interactable = false;

        _submitBtn.SetActive(false);
        _dismissBtn.SetActive(true);

        OnAlarmClick();
    }
}
