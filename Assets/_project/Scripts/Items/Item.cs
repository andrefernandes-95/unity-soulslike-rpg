namespace AFV2
{
    using UnityEngine;

    public class Item : MonoBehaviour
    {
        [Header("Basic Info")]
        public string portugueseName;

        public string DisplayName
        {
            get => Glossary.IsPortuguese() ? portugueseName : name;
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

        [Header("Value")]
        [SerializeField] private int value = 1;
        public int Value
        {
            get => value;
        }

        [Header("World Pickup")]
        public GameObject worldPickup;

        [Header("UI Options")]
        public bool hideFromUi = false;
    }
}
