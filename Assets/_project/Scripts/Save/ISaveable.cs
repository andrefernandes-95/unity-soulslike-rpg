namespace AFV2
{
    public interface ISaveable
    {
        void SaveData(SaveWriter writer);
        void LoadData(SaveReader reader);
    }
}
