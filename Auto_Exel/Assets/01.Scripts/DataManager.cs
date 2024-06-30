using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager : MonoBehaviour
{
    [TextArea] public string _URL;
    public int rowNumber;
    public int columnNumber;


    [ContextMenu("GET_REQ")]
    public void GetRequest()
    {
        StartCoroutine(GetCorotine(LoadComplete));
    }

    [ContextMenu("Post_REQ")]

    public void PostRequest()
    {
        StartCoroutine(PostCorutine(LoadComplete));
    }

    private IEnumerator PostCorutine(Action<string> onComplete)
    {
        WWWForm form = new WWWForm();
        form.AddField("row", rowNumber);
        form.AddField("col", columnNumber);
        UnityWebRequest www = UnityWebRequest.Post(_URL,form);
        yield return www.SendWebRequest();

        onComplete?.Invoke(www.downloadHandler.text);
    }

    private void LoadComplete(string response)
    {
        Debug.Log(response);
    }

    private IEnumerator GetCorotine(Action<string> OnComplete)
    {
        UnityWebRequest www = UnityWebRequest.Get(_URL);
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;

        OnComplete?.Invoke(data);
    }
}
