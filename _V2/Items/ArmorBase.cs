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

            GUI.enabled = Application.isPlaying;

            ArmorBase armorBase = target as ArmorBase;

            if (GUILayout.Button("Equip"))
                armorBase.OnEquip.Invoke();

            if (GUILayout.Button("Unequip"))
                armorBase.OnUnequip.Invoke();
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

    }
}
