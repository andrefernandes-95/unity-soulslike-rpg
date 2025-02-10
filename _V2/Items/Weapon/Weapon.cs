namespace AFV2
{
    using System.Collections.Generic;
    using AF;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;


#if UNITY_EDITOR

    [CustomEditor(typeof(Weapon), editorForChildClasses: true)]
    public class WeaponEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Weapon weapon = target as Weapon;

            CharacterEquipment characterEquipment = weapon
                ?.GetComponentInParent<CharacterApi>()
                ?.characterEquipment;

            if (characterEquipment == null) return;

            if (GUILayout.Button("Equip Right Weapon (Slot 0)"))
                characterEquipment.characterWeapons.EquipRightWeapon(weapon);

            if (GUILayout.Button("Unequip Right Weapon (Slot 0)"))
                characterEquipment.characterWeapons.UnequipRightWeapon();

            if (GUILayout.Button("Equip Left Weapon (Slot 0)"))
                characterEquipment.characterWeapons.EquipLeftWeapon(weapon);

            if (GUILayout.Button("Unequip Left Weapon (Slot 0)"))
                characterEquipment.characterWeapons.UnequipLeftWeapon();
        }
    }
#endif

    [RequireComponent(typeof(WeaponAnimations))]
    [RequireComponent(typeof(WeaponRenderer))]
    [RequireComponent(typeof(OneHandPivots))]
    [RequireComponent(typeof(TwoHandPivots))]
    public class Weapon : Item
    {
        [Header("Scaling")]
        public WeaponScaling strengthScaling = WeaponScaling.E;
        public WeaponScaling dexterityScaling = WeaponScaling.E;
        public WeaponScaling intelligenceScalling = WeaponScaling.E;

        [Header("Events")]
        public UnityEvent OnEquip;
        public UnityEvent OnUnequip;

        [Header("Components")]
        OneHandPivots oneHandPivots => GetComponent<OneHandPivots>();
        TwoHandPivots twoHandPivots => GetComponent<TwoHandPivots>();
        WeaponRenderer weaponRenderer => GetComponent<WeaponRenderer>();
        WeaponAnimations weaponAnimations => GetComponent<WeaponAnimations>();

        Dictionary<Transform, Weapon> instances = new();

        CharacterApi characterApi;

        private void OnEnable()
        {
            if (characterApi == null)
                characterApi = GetComponentInParent<CharacterApi>();
        }

        public void Equip(Transform parent, bool isRightHand)
        {
            instances[parent] = Instantiate(this, parent);
            instances[parent].name += " " + (isRightHand ? "(Right)" : "(Left)");

            ApplyPivots(instances[parent], isRightHand);
            ApplyAnimations(instances[parent]);

            instances[parent].weaponRenderer.EnableRenderer();
        }

        public void Unequip(Transform parent)
        {
            if (instances[parent] != null)
                Destroy(instances[parent].gameObject);
        }

        public void ApplyPivots(Weapon instance, bool isRightHand)
        {
            if (characterApi.characterEquipment.characterWeapons.IsTwoHanding)
            {
                instance.transform.localPosition = twoHandPivots.rightHandPivot;
                instance.transform.localEulerAngles = twoHandPivots.rightHandRotation;
                return;
            }

            instance.transform.localPosition = isRightHand ? oneHandPivots.rightHandPivot : oneHandPivots.leftHandPivot;
            instance.transform.localEulerAngles = isRightHand ? oneHandPivots.rightHandRotation : oneHandPivots.leftHandRotation;
        }

        public void ApplyAnimations(Weapon instance) => instance.weaponAnimations.ApplyAnimations();
    }
}
