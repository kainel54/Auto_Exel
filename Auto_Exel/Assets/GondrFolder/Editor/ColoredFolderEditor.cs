using UnityEngine;
using UnityEditor;
using System;

namespace Gondr.ColorFolder
{
    //유니티가 로드될 때 이 클래스가 초기화된다. 
    //컴파일, 플레이모드 진입(Awake() 호출 이전) 
    [InitializeOnLoad]
    public static class ColoredFolderEditor
    {
        private static FolderOptionSO _folderOption;
        public static FolderOptionSO FolderOption 
        {
            get {
                if(_folderOption == null)
                {
                    string[] guids = AssetDatabase.FindAssets("FolderOption");
            
                    foreach(var guid in guids)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        var option = AssetDatabase.LoadAssetAtPath<FolderOptionSO>(path);
                        if(option != null){
                            _folderOption = option;
                            break;
                        }
                    }
                }
                if(_folderOption == null)
                {
                    var so = ScriptableObject.CreateInstance<FolderOptionSO>();
                    AssetDatabase.CreateAsset(so, "Assets/FolderOption.asset"); //루트폴더에 생성
                    _folderOption = so;
                }
                return _folderOption;
            }
        }

        private static string _iconName;
        static ColoredFolderEditor()
        {
            EditorApplication.projectWindowItemOnGUI -= OnGUICall;
            EditorApplication.projectWindowItemOnGUI += OnGUICall;
        }

        private static void OnGUICall(string guid, Rect selectionRect)
        {
            GetRectsAndColor(selectionRect, out Rect folderRect, out Color backColor);            


            string iconGuid = FolderOption.GetString(guid, ""); //만약 저장된 값이 있다면. 
            
            if(string.IsNullOrEmpty(iconGuid) || iconGuid == "00000000000000000000000000000000")
                return;

            string iconPath = AssetDatabase.GUIDToAssetPath(iconGuid);
            EditorGUI.DrawRect(folderRect, backColor);
            Texture2D folderTex = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            GUI.DrawTexture(folderRect, folderTex);
                        
        }

        private static void GetRectsAndColor(Rect sr, out Rect folderRect, out Color backColor)
        {
            backColor = new Color(0.2f, 0.2f, 0.2f);
            if(sr.x < 15)  
            {
                //두번째 컬럼이고 축소상태
                folderRect = new Rect(sr.x + 3, sr.y, sr.height,sr.height); //정사각형으로
            }else if(sr.x >= 15 && sr.height < 25)
            {
                //첫번째 컬럼
                backColor = new Color(0.22f,0.22f,0.22f);
                folderRect = new Rect(sr.x, sr.y, sr.height,sr.height); //정사각형으로
            }else {
                //두번째 컬럼이고 확대상태
                folderRect = new Rect(sr.x, sr.y, sr.width, sr.width);
            }
        }

        public static void SetIconName(string iconName)
        {
            //Validate에 의해 여기까지 왔으면 Selection에 오브젝트가 들어가 있는 상태이다.
            //Guid를 쓰는 이유는 Path와 달리 guid는 정적이라 폴더를 이동해도 변함이 없다. 그래서 guid를 사용.
            string folderGuid = GetActiveObjectGuid();

            string iconPath = $"Assets/GondrFolder/Icons/{iconName}.png";
            string iconGuid = AssetDatabase.GUIDFromAssetPath(iconPath).ToString();

            //에디터에 정보저장.
            FolderOption.SetString(folderGuid, iconGuid);
        }

        public static void ResetFolderIcon()
        {
            FolderOption.DeleteKey(GetActiveObjectGuid());
        }

        private static string GetActiveObjectGuid()
        {
            var obj = Selection.activeObject;
            string folderPath = AssetDatabase.GetAssetPath(obj);
            return AssetDatabase.GUIDFromAssetPath(folderPath).ToString();
        }
    }

}
