namespace AFV2
{
    using AF.Health;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;

#if UNITY_EDITOR

    [CustomEditor(typeof(ArmorBase), editorForChildClasses: true)]
    public class EventEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ArmorBase armorBase = target as ArmorBase;

            if (GUILayout.Button("Equip"))
                armorBase.Equip();

            if (GUILayout.Button("Unequip"))
                armorBase.Unequip();
        }
    }
#endif

    public class ArmorBase : Item
    {
        [Tooltip("Will subtract from incoming damage")]
        public Damage damageNegation;

        [Header("Events")]
        public UnityEvent OnEquip;
        public UnityEvent OnUnequip;

        [Header("Sounds")]
        public AudioClip equipSound;

        CharacterApi characterApi => GetComponentInParent<CharacterApi>();

        public void Equip()
        {
            if (equipSound != null)
                characterApi.characterSound.PlaySound(equipSound);

            OnEquip?.Invoke();
        }

        public void Unequip()
        {
            OnUnequip?.Invoke();
        }



    }
}
