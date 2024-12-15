using UnityEngine;
using Systems.SaveLoad;
using Systems.SaveLoad.Interface;

namespace Systems.SaveLoad.Service
{
    public class JsonSerializerService : ISerializer
    {
        public string Serialize<T>(T obj)
        {
            return JsonUtility.ToJson(obj, true);
        }
        public T Deserialize<T>(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }
    }
}