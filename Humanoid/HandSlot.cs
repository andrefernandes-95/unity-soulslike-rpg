namespace AF
{
    using System.Collections.Generic;
    using UnityEngine;

    public class HandSlot : MonoBehaviour
    {
        public Dictionary<Weapon, WeaponHitbox> availableWeapons;
        private Hitbox activeWeapon;
        private UnarmedHitbox unarmedHitbox;

        void Awake()
        {
            CollectUnarmedHitbox();
            CollectWeaponsFromChildren();
        }

        void CollectUnarmedHitbox()
        {
            unarmedHitbox = transform.GetComponentInChildren<UnarmedHitbox>();
        }

        void CollectWeaponsFromChildren()
        {
            foreach (WeaponHitbox characterWeaponHitbox in transform.GetComponentsInChildren<WeaponHitbox>())
            {
                if (characterWeaponHitbox == null || characterWeaponHitbox.weapon == null) continue;

                if (!availableWeapons.ContainsKey(characterWeaponHitbox.weapon))
                {
                    availableWeapons.Add(characterWeaponHitbox.weapon, characterWeaponHitbox);
                }
            }
        }


        public void Equip(Weapon weapon)
        {
            activeWeapon = availableWeapons[weapon];
            activeWeapon.gameObject.SetActive(true);
        }

        public void Unequip()
        {
            if (activeWeapon != null)
            {
                activeWeapon.gameObject.SetActive(false);
                activeWeapon = null;
            }

            EquipUnarmedWeapon();
        }


        void EquipUnarmedWeapon()
        {
            if (unarmedHitbox == null) return;

            unarmedHitbox.gameObject.SetActive(true);
        }

        void HideUnarmedWeapon()
        {
            if (unarmedHitbox == null) return;

            unarmedHitbox.gameObject.SetActive(true);
        }
    }
}
