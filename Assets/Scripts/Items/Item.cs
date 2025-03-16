namespace AFV2
{
    using UnityEngine;

    public class Item : ScriptableObject
    {
        [Header("📃 Basic Info")]
        [SerializeField] string englishName;
        [SerializeField] string portugueseName;

        [HideInInspector]
        public string DisplayName
        {
            get => Glossary.IsPortuguese() ? portugueseName : englishName;
        }

        [Space(10)]

        [SerializeField]
        [TextArea(minLines: 1, maxLines: 5)]
        string englishDescription;

        [SerializeField]
        [TextArea(minLines: 1, maxLines: 5)]
        string portugueseDescription;

        public string Description
        {
            get => Glossary.IsPortuguese() ? portugueseDescription : englishDescription;
        }

        [Space(10)]

        [SerializeField] Sprite sprite;
        public Sprite Sprite
        {
            get => sprite;
        }

        [Header("💰 Value")]
        [SerializeField] private int value = 1;
        public int Value
        {
            get => value;
        }

        [Header("⚖️ Weight")]
        [SerializeField] private int weight = 1;
        public int Weight
        {
            get => weight;
        }

        [Header("World Pickup")]
        public GameObject worldPickup;
    }
}
