namespace AF
{
    using System.Collections.Generic;
    using System.Linq;
    using AF.Events;
    using AF.Health;
    using AF.Stats;
    using TigerForge;
    using UnityEngine;
    using UnityEngine.Localization;

    public class PlayerWeaponsManager : MonoBehaviour
    {
        [Header("Unarmed Weapon References In-World")]
        public WeaponHitbox leftHandHitbox;
        public WeaponHitbox rightHandHitbox;
        public WeaponHitbox leftFootHitbox;
        public WeaponHitbox rightFootHitbox;

        [Header("Weapon References In-World")]
        public List<WeaponHitbox> weaponInstances;
        public List<WeaponHitbox> secondaryWeaponInstances;
        public List<ShieldWorldInstance> shieldInstances;
        public List<HolsteredWeapon> holsteredWeapons;

        [Header("Current Weapon")]
        public WeaponHitbox currentWeaponWorldInstance;
        public WeaponHitbox currentSecondaryWeaponInstance;
        public ShieldWorldInstance currentShieldWorldInstance;

        [Header("Dual Wielding")]
        public WeaponHitbox secondaryWeaponInstance;

        [Header("Database")]
        public EquipmentDatabase equipmentDatabase;

        [Header("Components")]
        public PlayerManager playerManager;
        StatsBonusController statsBonusController;
        public NotificationManager notificationManager;

        [Header("Localization")]

        // "Can not apply buff to this weapon"
        public LocalizedString CanNotApplyBuffToThisWeapon;
        // "Weapon is already buffed"
        public LocalizedString WeaponIsAlreadyBuffed;

        private void Awake()
        {
            playerManager.damageReceiver.onDamageEvent += OnDamageEvent;

            statsBonusController = playerManager.statsBonusController;

            EventManager.StartListening(
                EventMessages.ON_EQUIPMENT_CHANGED,
                UpdateEquipment);

            EventManager.StartListening(EventMessages.ON_TWO_HANDING_CHANGED, () =>
            {
                UpdateCurrentWeapon();
                UpdateCurrentShield();
            });
        }

        private void Start()
        {
            UpdateEquipment();
        }

        void UpdateEquipment()
        {
            UpdateCurrentWeapon();
            UpdateCurrentShield();
            UpdateCurrentArrows();
            UpdateCurrentSpells();
        }

