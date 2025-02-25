using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace AFV2
{
    public class SaveWriter
    {
        private Dictionary<string, object> data = new Dictionary<string, object>();
        private string filePath;

        private SaveWriter(string fileName)
        {
            filePath = Path.Combine(Application.persistentDataPath, fileName + ".json");
        }

        public static SaveWriter Create(string fileName)
        {
            return new SaveWriter(fileName);
        }

        // ✅ Nested key writing with "." notation ("player.stats.health")
        public void Write<T>(string path, T value)
        {
            var keys = path.Split('.'); // Split "player.stats.health" into ["player", "stats", "health"]
            Dictionary<string, object> currentDict = data;

            for (int i = 0; i < keys.Length - 1; i++)
            {
                string currentKey = keys[i];

                if (!currentDict.ContainsKey(currentKey) || !(currentDict[currentKey] is Dictionary<string, object>))
                {
                    currentDict[currentKey] = new Dictionary<string, object>(); // Create if not existing
                }

                currentDict = (Dictionary<string, object>)currentDict[currentKey];
            }

            currentDict[keys[^1]] = value; // Assign the final value
        }

        public bool TryCommit()
        {
            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(filePath, json);

                Debug.Log($"Data successfully saved to: {filePath}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save data: {e.Message}");
                return false;
            }
        }
    }
}
