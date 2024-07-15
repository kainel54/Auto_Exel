using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class SheetManagerEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private VisualElement _loadingScreen;
    private ToastMsg _toastMessage;
    private readonly string _popupTreePath = "Assets/10.UIToolKit/Editor/Popup/PopupPanel.uxml";
    private VisualTreeAsset _popupTreeAsset;

    private CharacterSheetManager _characterSheet;
    private DialogSheetManager _dialogSheet;

    private void OnEnable()
    {
        if (_characterSheet == null)
            _characterSheet = new CharacterSheetManager();

        if (_toastMessage == null)
            _toastMessage = new ToastMsg();

        if(_dialogSheet == null){
            _dialogSheet = new DialogSheetManager();
        }
        _popupTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_popupTreePath);
    }

    public void ShowPopupScreen(PopupString str,Action Confirm,Action Cancle)
    {
        var popupRoot = _popupTreeAsset.Instantiate().Q("Popup");
        rootVisualElement.Add(popupRoot);
        new PopupPanel(popupRoot, str, Confirm, Cancle);
    }

    [MenuItem("Tools/SheetManager")]
    public static void ShowSheetManager()
    {
        SheetManagerEditor wnd = GetWindow<SheetManagerEditor>();
        wnd.titleContent = new GUIContent("SheetManager");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        VisualElement template = m_VisualTreeAsset.Instantiate();
        root.Add(template);
        template.style.flexGrow = 1;

        var characterRoot = root.Q("CharacterContainer");
        _characterSheet.Initialize(characterRoot, this);

        _loadingScreen = root.Q("Loading");

        var toastRoot = root.Q("Toast");
        _toastMessage.Initialize(toastRoot);

        var dialogRoot = root.Q("DialogContainer");
        _dialogSheet.Initialize(dialogRoot,this);

        //ShowPopupScreen(new PopupString { title = "안녕", content = "내용", cancelText = "취소", confirmText = "확인" }, null, null);
    }

    public void SetLoadingScreen(bool value)
    {
        if (value)
            _loadingScreen.AddToClassList("on");
        else
            _loadingScreen.RemoveFromClassList("on");
    }

    public void ShowToastMessage(string message, float time)
    {
        _toastMessage.ShowMessage(message, time);
    }

    public async void SendPostRequest(
        string url, string payload,string type, Action<string> OnComplete)
    {
        WWWForm form = new WWWForm();
        form.AddField("payload", payload);
        form.AddField("type", type);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            var asyncOP = www.SendWebRequest();
            await asyncOP;

            if (asyncOP.isDone)
            {
                OnComplete?.Invoke(www.downloadHandler.text);
            }
            else
            {
                OnComplete?.Invoke("Connection Failed");
            }
        }
    }

    public async void SendGetRequest(string url, string type, Action<string> Oncomplete)
    {
        url = string.IsNullOrEmpty(type) ? url : $"{url}?type={type}";
        using(UnityWebRequest www = UnityWebRequest.Get(url))
        {
            var asyncOP = www.SendWebRequest();

            await asyncOP;

            if (asyncOP.isDone)
            {
                Oncomplete?.Invoke(www.downloadHandler.text);
            }
            else
            {
                Oncomplete?.Invoke("Error:COnnection Failure");
            }

        }
    }
}
