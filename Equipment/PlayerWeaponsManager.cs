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
        public CharacterWeaponHitbox leftHandHitbox;
        public CharacterWeaponHitbox rightHandHitbox;
        public CharacterWeaponHitbox leftFootHitbox;
        public CharacterWeaponHitbox rightFootHitbox;

        [Header("Weapon References In-World")]
        public List<CharacterWeaponHitbox> weaponInstances;
        public List<CharacterWeaponHitbox> secondaryWeaponInstances;
        public List<ShieldWorldInstance> shieldInstances;
        public List<HolsteredWeapon> holsteredWeapons;

        [Header("Current Weapon")]
        public CharacterWeaponHitbox currentWeaponWorldInstance;
        public CharacterWeaponHitbox currentSecondaryWeaponInstance;
        public ShieldWorldInstance currentShieldWorldInstance;

        [Header("Dual Wielding")]
        public CharacterWeaponHitbox secondaryWeaponInstance;

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

            List<CharacterWeaponHitbox> weaponsList = new List<CharacterWeaponHitbox>();
            weaponsList.AddRange(weaponInstances);
            weaponsList.AddRange(secondaryWeaponInstances);

            foreach (CharacterWeaponHitbox weaponHitbox in weaponsList)
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
            currentWeaponWorldInstance?.ShowWeapon();
            currentSecondaryWeaponInstance?.ShowWeapon();
            currentShieldWorldInstance?.ResetStates();
        }

        public void HideEquipment()
        {
            currentWeaponWorldInstance?.HideWeapon();
            currentSecondaryWeaponInstance?.HideWeapon();
            currentShieldWorldInstance?.ShowBackShield();
        }

        public void HideShield() => currentShieldWorldInstance?.ShowBackShield();
        public void ShowShield() => currentShieldWorldInstance?.ResetStates();

        bool CanApplyBuff()
        {
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
            }

            return true;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplyFireToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.FIRE, customDuration);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplyFrostToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.FROST, customDuration);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplyLightningToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.LIGHTNING, customDuration);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplyMagicToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.MAGIC, customDuration);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplyDarknessToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.DARKNESS, customDuration);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplyPoisonToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.POISON, customDuration);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplyBloodToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.BLOOD, customDuration);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplySharpnessToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.SHARPNESS, customDuration);
        }


        public void ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName weaponBuffName, float customDuration)
        {
            if (!CanApplyBuff())
            {
                return;
            }

            if (customDuration > 0)
            {
                currentWeaponWorldInstance?.characterWeaponBuffs?.ApplyBuff(weaponBuffName, customDuration);
                secondaryWeaponInstance?.characterWeaponBuffs?.ApplyBuff(weaponBuffName, customDuration);
            }
            else
            {
                currentWeaponWorldInstance?.characterWeaponBuffs?.ApplyBuff(weaponBuffName);
                secondaryWeaponInstance?.characterWeaponBuffs?.ApplyBuff(weaponBuffName);
            }
        }

        public Damage GetBuffedDamage(Damage weaponDamage)
        {
            if (currentWeaponWorldInstance == null || currentWeaponWorldInstance.characterWeaponBuffs == null || currentWeaponWorldInstance.characterWeaponBuffs.HasOnGoingBuff() == false)
            {
                return weaponDamage;
            }

            if (currentWeaponWorldInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.FIRE)
            {
                weaponDamage.fire += currentWeaponWorldInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.FIRE].damageBonus;
            }

            if (currentWeaponWorldInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.FROST)
            {
                weaponDamage.frost += currentWeaponWorldInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.FROST].damageBonus;
            }

            if (currentWeaponWorldInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.LIGHTNING)
            {
                weaponDamage.lightning += currentWeaponWorldInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.LIGHTNING].damageBonus;
            }

            if (currentWeaponWorldInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.MAGIC)
            {
                weaponDamage.magic += currentWeaponWorldInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.MAGIC].damageBonus;
            }

            if (currentWeaponWorldInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.DARKNESS)
            {
                weaponDamage.darkness += currentWeaponWorldInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.DARKNESS].damageBonus;
            }

            if (currentWeaponWorldInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.WATER)
            {
                weaponDamage.water += currentWeaponWorldInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.WATER].damageBonus;
            }

            if (currentWeaponWorldInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.SHARPNESS)
            {
                weaponDamage.physical += currentWeaponWorldInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.SHARPNESS].damageBonus;
            }

            if (currentWeaponWorldInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.POISON)
            {
                StatusEffectEntry statusEffectToApply = new()
                {
                    statusEffect = currentWeaponWorldInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.POISON].statusEffect,
                    amountPerHit = currentWeaponWorldInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.POISON].statusEffectAmountToApply,
                };

                if (weaponDamage.statusEffects == null)
                {
                    weaponDamage.statusEffects = new StatusEffectEntry[] {
                        statusEffectToApply
                    };
                }
                else
                {
                    weaponDamage.statusEffects = weaponDamage.statusEffects.Append(statusEffectToApply).ToArray();
                }
            }

            if (currentWeaponWorldInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.BLOOD)
            {
                StatusEffectEntry statusEffectToApply = new()
                {
                    statusEffect = currentWeaponWorldInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.BLOOD].statusEffect,
                    amountPerHit = currentWeaponWorldInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.BLOOD].statusEffectAmountToApply,
                };

                if (weaponDamage.statusEffects == null)
                {
                    weaponDamage.statusEffects = new StatusEffectEntry[] {
                        statusEffectToApply
                    };
                }
                else
                {
                    weaponDamage.statusEffects = weaponDamage.statusEffects.Append(statusEffectToApply).ToArray();
                }
            }

            return weaponDamage;
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
                || playerManager.playerWeaponsManager.currentWeaponWorldInstance.onWeaponSpecial == null
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

            playerManager.playerWeaponsManager.currentWeaponWorldInstance.onWeaponSpecial?.Invoke();
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