        public void ResetStates()
        {
            CloseAllWeaponHitboxes();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void CloseAllWeaponHitboxes()
        {
            currentWeaponWorldInstance?.DisableHitbox();
            currentSecondaryWeaponInstance?.DisableHitbox();
            leftFootHitbox?.DisableHitbox();
            rightFootHitbox?.DisableHitbox();
            leftHandHitbox?.DisableHitbox();
            rightHandHitbox?.DisableHitbox();
        }

        void UpdateCurrentWeapon()
        {
            var CurrentWeaponInstance = equipmentDatabase.GetCurrentWeapon();
            var SecondaryWeaponInstance = equipmentDatabase.GetCurrentSecondaryWeapon();

            if (currentWeaponWorldInstance != null) currentWeaponWorldInstance = null;
            if (currentSecondaryWeaponInstance != null) currentSecondaryWeaponInstance = null;

            List<WeaponHitbox> weaponsList = new List<WeaponHitbox>();
            weaponsList.AddRange(weaponInstances);
            weaponsList.AddRange(secondaryWeaponInstances);

            foreach (WeaponHitbox weaponHitbox in weaponsList)
            {
                weaponHitbox?.DisableHitbox();
                weaponHitbox?.gameObject.SetActive(false);
            }
            foreach (HolsteredWeapon holsteredWeapon in holsteredWeapons)
            {
                holsteredWeapon?.gameObject.SetActive(false);
            }

            if (CurrentWeaponInstance.Exists())
            {
                var gameObjectWeapon = weaponInstances.FirstOrDefault(weapon => CurrentWeaponInstance.HasItem(weapon.weapon));
                currentWeaponWorldInstance = gameObjectWeapon;

                if (currentWeaponWorldInstance != null)
                {
                    currentWeaponWorldInstance.gameObject.SetActive(true);
                }
            }

            if (SecondaryWeaponInstance.Exists())
            {
                if (equipmentDatabase.isTwoHanding)
                {
                    var holsteredWeapon = holsteredWeapons.FirstOrDefault(holsteredWeapon => SecondaryWeaponInstance.HasItem(holsteredWeapon.weapon));

                    if (holsteredWeapon != null)
                    {
                        holsteredWeapon.gameObject.SetActive(true);
                    }
                    else
                    {
                        Debug.LogError("Missing Holstered Weapon for secondary weapon: " + SecondaryWeaponInstance?.GetItem()?.name ?? "");
                    }
                }
                else
                {
                    var gameObjectWeapon = secondaryWeaponInstances.FirstOrDefault(secondaryWeapon => SecondaryWeaponInstance.HasItem(secondaryWeapon.weapon));

                    if (gameObjectWeapon != null)
                    {
                        secondaryWeaponInstance = gameObjectWeapon;
                        secondaryWeaponInstance.gameObject.SetActive(true);
                    }
                    else
                    {
                        Debug.LogError("Missing Secondary Weapon instance for secondary weapon: " + SecondaryWeaponInstance?.GetItem()?.name ?? "");
                    }
                }
            }

            playerManager.UpdateAnimatorOverrideControllerClips();

            UpdateCurrentShield();
        }

        void UpdateCurrentArrows()
        {
            if (equipmentDatabase.IsBowEquipped() == false)
            {
                return;
            }
        }

        void UpdateCurrentSpells()
        {
            if (equipmentDatabase.IsStaffEquipped() == false)
            {
                return;
            }
        }

        void UpdateCurrentShield()
        {
            ShieldInstance CurrentShieldInstance = equipmentDatabase.GetCurrentShield();

            statsBonusController.RecalculateEquipmentBonus();

            if (currentShieldWorldInstance != null)
            {
                currentShieldWorldInstance = null;
            }

            foreach (var shieldInstance in shieldInstances)
            {
                shieldInstance.HideShield();
            }

            if (CurrentShieldInstance.Exists())
            {
                var gameObjectShield = shieldInstances.FirstOrDefault(shield => CurrentShieldInstance.HasItem(shield.shield));
                currentShieldWorldInstance = gameObjectShield;
                currentShieldWorldInstance.ResetStates();
            }
        }

        public void EquipWeapon(WeaponInstance weaponToEquip, int slot)
        {
            equipmentDatabase.EquipWeapon(weaponToEquip, slot);

            UpdateCurrentWeapon();
        }

        public void UnequipWeapon(int slot)
        {
            equipmentDatabase.UnequipWeapon(slot);

            UpdateCurrentWeapon();
        }

        public void EquipSecondaryWeapon(WeaponInstance weaponToEquip, int slot)
        {
            equipmentDatabase.EquipSecondaryWeapon(weaponToEquip, slot);

            UpdateCurrentWeapon();
        }

        public void UnequipSecondaryWeapon(int slot)
        {
            equipmentDatabase.UnequipSecondaryWeapon(slot);

            UpdateCurrentWeapon();
        }

        public void EquipShield(ShieldInstance shieldToEquip, int slot)
        {
            equipmentDatabase.EquipShield(shieldToEquip, slot);

            UpdateCurrentShield();

            playerManager.statsBonusController.RecalculateEquipmentBonus();
        }

        public void UnequipShield(int slot)
        {
            equipmentDatabase.UnequipShield(slot);

            UpdateCurrentShield();

            playerManager.statsBonusController.RecalculateEquipmentBonus();
        }

        public void ShowEquipment()
        {
            //            currentWeaponWorldInstance?.ShowWeapon();
            //          currentSecondaryWeaponInstance?.ShowWeapon();
            currentShieldWorldInstance?.ResetStates();
        }

        public void HideEquipment()
        {
            //currentWeaponWorldInstance?.HideWeapon();
            //  currentSecondaryWeaponInstance?.HideWeapon();
            currentShieldWorldInstance?.ShowBackShield();
        }

        public void HideShield() => currentShieldWorldInstance?.ShowBackShield();
        public void ShowShield() => currentShieldWorldInstance?.ResetStates();

        bool CanApplyBuff()
        {/*
            if (currentWeaponWorldInstance == null || currentWeaponWorldInstance.characterWeaponBuffs == null)
            {
                notificationManager.ShowNotification(
                    CanNotApplyBuffToThisWeapon.GetLocalizedString(), notificationManager.systemError);
                return false;
            }
            else if (currentWeaponWorldInstance.characterWeaponBuffs.HasOnGoingBuff())
            {
                notificationManager.ShowNotification(
                    WeaponIsAlreadyBuffed.GetLocalizedString(), notificationManager.systemError);
                return false;
            }*/

            return true;
        }
        public void ApplyWeaponBuffToWeapon(WeaponBuffType weaponBuffType)
        {
            if (!CanApplyBuff())
            {
                return;
            }

            //            currentWeaponWorldInstance?.characterWeaponBuffs?.ApplyBuff(weaponBuffType);
            //          secondaryWeaponInstance?.characterWeaponBuffs?.ApplyBuff(weaponBuffType);
        }

        WeaponHitbox GetCharacterWeaponHitboxFromWeaponInstance(WeaponInstance weaponInstance)
        {
            if (equipmentDatabase.GetCurrentWeapon() == weaponInstance)
            {
                return currentWeaponWorldInstance;
            }

            if (equipmentDatabase.GetCurrentSecondaryWeapon() == weaponInstance)
            {
                return secondaryWeaponInstance;
            }

            return null;
        }

        public Damage GetBuffedDamage(WeaponInstance weaponInstance, Damage weaponDamage)
        {
            WeaponHitbox weaponWorldInstance = GetCharacterWeaponHitboxFromWeaponInstance(weaponInstance);
            return null;
            /*
            if (weaponWorldInstance == null
                || weaponWorldInstance.characterWeaponBuffs == null
                || weaponWorldInstance.characterWeaponBuffs.HasOnGoingBuff() == false)
            {
                return weaponDamage;
            }

            return weaponWorldInstance.characterWeaponBuffs.CombineBuffDamage(weaponDamage);*/
        }

        public int GetCurrentBlockStaminaCost()
        {
            if (playerManager.playerWeaponsManager.currentShieldWorldInstance == null)
            {
                return playerManager.characterBlockController.unarmedStaminaCostPerBlock;
            }

            return (int)playerManager.playerWeaponsManager.currentShieldWorldInstance.shield.blockStaminaCost;
        }

        public Damage GetCurrentShieldDefenseAbsorption(Damage incomingDamage)
        {
            if (equipmentDatabase.isTwoHanding && equipmentDatabase.GetCurrentWeapon().Exists())
            {
                incomingDamage.physical = (int)(incomingDamage.physical * equipmentDatabase.GetCurrentWeapon().GetItem().blockAbsorption);
                return incomingDamage;
            }
            else if (currentShieldWorldInstance == null || currentShieldWorldInstance.shield == null)
            {
                incomingDamage.physical = (int)(incomingDamage.physical * playerManager.characterBlockController.unarmedDefenseAbsorption);
                return incomingDamage;
            }

            return currentShieldWorldInstance.shield.FilterDamage(incomingDamage);
        }
        public Damage GetCurrentShieldPassiveDamageFilter(Damage incomingDamage)
        {
            if (currentShieldWorldInstance == null || currentShieldWorldInstance.shield == null)
            {
                return incomingDamage;
            }

            return currentShieldWorldInstance.shield.FilterPassiveDamage(incomingDamage);
        }

        public void ApplyShieldDamageToAttacker(CharacterManager attacker)
        {
            if (currentShieldWorldInstance == null || currentShieldWorldInstance.shield == null)
            {
                return;
            }

            currentShieldWorldInstance.shield.AttackShieldAttacker(attacker);
        }

        public void HandleWeaponSpecial()
        {
            if (
                playerManager.playerWeaponsManager.currentWeaponWorldInstance == null
                || false //playerManager.playerWeaponsManager.currentWeaponWorldInstance.onWeaponSpecial == null
                || playerManager.playerWeaponsManager.currentWeaponWorldInstance.weapon == null
                )
            {
                return;
            }

            if (playerManager.manaManager.playerStatsDatabase.currentMana < playerManager.playerWeaponsManager.currentWeaponWorldInstance.weapon.manaCostToUseWeaponSpecialAttack)
            {
                //                notificationManager.ShowNotification(NotEnoughManaToUseWeaponSpecial.GetLocalizedString());
                return;
            }

            playerManager.manaManager.DecreaseMana(
                playerManager.playerWeaponsManager.currentWeaponWorldInstance.weapon.manaCostToUseWeaponSpecialAttack
            );

            //playerManager.playerWeaponsManager.currentWeaponWorldInstance.onWeaponSpecial?.Invoke();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ThrowWeapon()
        {

        }

        public Damage OnDamageEvent(CharacterBaseManager attacker, CharacterBaseManager receiver, Damage damage)
        {
            if (damage == null)
            {
                return null;
            }

            return GetCurrentShieldPassiveDamageFilter(damage);
        }

    }
}
