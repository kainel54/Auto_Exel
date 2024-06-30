using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class SimpleGoogleDownloader : MonoBehaviour
{
    /*
     * GET,POST
     * 
     * �Ѵ� ������ ���۰� ������ ����
     * GET�� ������ ������ Query��Ʈ���� �Ǿ ����
     * POST�� ������ ������ Body���ٰ� �Ǿ ����
     * 
     * FORMDATA
     * 
     * URLENcode
     * 
     * REST ����
     * 
     * URI ��ġ
     * ���: METHOD
     * 
     * GET ,POST,PUT,DELETE
     * 
     */


    [SerializeField]
    [TextArea]
    private string _URL;

    [ContextMenu("load_URL")]
    private void Load()
    {
        StartCoroutine(DownloadTSV());
    }

    private IEnumerator DownloadTSV()
    {
        UnityWebRequest www = UnityWebRequest.Get(_URL);
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;

        string[][] lines = data.Split("\n").ToList().Select(line=>line.Split("\t").ToArray()).ToArray();

        StringBuilder builder = new StringBuilder();
        for(int i = 0; i < lines.Length; i++)
        {
            builder.Clear();
            for(int j = 0; j < lines[i].Length; j++)
            {
                builder.Append(lines[i][j]);
                if (j < lines[i].Length - 1)
                    builder.Append(", ");
            }

            Debug.Log(builder.ToString());
        }


    }
}
