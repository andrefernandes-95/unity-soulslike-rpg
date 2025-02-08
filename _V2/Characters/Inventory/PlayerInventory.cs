namespace AFV2
{
    using System.Collections.Generic;
    using AF;
    using CI.QuickSave;
    using UnityEngine;

    public class PlayerInventory : CharacterInventory, ISaveable
    {
        const string ITEMS = "items";

        [Header("Components")]
        public InventoryBank inventoryBank;

        public void LoadData(QuickSaveReader reader)
        {
            if (reader.TryRead(ITEMS, out List<SerializedItem> items))
            {
                foreach (SerializedItem item in items)
                {
                    InventoryUtils.AddSerializedItemToCharacterInventory(
                        this, item, inventoryBank
                    );
                }
            }
        }

        public void SaveData(QuickSaveWriter writer)
        {
            List<SerializedItem> itemsToSave = new();

            foreach (Item item in Items)
            {
                SerializedItem serializedItem = new()
                {
                    name = item.name
                };

                itemsToSave.Add(serializedItem);
            }

            writer.Write(ITEMS, itemsToSave);
        }
    }
}
