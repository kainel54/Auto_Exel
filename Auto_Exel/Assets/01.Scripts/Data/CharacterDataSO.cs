using Defective.JSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName ="SO/Character/Data")]
public class CharacterDataSO : ScriptableObject, IToJson
{
    public float moveSpeed;
    public string charaterName;
    public int maxHealth;
    public string guid;
    public Sprite sprite;

    public string ToJson()
    {
        JSONObject obj = new JSONObject();
        obj.AddField("moveSpeed",moveSpeed);
        obj.AddField("characterName", charaterName);
        obj.AddField("maxHealth", maxHealth);
        obj.AddField("guid", guid);
        return obj.ToString();
    }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(guid))
            GenerateGUID();
    }

    public void GenerateGUID()
    {
        string path = AssetDatabase.GetAssetPath(this);
        GUID assetGUID = AssetDatabase.GUIDFromAssetPath(path);
        guid = assetGUID.ToString();
    }
}
