namespace AF
{
    using System.Linq;
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

        [Header("Sounds")]
        public AudioClip conversationClip;
        public AudioClip hurtClip;
        public AudioClip knockdownClip;
        public AudioClip deathClip;
        public AudioClip evadeClip;

        public enum InheritOption
        {
            INHERIT,
            OVERRIDE,
            MERGE,
        }

        [Header("Factions")]
        public Combatant[] friendlies;

        public bool IsFriendsWith(Combatant possibleFriendly)
        {
            if (possibleFriendly == this)
            {
                return true;
            }

            if (friendlies == null || friendlies.Length <= 0)
            {
                return false;
            }

            return friendlies.Any(friend => friend == possibleFriendly);
        }

        public void PlayConversation(AudioSource audioSource)
        {
            if (audioSource == null || conversationClip == null)
            {
                return;
            }

            audioSource.PlayOneShot(conversationClip);
        }
        public void PlayHurt(AudioSource audioSource)
        {
            if (audioSource == null || hurtClip == null)
            {
                return;
            }

            audioSource.PlayOneShot(hurtClip);
        }
        public void PlayKnockdown(AudioSource audioSource)
        {
            if (audioSource == null || knockdownClip == null)
            {
                return;
            }

            audioSource.PlayOneShot(knockdownClip);
        }
        public void PlayDeath(AudioSource audioSource)
        {
            if (audioSource == null || deathClip == null)
            {
                return;
            }

            audioSource.PlayOneShot(deathClip);
        }
        public void PlayEvade(AudioSource audioSource)
        {
            if (audioSource == null || evadeClip == null)
            {
                return;
            }

            audioSource.PlayOneShot(evadeClip);
        }
    }
}
