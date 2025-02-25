namespace AFV2
{
    using System.Collections.Generic;
    using UnityEngine;

    public class CharacterInventory : MonoBehaviour
    {
        [SerializeField] private List<Item> items = new();
        public List<Item> Items
        {
            get => items;
        }

        public Item AddItem(Item item, int amount)
        {
            if (item == null)
            {
                Debug.LogError("Could not add item because it was null");
                return null;
            }

            Item instance = null;

            for (int i = 0; i < amount; i++)
            {
                instance = Instantiate(item, transform);
                items.Add(instance);
            }

            return instance;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(Item item) => AddItem(item, 1);

        public bool HasItem(Item item) => Items.Exists(itemInInventory => itemInInventory.name == item.name);
    }
}
