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
                characterEquipment.characterWeapons.EquipRightWeapon(weapon, 0);

            if (GUILayout.Button("Unequip Right Weapon (Slot 0)"))
                characterEquipment.characterWeapons.UnequipRightWeapon(0);

            if (GUILayout.Button("Equip Left Weapon (Slot 0)"))
                characterEquipment.characterWeapons.EquipLeftWeapon(weapon, 0);

            if (GUILayout.Button("Unequip Left Weapon (Slot 0)"))
                characterEquipment.characterWeapons.UnequipLeftWeapon(0);
        }
    }
#endif

    [RequireComponent(typeof(WeaponAnimations))]
    [RequireComponent(typeof(WeaponRenderer))]
    [RequireComponent(typeof(OneHandPivots))]
    [RequireComponent(typeof(TwoHandPivots))]
    [RequireComponent(typeof(WeaponHitbox))]
    public class Weapon : Item
    {
        #region Scaling
        [Header("Scaling")]
        public WeaponScaling strengthScaling = WeaponScaling.E;
        public WeaponScaling dexterityScaling = WeaponScaling.E;
        public WeaponScaling intelligenceScalling = WeaponScaling.E;
        #endregion

        #region Stamina
        [Header("Stamina")]
        public float lightAttackStaminaCost = 10f;
        public float heavyAttackStaminaCost = 30f;
        #endregion

        #region Events
        [Header("Events")]
        public UnityEvent OnEquip;
        public UnityEvent OnUnequip;
        #endregion

        #region  Components
        [Header("Components")]
        OneHandPivots oneHandPivots => GetComponent<OneHandPivots>();
        TwoHandPivots twoHandPivots => GetComponent<TwoHandPivots>();
        WeaponRenderer weaponRenderer => GetComponent<WeaponRenderer>();
        WeaponAnimations weaponAnimations => GetComponent<WeaponAnimations>();
        WeaponHitbox weaponHitbox => GetComponent<WeaponHitbox>();
        #endregion

        #region Private
        Dictionary<Transform, Weapon> instances = new();

        CharacterApi _characterApi;
        #endregion

        private void OnEnable()
        {
            GetCharacterApi();
        }

        CharacterApi GetCharacterApi()
        {
            if (_characterApi == null)
                _characterApi = GetComponentInParent<CharacterApi>();

            return _characterApi;
        }

        public void Equip(Transform parent, bool isRightHand)
        {
            instances[parent] = Instantiate(this, parent);
            instances[parent].name += " " + (isRightHand ? "(Right)" : "(Left)");

            ApplyPivots(instances[parent], isRightHand);
            ApplyAnimations(instances[parent]);

            instances[parent].weaponRenderer.EnableRenderer();

            DisableHitbox();

            instances[parent].OnEquip.Invoke();
        }

        public void Unequip(Transform parent)
        {
            if (instances.ContainsKey(parent) && instances[parent] != null)
            {
                instances[parent].OnUnequip.Invoke();
                Destroy(instances[parent].gameObject);
            }
        }

        #region Pivots
        public void ApplyPivots(Weapon instance, bool isRightHand)
        {
            if (GetCharacterApi().characterEquipment.characterWeapons.IsTwoHanding)
            {
                instance.transform.localPosition = twoHandPivots.rightHandPivot;
                instance.transform.localEulerAngles = twoHandPivots.rightHandRotation;
                return;
            }

            instance.transform.localPosition = isRightHand ? oneHandPivots.rightHandPivot : oneHandPivots.leftHandPivot;
            instance.transform.localEulerAngles = isRightHand ? oneHandPivots.rightHandRotation : oneHandPivots.leftHandRotation;
        }
        #endregion

        #region Hitboxes
        public void EnableHitbox() => weaponHitbox.EnableHitbox();
        public void DisableHitbox() => weaponHitbox.DisableHitbox();
        #endregion

        #region Attack Animations
        public void ApplyAnimations(Weapon instance) => instance.weaponAnimations.ApplyAnimations();

        public List<string> GetAttacksForCombatDecision(CombatDecision combatDecision)
        {
            if (combatDecision == CombatDecision.RIGHT_AIR_ATTACK)
                return weaponAnimations.RightAirAttacks;
            if (combatDecision == CombatDecision.LEFT_AIR_ATTACK)
                return weaponAnimations.LeftAirAttacks;
            if (combatDecision == CombatDecision.RIGHT_LIGHT_ATTACK)
                return weaponAnimations.RightLightAttacks;
            if (combatDecision == CombatDecision.LEFT_LIGHT_ATTACK)
                return weaponAnimations.LeftLightAttacks;
            if (combatDecision == CombatDecision.HEAVY_ATTACK)
                return weaponAnimations.HeavyAttacks;
            return new();
        }
        #endregion
    }
}
