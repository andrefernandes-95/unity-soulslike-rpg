namespace AFV2
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class CharacterInventory : MonoBehaviour
    {
        [SerializeField] private Dictionary<Item, List<ItemInstance>> ownedItems = new();
        public IReadOnlyDictionary<Item, List<ItemInstance>> OwnedItems => ownedItems;

        public ItemInstance AddItem(Item item)
        {
            var createdItem = new ItemInstance(item);

            if (item is Weapon)
            {
                createdItem = new WeaponInstance(item);
            }
            else if (item is Arrow)
            {
                createdItem = new ArrowInstance(item);
            }

            if (ownedItems.ContainsKey(item))
            {
                ownedItems[item].Add(createdItem);
            }
            else
            {
                ownedItems.Add(item, new() { createdItem });
            }

            return createdItem;
        }

        public bool HasItem(Item item) => ownedItems.ContainsKey(item);

        public IEnumerable<ItemInstance> GetItems<T>() where T : Item
        {
            var filteredList = OwnedItems
                .Where(item => item.Key is T)
                .SelectMany(item => item.Value)
                .OfType<ItemInstance>();

            return filteredList;
        }

        #region Unity Events
        public void OnAddItem(Item item)
        {
            AddItem(item);
        }
        #endregion
    }
}
