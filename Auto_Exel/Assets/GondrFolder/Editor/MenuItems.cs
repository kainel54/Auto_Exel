using UnityEditor;
using UnityEngine;

namespace Gondr.ColorFolder
{
    public static class MenuItems
    {
        private const int _priority = 100000;

        [MenuItem("Assets/Gondr/Red", false, _priority)]
        private static void Red()
        {
            ColoredFolderEditor.SetIconName("Red");
        }

        [MenuItem("Assets/Gondr/Green", false, _priority)]
        private static void Green()
        {
            ColoredFolderEditor.SetIconName("Green");
        }

        [MenuItem("Assets/Gondr/Blue", false, _priority)]
        private static void Blue()
        {
            ColoredFolderEditor.SetIconName("Blue");
        }

         [MenuItem("Assets/Gondr/Yellow", false, _priority)]
        private static void Yellow()
        {
            ColoredFolderEditor.SetIconName("Yellow");
        }

        [MenuItem("Assets/Gondr/Custom icon", false, _priority + 11)]
        private static void Custom()
        {
            IconFolderEditor.ChooseCustomIcon();
        }

        [MenuItem("Assets/Gondr/ResetIcon", false, _priority + 21)]
        private static void ResetIcon()
        {
            ColoredFolderEditor.ResetFolderIcon();
        }

        [MenuItem("Assets/Gondr/Red", true)]
        [MenuItem("Assets/Gondr/Green", true)]
        [MenuItem("Assets/Gondr/Blue", true)]
        [MenuItem("Assets/Gondr/Yellow", true)]
        [MenuItem("Assets/Gondr/Custom icon", true)]
        [MenuItem("Assets/Gondr/ResetIcon", true)]
        private static bool ValidateFolder()
        {
            if(Selection.activeObject == null)
                return false;
            
            Object selectObj = Selection.activeObject;

            string path = AssetDatabase.GetAssetPath(selectObj);
            return AssetDatabase.IsValidFolder(path);
        }

    }
}

