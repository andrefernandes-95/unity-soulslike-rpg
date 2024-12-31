
namespace AF
{
    using AF.Inventory;
    using AYellowpaper.SerializedCollections;
    using UnityEngine;
    using UnityEngine.Localization;

    [CreateAssetMenu(menuName = "Combat / New Combatant")]
    public class Combatant : ScriptableObject
    {
        [Header("Name")]
        public LocalizedString name;

        [Header("Attributes")]
        public int health = 400;
        [Tooltip("Applicable to player only")] public int stamina = 150;
        [Tooltip("Applicable to player only")] public int mana = 100;

        [Header("Poise")]
        public int poise = 1;

        [Header("Posture")]
        public int posture = 100;

        [Header("Backstab")]
        public bool allowBackstabs = true;

        [Header("Status Effect Resistances")]
        [SerializedDictionary("Status Resistances", "Duration (seconds)")]
        public SerializedDictionary<StatusEffect, float> statusEffectResistances = new();

        [Header("Loot")]
        [SerializedDictionary("Item", "Chance To Get")]
        public SerializedDictionary<Item, LootItemAmount> lootTable;

        [Header("Experience")]
        public int gold = 400;

        public enum InheritOption
        {
            INHERIT,
            OVERRIDE,
            MERGE,
        }
    }
}
