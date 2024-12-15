using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using UnityEditor;


namespace Systems.Scriptable.Events
{
    [CreateAssetMenu(fileName = "Observer", menuName = "Events/Observer")]
    public class Observer : ScriptableObject
    {
        private static Observer _instance;

        public static Observer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<Observer>("GlobalObserver");
                    if (_instance == null)
                    {
                        _instance = CreateInstance<Observer>();
                        string path = "Assets/Resources/GlobalObserver.asset";
                        AssetDatabase.CreateAsset(_instance, path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
                return _instance;
            }
        }
        static Dictionary<string, List<Action<object[]>>> Listeners =
            new Dictionary<string, List<Action<object[]>>>();
        public void AddObserver(string name, Action<object[]> callback)
        {
            if (!Listeners.ContainsKey(name))
                Listeners.Add(name, new List<Action<object[]>>());

            Listeners[name].Add(callback);
        }

        public void RemoveListener(string name, Action<object[]> callback)
        {
            if (!Listeners.ContainsKey(name))
                return;

            Listeners[name].Remove(callback);
        }
        public void Notify(string name, params object[] data)
        {
            if (!Listeners.ContainsKey(name))
                return;

            foreach (var item in Listeners[name])
            {
                try
                {
                    item?.Invoke(data);
                }
                catch (Exception)
                {
                    
                }
            }
        }
    }
}
