namespace AFV2
{
    using CI.QuickSave;

    public interface ISaveable
    {
        void SaveData(QuickSaveWriter writer);
        void LoadData(QuickSaveReader reader);
    }
}
