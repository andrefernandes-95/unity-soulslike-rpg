namespace AF
{
    using AF.Health;
    using AYellowpaper.SerializedCollections;
    using UnityEngine;

    public class CharacterWeaponBuffs : MonoBehaviour
    {

        public SerializedDictionary<WeaponBuffType, WeaponBuff> weaponBuffs = new();

        WeaponBuffType currentAppliedBuff = WeaponBuffType.NONE;

        private void Awake()
        {
            GetBuffs();

            DisableAllBuffContainers();
        }

        void GetBuffs()
        {
            foreach (WeaponBuff weaponBuff in transform.GetComponentsInChildren<WeaponBuff>())
            {
                if (!weaponBuffs.ContainsKey(weaponBuff.weaponBuffType))
                {
                    weaponBuffs.Add(weaponBuff.weaponBuffType, weaponBuff);
                }
            }
        }

        void DisableAllBuffContainers()
        {
            foreach (var weaponBuff in weaponBuffs)
            {
                weaponBuff.Value.gameObject.SetActive(false);
            }
        }


        public void ApplyBuff(WeaponBuffType weaponBuffType)
        {
            if (!CanUseBuff(weaponBuffType))
            {
                return;
            }

            currentAppliedBuff = weaponBuffType;

            weaponBuffs[weaponBuffType].gameObject.SetActive(true);

            Invoke(nameof(DisableBuff), weaponBuffs[weaponBuffType].appliedDuration);
        }

        void DisableBuff()
        {
            DisableAllBuffContainers();

            currentAppliedBuff = WeaponBuffType.NONE;
        }

        bool CanUseBuff(WeaponBuffType weaponBuffType)
        {
            if (HasOnGoingBuff())
            {
                return false;
            }

            if (!weaponBuffs.ContainsKey(weaponBuffType))
            {
                return false;
            }

            return true;
        }

        public bool HasOnGoingBuff()
        {
            return currentAppliedBuff != WeaponBuffType.NONE;
        }

        public Damage CombineBuffDamage(Damage baseDamage)
        {
            return baseDamage.Combine(weaponBuffs[currentAppliedBuff].damageModifier);
        }
    }
}
