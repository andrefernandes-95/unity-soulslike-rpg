namespace AFV2
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Weapon", menuName = "Items/ New Weapon")]
    public class Weapon : Item
    {
        #region Weapon Type
        public WeaponType weaponType;
        #endregion

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

        #region Damage
        [Header("💥 Damage")]
        public Damage damage;
        #endregion

        [Header("⚙️ Options")]
        public bool isFallbackWeapon = false;
    }
}
