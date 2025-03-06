namespace AFV2
{
    using UnityEngine;
    using UnityEngine.Events;

    public class ArmorBase : Item
    {
        [Tooltip("Will subtract from incoming damage")]
        public Damage damageNegation;

        [Header("Events")]
        public UnityEvent OnEquip;
        public UnityEvent OnUnequip;

        [Header("Sounds")]
        public AudioClip equipSound;
    }
}
