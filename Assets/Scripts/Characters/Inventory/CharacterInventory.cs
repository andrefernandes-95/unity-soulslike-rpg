namespace AFV2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.AI;

    public class CharacterInventory : MonoBehaviour
    {
        [SerializeField] CharacterApi characterApi;

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

        public void DropItem(ItemInstance itemInstance)
        {
            WorldItemPickup worldItemInstance = Instantiate(itemInstance.item.worldPickup);

            worldItemInstance.SetWorldItem(itemInstance.item, 1);
            worldItemInstance.transform.parent = null;

            NavMesh.SamplePosition(transform.position, out var hit, 1.0f, NavMesh.AllAreas);
            worldItemInstance.transform.position = hit.position;

            RemoveItem(itemInstance);
        }

        public void RemoveItem(ItemInstance itemInstance)
        {
            if (itemInstance == null)
            {
                return;
            }

            if (!ownedItems.TryGetValue(itemInstance.item, out var itemList))
            {
                return;
            }

            int itemIndexForRemoval = itemList.FindIndex(_item => _item.ID == itemInstance.ID);

            if (itemIndexForRemoval == -1)
            {
                return;
            }

            UnequipItemInstance(itemInstance);

            itemList.RemoveAt(itemIndexForRemoval);

            if (itemList.Count == 0)
            {
                ownedItems.Remove(itemInstance.item);
            }
        }

        void UnequipItemInstance(ItemInstance itemInstance)
        {
            if (itemInstance is WeaponInstance weaponInstance)
            {
                int rightWeaponIdx = Array.FindIndex(characterApi.characterWeapons.rightWeapons, x => x?.ID == weaponInstance.ID);
                if (rightWeaponIdx != -1)
                {
                    characterApi.characterWeapons.UnequipRightWeapon(rightWeaponIdx);
                    return;
                }

                int leftWeaponIdx = Array.FindIndex(characterApi.characterWeapons.leftWeapons, x => x?.ID == weaponInstance.ID);
                if (leftWeaponIdx != -1)
                {
                    characterApi.characterWeapons.UnequipLeftWeapon(leftWeaponIdx);
                }
            }
            else if (itemInstance is ArrowInstance arrowInstance)
            {
                int arrowIdx = Array.FindIndex(characterApi.characterArchery.arrows, x => x == arrowInstance.item);
                characterApi.characterArchery.UnequipArrow(arrowIdx);
            }

        }

        public bool HasItem(Item item) => ownedItems.ContainsKey(item);

        public int GetItemCount(Item item)
        {
            if (OwnedItems.TryGetValue(item, out var itemInstances))
            {
                return itemInstances.Count;
            }
            return 0;
        }

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
