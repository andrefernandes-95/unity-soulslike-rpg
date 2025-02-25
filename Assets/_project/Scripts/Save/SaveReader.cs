namespace AFV2
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using UnityEngine;

    public class SaveReader
    {
        private Dictionary<string, object> data;

        private SaveReader(Dictionary<string, object> savedData)
        {
            data = ConvertJObjects(savedData);
        }

        public static SaveReader Load(string fileName)
        {
            string filePath = Path.Combine(Application.persistentDataPath, fileName + ".json");

            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"Save file not found: {filePath}");
                return new SaveReader(new Dictionary<string, object>());
            }

            try
            {
                string json = File.ReadAllText(filePath);
                var loadedData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                return new SaveReader(loadedData ?? new Dictionary<string, object>());
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load data: {e.Message}");
                return new SaveReader(new Dictionary<string, object>());
            }
        }

        public bool TryRead<T>(string key, out T value)
        {
            if (data.TryGetValue(key, out object obj))
            {
                try
                {
                    value = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to read key '{key}': {e.Message}");
                }
            }

            value = default;
            return false;
        }

        public bool TryRead<T>(string key, string subKey, out T value)
        {
            if (data.TryGetValue(key, out object obj) && obj is Dictionary<string, object> nestedDict)
            {
                if (nestedDict.TryGetValue(subKey, out object subValue))
                {
                    try
                    {
                        value = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(subValue));
                        return true;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to read '{key}/{subKey}': {e.Message}");
                    }
                }
            }

            value = default;
            return false;
        }

        private static Dictionary<string, object> ConvertJObjects(Dictionary<string, object> originalData)
        {
            var converted = new Dictionary<string, object>();

            foreach (var kvp in originalData)
            {
                if (kvp.Value is JObject jObject)
                {
                    converted[kvp.Key] = jObject.ToObject<Dictionary<string, object>>();
                }
                else
                {
                    converted[kvp.Key] = kvp.Value;
                }
            }

            return converted;
        }
    }
}
