using System;

namespace AFV2
{
    [System.Serializable]
    public class ItemInstance
    {
        public Item item;
        public string ID;

        public ItemInstance(Item item)
        {
            this.item = item;
            ID = Guid.NewGuid().ToString();
        }

    }
}
