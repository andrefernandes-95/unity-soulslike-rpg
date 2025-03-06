namespace AFV2
{
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayerInventory : CharacterInventory, ISaveable
    {
        const string ITEMS = "items";

        [Header("Components")]
        public InventoryBank inventoryBank;

        public void LoadData(SaveReader reader)
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

        public void SaveData(SaveWriter writer)
        {
            List<SerializedItem> itemsToSave = new();

            foreach (KeyValuePair<Item, List<ItemInstance>> ownedItem in OwnedItems)
            {
                foreach (var itemInstance in ownedItem.Value)
                {
                    SerializedItem serializedItem = new()
                    {
                        id = itemInstance.ID,
                        itemPath = "Resources/" + itemInstance.item.GetType() + "/" + itemInstance.item.name,
                    };

                    itemsToSave.Add(serializedItem);
                }
            }

            writer.Write(ITEMS, itemsToSave);
        }
    }
}
