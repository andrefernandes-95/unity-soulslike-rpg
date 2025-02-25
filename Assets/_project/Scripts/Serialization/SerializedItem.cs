namespace AFV2
{
    [System.Serializable]
    public class SerializedItem
    {
        public string name;

        // OLD
        public string itemPath;
        public string id;
        public bool wasUsed;
        public int level = 0;

        public string[] attachedGemstoneIds;
    }
}
