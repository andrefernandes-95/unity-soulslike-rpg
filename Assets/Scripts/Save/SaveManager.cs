namespace AFV2
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class SaveManager : MonoBehaviour
    {
        [SerializeField] InputListener inputListener;

        List<ISaveable> sceneSaveables = new();

        void Awake()
        {
            inputListener.onQuickSave.AddListener(QuickSave);
            inputListener.onQuickLoad.AddListener(QuickLoad);
            sceneSaveables = GetSceneSaveables();
        }

        void QuickSave()
        {
            SaveWriter saveWriter = SaveWriter.Create("quickSave");

            foreach (ISaveable saveable in sceneSaveables)
            {
                saveable.SaveData(saveWriter);
            }

            saveWriter.TryCommit();
        }

        void QuickLoad()
        {
            SaveReader saveReader = SaveReader.Load("quickSave");

            foreach (ISaveable saveable in sceneSaveables)
            {
                saveable.LoadData(saveReader);
            }
        }

        List<ISaveable> GetSceneSaveables()
        {
            return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .OfType<ISaveable>()  // Filters objects implementing ISaveable
                .ToList();
        }
    }
}
