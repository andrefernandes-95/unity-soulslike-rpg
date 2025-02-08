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

        public void AddItem(Item item, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Item instance = Instantiate(item, transform);
                items.Add(instance);
            }
        }
    }
}
