using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkService
{
    private IEnumerator CallAPI(string url, Action<string, string> callback, string key)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("Network problem: " + request.error);
            }
            else if (request.responseCode != (long)System.Net.HttpStatusCode.OK)
            {
                Debug.LogError("Responce error: " + request.responseCode);
            }
            else
            {
                callback(request.downloadHandler.text, key);
            }
        }
    }

    public IEnumerator GetJson(string url, Action<string, string> callback, string key)
    {
        return CallAPI(url, callback, key);     
    }
}
