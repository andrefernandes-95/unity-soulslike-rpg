namespace AFV2
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class InventoryBank : MonoBehaviour
    {
        List<Item> items = new();
        public List<Item> Items
        {
            get { return items; }
        }

        private void Awake()
        {
            items = transform.GetComponentsInChildren<Item>().ToList();
        }

        public Item FindByGameObjectName(string name) => items.FirstOrDefault(
            item => item.name == name
        );
    }
}
