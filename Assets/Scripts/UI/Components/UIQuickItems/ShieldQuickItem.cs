namespace AFV2
{
    using UnityEngine;

    public class ShieldQuickItem : QuickItem
    {
        [SerializeField] Sprite unequippedShieldIcon;

        new void Awake()
        {
            base.Awake();

            characterApi.characterWeapons.onLeftWeaponSwitched.AddListener(OnLeftWeaponSwitched);
        }

        void OnLeftWeaponSwitched(WeaponInstance weaponInstance)
        {
            HideItemCount();

            if (weaponInstance.item is Weapon weapon && weapon.isFallbackWeapon == false)
            {
                UpdateIcon(weaponInstance.item.Sprite);
            }
            else
            {
                UpdateIcon(unequippedShieldIcon);
            }
        }
    }
}
