using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DragComponent : MonoBehaviour
{
    [SerializeField] private RectTransform _transform;
    [SerializeField] private int _minAngle;
    [SerializeField] private TMP_InputField _text;
    [SerializeField] private DragType _type;

    private int _currentValue;

    private bool _pm;
    private bool _isActive;
    private bool _isPrepare;

    private DigitalClockComponent _clock;

    private void Start()
    {
        _clock = FindObjectOfType<DigitalClockComponent>();
    }
    public void OnMouseDrag(BaseEventData data)
    {
        if (!_isActive) return;

        PointerEventData pointerData = data as PointerEventData;

        Vector3 relative = Camera.main.ScreenToWorldPoint(pointerData.position);

        float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;

        if (angle < 0) angle = 360 + angle;
        
        angle = (float)Math.Truncate( angle / _minAngle) * _minAngle;
        transform.eulerAngles = new Vector3(0, 0, -angle);

        var time = (int)Math.Truncate(angle / _minAngle);

        if (_type == DragType.Hour)
        {
            var currentHour = int.Parse(_clock.Hours.text);

            if (!_isPrepare)
            {
                _currentValue = currentHour;
                _pm = (currentHour <= 12 && !_isPrepare) ? false : true;
                _isPrepare = true;
            }
            
            if (time == 0 && !_pm && _currentValue == 11) _pm = !_pm;
            if (time == 11 && _pm && _currentValue == 12) _pm = !_pm;
            if (time == 0 && _pm && _currentValue == 23) _pm = !_pm;
            if (time == 11 && !_pm && _currentValue == 0) _pm = !_pm;
            
            time = _pm ? time += 12 : time;
            _currentValue = time;
        }

        _text.text = time.ToString("D2");
    }

    public void Activate(bool active)
    {
        _isActive = active;
    }
}

public enum DragType
{ 
    Minute,
    Hour
}
