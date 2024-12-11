using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ICON.Utilities
{
    public enum LabelIcon
    {
        Gray,
        Blue,
        Teal,
        Green,
        Yellow,
        Orange,
        Red,
        Purple
    }
    
    public static class GameObjectExtensions
    {
        public static void SetIcon(this GameObject gameObject, LabelIcon labelIcon)
        {
            IconManager.SetIcon(gameObject, $"sv_label_{(int)labelIcon}");
        }
    }
    
    public static class IconManager
    {
        private static MethodInfo setIconForObjectMethodInfo;
   
        public static void SetIcon(GameObject gameObject, string contentName)
        {
            GUIContent iconContent = EditorGUIUtility.IconContent(contentName);
            SetIconForObject(gameObject, (Texture2D) iconContent.image);
        }

        public static void SetIconForObject(GameObject obj, Texture2D icon)
        {
            if (obj == null)
            {
                Debug.LogError("GameObject is null. Cannot set icon.");
                return;
            }

            if (icon == null)
            {
                Debug.LogError("Icon is null. Cannot set icon for GameObject.");
                return;
            }

            if (setIconForObjectMethodInfo == null)
            {
                Type type = typeof(EditorGUIUtility);
                setIconForObjectMethodInfo = type.GetMethod("SetIconForObject", BindingFlags.Static | BindingFlags.NonPublic);

                if (setIconForObjectMethodInfo == null)
                {
                    Debug.LogError("SetIconForObject method not found. Unity API may have changed.");
                    return;
                }
            }

            try
            {
                setIconForObjectMethodInfo.Invoke(null, new object[] { obj, icon });
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to set icon: {ex.Message}");
            }
        }

    }

}