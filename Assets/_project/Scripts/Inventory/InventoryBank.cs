namespace AFV2
{
    using System.Collections.Generic;
    using UnityEngine;

    public class InventoryBank : MonoBehaviour
    {
        Dictionary<string, Item> items = new();
        public Dictionary<string, Item> Items
        {
            get { return items; }
        }

        private void Awake()
        {
            foreach (Item item in transform.GetComponentsInChildren<Item>(true))
            {
                if (items.ContainsKey(item.name))
                {
                    Debug.LogError($"Found duplicate item name {item.name}");
                    continue;
                }

                items.Add(item.name, item);
            }
        }
    }
}
