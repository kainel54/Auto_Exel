using Defective.JSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterSheetManager : ISheetManager
{
    private CharacterDataListSO _listSO;
    private readonly string _soPath = "Assets/08.SO";
    private readonly string _url = "https://script.google.com/macros/s/AKfycbxVYteemhrUuvO_iVdP3ee_bZWpDC-PIbDrPdhBOfTe7mRhxLTcgHaNDyLb6AghCYeD/exec";


    public override void Initialize(VisualElement root,SheetManagerEditor manager)
    {
        base.Initialize(root, manager);
        var uploadBtn= root.Q<Button>("UploadBtn");
        uploadBtn.clicked += UploadToSheet;
        var downloadBtn= root.Q<Button>("DownloadBtn");
        downloadBtn.clicked += DownloadFromSheet;
        if (_listSO == null)
            FindList();

        RefreshAsset();
    }
    private void FindList()
    {
        string path = $"{_soPath}/Character/List.asset";
        _listSO = AssetDatabase.LoadAssetAtPath<CharacterDataListSO>(path);

        if(_listSO == null)
        {
            _listSO = ScriptableObject.CreateInstance<CharacterDataListSO>();

            AssetDatabase.CreateAsset(_listSO, path);
            Debug.Log($"chracter list asset create at {path}");
        }
    }

    //����Ʈ���ٰ� ���� ���Ͽ� �ִ� ���µ��� �߰����ִ� �޼���
    private void RefreshAsset()
    {
        string path = $"{_soPath}/Character";
        string[] assetGuids = AssetDatabase.FindAssets("", new[] { path });

        _listSO.list = assetGuids.Select(x =>
        {
            string path = AssetDatabase.GUIDToAssetPath(x);
            var data = AssetDatabase.LoadAssetAtPath<CharacterDataSO>(path);
            return data;
        }).Where(x => x!=null).ToList();

        EditorUtility.SetDirty(_listSO);
        AssetDatabase.SaveAssets();
    }
    public override void DownloadFromSheet()
    {
        _manager.SetLoadingScreen(true);
        _manager.SendGetRequest(_url, msg =>
        {
            JSONObject obj = new JSONObject(msg); // ��Ʈ�� -> JSONOBJECT�� �����
            string path = $"{_soPath}/Character";

            var list = obj.list;
            int created = 0;
            int updated = 0;
            List<string> updatedGuid = new List<string>();

            foreach(var data in list)
            {
                string charaterName = data.GetField("characterName").stringValue;
                float moveSpeed = data.GetField("moveSpeed").floatValue;
                int maxHealth = data.GetField("maxHealth").intValue;
                string guid = data.GetField("guid").stringValue;

                var existSO = _listSO.FindCharDataByGUID(guid);
                if(existSO == null)
                {
                    existSO = ScriptableObject.CreateInstance<CharacterDataSO>();
                    AssetDatabase.CreateAsset(existSO, $"{path}/{charaterName}.asset");
                    _listSO.list.Add(existSO);
                    existSO.guid = guid;
                    EditorUtility.SetDirty(_listSO);
                    created++;
                }
                else
                {
                    updated++;
                    updatedGuid.Add(existSO.guid); //���ŵ� guid
                }
                existSO.maxHealth = maxHealth;
                existSO.moveSpeed = moveSpeed;
                existSO.charaterName = charaterName;

                EditorUtility.SetDirty(_listSO);

                if (existSO.name != charaterName)
                {
                    string assetPath = AssetDatabase.GetAssetPath(existSO);
                    AssetDatabase.RenameAsset(assetPath, charaterName);
                }
            }

            UpdateEmptyGUIDToServer();

            List<CharacterDataSO> notExistOnServer = new List<CharacterDataSO>();
            foreach (var data in _listSO.list)
            {
                if (updatedGuid.Any(x => !(x == data.guid)))
                {
                    notExistOnServer.Add(data);
                }
            }

            if (notExistOnServer.Count > 0)
            {
                StringBuilder builder = new StringBuilder();

                foreach(var data in notExistOnServer)
                {
                    builder.Append(data.name);
                    builder.AppendLine();
                }

                PopupString str = new PopupString
                {
                    title = "������ �������� �ʴ� ���ϵ�",
                    content = builder.ToString(),
                    confirmText = "�����ϱ�",
                    cancelText = "�����ϱ�"
                };

                _manager.ShowPopupScreen(str, () => {
                    for(int i = notExistOnServer.Count - 1; i >= 0; i--)
                    {
                        _listSO.list.Remove(notExistOnServer[i]);
                        string path = AssetDatabase.GetAssetPath(notExistOnServer[i]);
                        AssetDatabase.DeleteAsset(path);
                    }

                    EditorUtility.SetDirty(_listSO);
                    AssetDatabase.SaveAssets();
                }, null);
            }

            AssetDatabase.SaveAssets();
            _manager.SetLoadingScreen(false);
            _manager.ShowToastMessage($"Created : {created},Updated:{updated}", 2f);
        });
    }

    private void UpdateEmptyGUIDToServer()
    {
        var createdList = _listSO.list.Where(x => string.IsNullOrEmpty(x.guid)).ToList();
        if (createdList.Count == 0) return;

        createdList.ForEach(x => x.GenerateGUID());

        var json = CharacterDataListSO.ToJsonByData(createdList);

        _manager.SendPostRequest(url: _url, payload: json, type: "update_guid", msg =>
        {
            Debug.Log(msg);
        });
    }

    public override void UploadToSheet()
    {
        string data = _listSO.ToJson();

        _manager.SetLoadingScreen(true);
        //Debug.Log(data);
        _manager.SendPostRequest(url:_url, payload:data,type:"upload", (res) =>
        {
            _manager.SetLoadingScreen(false);
            Debug.Log(res);
            _manager.ShowToastMessage(res, 2);
        });
    }
}
