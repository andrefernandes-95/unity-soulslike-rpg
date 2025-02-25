namespace AFV2
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class Weapon : Item
    {
        #region Scaling
        [Header("Scaling")]
        //        public WeaponScaling strengthScaling = WeaponScaling.E;
        //        public WeaponScaling dexterityScaling = WeaponScaling.E;
        //        public WeaponScaling intelligenceScalling = WeaponScaling.E;
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
        WeaponAnimations weaponAnimations => GetComponent<WeaponAnimations>();
        public WeaponAnimations Animations => weaponAnimations;
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
    }
}
