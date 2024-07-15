using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class SpritePair 
{   
    public string folderGuid;
    public string textureGuid;
}

public class FolderOptionSO : ScriptableObject
{
    private Dictionary<string, string> _spriteDictionary;
    public List<SpritePair> spriteList = new List<SpritePair>();

    private void OnEnable()
    {
        _spriteDictionary = new Dictionary<string, string>();

        foreach(var pair in spriteList)
        {
            if(string.IsNullOrEmpty(pair.folderGuid)){
                Debug.Log("There is empty guid in folder list");
                continue;
            }

            if(_spriteDictionary.ContainsKey(pair.folderGuid))
            {
                Debug.Log("There is duplicated guid in folder list");
                continue;
            }
            _spriteDictionary.Add(pair.folderGuid, pair.textureGuid);
        }
    }

    public void SetString(string folderGuid, string textureGuid)
    {
        if(_spriteDictionary.ContainsKey(folderGuid))
        {
            _spriteDictionary[folderGuid] = textureGuid;
            var pair = spriteList.Find(x => x.folderGuid == folderGuid);
            if(pair != null) 
            {
                pair.textureGuid = textureGuid;
            }
        }else{
            spriteList.Add(new SpritePair{ folderGuid = folderGuid, textureGuid = textureGuid });
            _spriteDictionary.Add(folderGuid, textureGuid);
        }
    }

    public string GetString(string folderGuid, string defaultValue)
    {
        if(_spriteDictionary.TryGetValue(folderGuid, out string textGuid))
        {
            return textGuid;
        }else {
            return defaultValue;
        }
    }

    public void DeleteKey(string guid)
    {
        if(_spriteDictionary.ContainsKey(guid))
        {
            _spriteDictionary.Remove(guid);

            var pair = spriteList.Find(x => x.folderGuid == guid);
            if(pair != null) 
            {
                spriteList.Remove(pair);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
