namespace AFV2
{
    using AF;
    using UnityEngine;

    public class Weapon : Item
    {
        [Header("Scaling")]
        public WeaponScaling strengthScaling = WeaponScaling.E;
        public WeaponScaling dexterityScaling = WeaponScaling.E;
        public WeaponScaling intelligenceScalling = WeaponScaling.E;



    }
}
