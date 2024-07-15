using Defective.JSON;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogSheetManager : SheetManager
{
    private readonly string _soPath = "Assets/08.SO";
    private readonly string _url = "https://script.google.com/macros/s/AKfycbxVYteemhrUuvO_iVdP3ee_bZWpDC-PIbDrPdhBOfTe7mRhxLTcgHaNDyLb6AghCYeD/exec";

    public override void Initialize(VisualElement root, SheetManagerEditor manager)
    {
        base.Initialize(root, manager);
        root.Q<Button>("LoadDialogBtn").clicked += DownloadFromSheet;
    }

    public override void DownloadFromSheet()
    {
        _manager.SetLoadingScreen(true);
        _manager.SendGetRequest(_url, "dialog", msg =>
        {
            //현재 존재하는 데이터는 다 지우고 새로 받는다.
            _manager.SetLoadingScreen(false);
            DeleteExistAsset();

            JSONObject obj = new JSONObject(msg); // 메세지를 전부 json으로 변환

            string path = $"{_soPath}/Dialog";
            string chatPath = $"{path}/Chat";
            var list = obj.list;

            int created = 0;

            foreach(var data in list)
            {
                int id = data.GetField("id").intValue;
                string description = data.GetField("description").stringValue;
                var chatListJSON = data.GetField("chatList").list;

                var dialogSO = ScriptableObject.CreateInstance<DialogSO>();
                dialogSO.dialogID = id;
                dialogSO.description = description;
                dialogSO.chatList = new List<ChatSO>();

                int chatIndex = 1;

                foreach(var ch in chatListJSON)
                {
                    var chatso = ScriptableObject.CreateInstance<ChatSO>();
                    chatso.dialogID = id;
                    chatso.characterName = ch.GetField("characterName").stringValue;
                    chatso.text = ch.GetField("text").stringValue;

                    AssetDatabase.CreateAsset(chatso, $"{chatPath}/chat_{id}_{chatIndex}.asset");
                    chatIndex++;
                    dialogSO.chatList.Add(chatso);
                }

                AssetDatabase.CreateAsset(dialogSO, $"{path}/dialog_{id}.asset");
                created += chatListJSON.Count + 1;
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            _manager.ShowToastMessage($"Created{created} Dialogs", 2f);
        });
    }

    private void DeleteExistAsset()
    {
        string dialogPath = $"{_soPath}/Dialog";
        string[] assetNames = AssetDatabase.FindAssets("", new[] { dialogPath });
        List<string> assetPaths = new List<string>();
        foreach(string guid in assetNames)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ChatSO chatData = AssetDatabase.LoadAssetAtPath<ChatSO>(path);
            if(chatData != null)
            {
                assetPaths.Add(path);
                continue;
            }

            DialogSO dialogData = AssetDatabase.LoadAssetAtPath<DialogSO>(path);
            if(dialogData != null)
            {
                assetPaths.Add(path);
                continue;
            }
        }
        List<string> failedAssetPath = new List<string>();
        AssetDatabase.DeleteAssets(assetPaths.ToArray(), failedAssetPath);

        if (failedAssetPath.Count > 0)
        {
            Debug.Log($"Failed : {failedAssetPath.Count} Asset delete failed");
            failedAssetPath.ForEach(p => Debug.Log(p));
        }
        int deleted = assetPaths.Count - failedAssetPath.Count;
        Debug.Log($"{deleted} Asset Deleted!");
    }

    public override void UploadToSheet()
    {

    }

    
}
