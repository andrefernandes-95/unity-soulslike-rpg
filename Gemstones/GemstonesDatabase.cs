
namespace AF
{
    using System.Collections.Generic;
    using System.Linq;
    using AF.Inventory;
    using AYellowpaper.SerializedCollections;
    using UnityEditor;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Gemstones Database", menuName = "System/New Gemstone Database", order = 0)]
    public class GemstonesDatabase : ScriptableObject
    {
        public SerializedDictionary<string, List<GemstoneInstance>> weaponsWithGemstones = new();

        public InventoryDatabase inventoryDatabase;

#if UNITY_EDITOR
        private void OnEnable()
        {
            // No need to populate the list; it's serialized directly
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                // Clear the list when exiting play mode
                Clear();
            }
        }
#endif

        public void Clear()
        {
            weaponsWithGemstones.Clear();
        }

        public void AttachGemstoneToWeapon(WeaponInstance weaponInstance, GemstoneInstance gemstoneInstance)
        {
            if (weaponsWithGemstones.ContainsKey(weaponInstance.GetId()))
            {

                int gemstoneIndex = weaponsWithGemstones[weaponInstance.GetId()].FindIndex(x => x.GetId() == gemstoneInstance.GetId());

                if (gemstoneIndex != -1)
                {
                    // Already equipped
                    return;
                }

                weaponsWithGemstones[weaponInstance.GetId()].Add(gemstoneInstance);
            }
            else
            {
                weaponsWithGemstones.Add(weaponInstance.GetId(), new() { gemstoneInstance });
            }
        }

        public void DettachGemstoneFromWeapon(WeaponInstance weaponInstanceToAttach, GemstoneInstance gemstoneInstance)
        {
            if (weaponsWithGemstones.ContainsKey(weaponInstanceToAttach.GetId()))
            {
                int gemstoneIndex = weaponsWithGemstones[weaponInstanceToAttach.GetId()].FindIndex(x => x.GetId() == gemstoneInstance.GetId());

                if (gemstoneIndex != -1)
                {
                    weaponsWithGemstones[weaponInstanceToAttach.GetId()].RemoveAt(gemstoneIndex);
                }
            }
        }

        public GemstoneInstance[] GetAttachedGemstoneInstancesFromWeapon(WeaponInstance weaponInstance)
        {
            if (!weaponsWithGemstones.ContainsKey(weaponInstance.GetId()))
            {
                return new List<GemstoneInstance>().ToArray();
            }

            return weaponsWithGemstones[weaponInstance.GetId()].ToArray();
        }

        public Gemstone[] GetAttachedGemstonesFromWeapon(WeaponInstance weaponInstance)
        {
            if (!weaponsWithGemstones.ContainsKey(weaponInstance.GetId()))
            {
                return new List<Gemstone>().ToArray();
            }

            return weaponsWithGemstones[weaponInstance.GetId()].Select(gemstoneInstance => gemstoneInstance.GetItem()).ToArray();
        }

        public string GetWeaponIdByAttachedGemstone(GemstoneInstance gemstoneInstance)
        {
            if (weaponsWithGemstones.Count <= 0)
            {
                return null;
            }

            var match = weaponsWithGemstones.FirstOrDefault(x => x.Value.Where(xGemstone => xGemstone.GetId() == gemstoneInstance.GetId()) != null);

            return match.Key;
        }

    }
}
