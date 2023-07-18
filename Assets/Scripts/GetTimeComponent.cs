using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GetTimeComponent : MonoBehaviour
{
    [SerializeField] private UnityEvent _OnInit;

    private const string sberApi = "https://smartapp-code.sberdevices.ru/tools/api/now";
    private const string yaApi = "https://yandex.com/time/sync.json";
    private const float updateTime = 3600;

    private DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
    private NetworkService _network;

    private int _hour;
    private int _minute;
    private int _second;
    private int _millisecond;
    private float _updateTimer;

    public delegate void OnTimeChanged(int hour, int minute, int second, int millisecond);
    public OnTimeChanged OnChange;

    private void Awake()
    {
        _network = new NetworkService();    
    }

    private void Start()
    {
        GetTime();
        _updateTimer = updateTime;
    }

    private void Update()
    {
        _updateTimer -= Time.deltaTime;

        if (_updateTimer <= 0)
        {
            GetTime();
            _updateTimer = updateTime;
        }
    }

    private void GetTime()
    {
        StartCoroutine(GetRequest());
    }

    private void GetApiTime(string response, string key)
    {       
        try
        {
            var getResult = JObject.Parse(response);

            var timestamp = Convert.ToDouble(getResult.ToObject<Dictionary<string, object>>()[key]);

            var time = dt.AddMilliseconds(timestamp).ToLocalTime();

            _hour = time.Hour;
            _minute = time.Minute;
            _second = time.Second;
            _millisecond = time.Millisecond;
        }
        catch (Exception)
        {
            throw new Exception("Parse error");
        }
    }

    private IEnumerator GetRequest()
    {
        yield return StartCoroutine(_network.GetJson(sberApi, GetApiTime, "timestamp"));
        yield return StartCoroutine(_network.GetJson(yaApi, GetApiTime, "time"));

        OnChange?.Invoke(_hour, _minute, _second, _millisecond);
        
        _OnInit?.Invoke();
        if (_OnInit != null) _OnInit = null;
    }
}
