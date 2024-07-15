using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Gondr.ColorFolder
{
    [InitializeOnLoad]
    public static class IconFolderEditor
    {
        private static string _selectedFolderGuid;
        private static int _controlID;

        static IconFolderEditor()
        {
            EditorApplication.projectWindowItemOnGUI -= OnGUICall;
            EditorApplication.projectWindowItemOnGUI += OnGUICall;
        }

        private static void OnGUICall(string guid, Rect selectionRect)
        {
            if(guid != _selectedFolderGuid) return;

            if(Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == _controlID)
            {
                var selectedObj = EditorGUIUtility.GetObjectPickerObject();

                string path = AssetDatabase.GetAssetPath(selectedObj);
                string textureGuid = AssetDatabase.GUIDFromAssetPath(path).ToString();

                ColoredFolderEditor.FolderOption.SetString(_selectedFolderGuid, textureGuid);
            }
        }

        public static void ChooseCustomIcon()
        {
            _selectedFolderGuid = Selection.assetGUIDs[0]; //선택된 폴더의 guid를 저장.

            //focus type은 passive와 keyboard가 있습니다. passive는 키 입력을 받지 않습니다.
            _controlID = EditorGUIUtility.GetControlID(FocusType.Passive);
            //오브젝트 - The object to be selected by default., 씬오브젝트허용여부, 필터, 컨트롤 아이디.
            EditorGUIUtility.ShowObjectPicker<Sprite>(null, false, "", _controlID);
            //여기서 선택을 완료하면 이벤트를 보낸다.
        }
    }
}
