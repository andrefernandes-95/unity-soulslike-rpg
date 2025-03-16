namespace AFV2
{
    using UnityEngine;

    public class WeaponQuickItem : QuickItem
    {
        [SerializeField] Sprite unequippedWeaponIcon;

        void Awake()
        {
            characterApi.characterWeapons.onRightWeaponSwitched.AddListener(OnRightWeaponSwitched);
        }

        void OnRightWeaponSwitched(WeaponInstance weaponInstance)
        {
            HideItemCount();

            if (weaponInstance.item is Weapon weapon && weapon.isFallbackWeapon == false)
            {
                UpdateIcon(weaponInstance.item.Sprite);
            }
            else
            {
                UpdateIcon(unequippedWeaponIcon);
            }
        }
    }
}
