namespace AF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AF.Health;
    using UnityEngine;
    using UnityEngine.Localization.Settings;

    [CreateAssetMenu(menuName = "Items / Weapon / New Weapon Damage")]
    public class WeaponDamage : ScriptableObject
    {
        [Header("Upgrades")]
        public WeaponUpgradeLevel[] weaponUpgradeLevels;

        public WeaponUpgradeLevel GetWeaponUpgradeLevel(int level)
        {
            if (weaponUpgradeLevels == null || weaponUpgradeLevels.Length <= 0)
            {
                throw new Exception($"No weapon level assigned to weapon damage {this.name}");
            }

            return weaponUpgradeLevels[level];
        }

        Damage GetCurrentDamage(WeaponInstance weaponInstance, Gemstone[] attachedGemstones)
        {
            Damage levelDamage = GetWeaponUpgradeLevel(weaponInstance.level)?.damage;

            return EnhanceWithGemstonesDamage(levelDamage, attachedGemstones);
        }

        public Damage EnhanceWithGemstonesDamage(Damage currentDamage, Gemstone[] attachedGemstones)
        {
            if (attachedGemstones.Length <= 0)
            {
                return currentDamage;
            }

            Damage newDamage = currentDamage.Copy();

            foreach (Gemstone gemstone in attachedGemstones)
            {
                newDamage = newDamage.Combine(gemstone.damageModifier);
            }

            return newDamage;
        }

        public Damage GetCurrentDamage(
            PlayerManager playerManager,
            // Must be passed as parameters because we might be previewing raised stats from the level up screen
            int playerStrength,
            int playerDexterity,
            int playerIntelligence,
            WeaponInstance currentWeaponInstance,
            Gemstone[] attachedGemstones)
        {
            Weapon currentWeapon = currentWeaponInstance.GetItem();

            Damage currentDamage = GetCurrentDamage(currentWeaponInstance, attachedGemstones);

            return new(
                   physical: GetWeaponAttack(WeaponElementType.Physical, playerManager, playerStrength, playerDexterity, playerIntelligence, currentWeaponInstance),
                   fire: GetWeaponAttack(WeaponElementType.Fire, playerManager, playerStrength, playerDexterity, playerIntelligence, currentWeaponInstance),
                   frost: GetWeaponAttack(WeaponElementType.Frost, playerManager, playerStrength, playerDexterity, playerIntelligence, currentWeaponInstance),
                   magic: GetWeaponAttack(WeaponElementType.Magic, playerManager, playerStrength, playerDexterity, playerIntelligence, currentWeaponInstance),
                   lightning: GetWeaponAttack(WeaponElementType.Lightning, playerManager, playerStrength, playerDexterity, playerIntelligence, currentWeaponInstance),
                   darkness: GetWeaponAttack(WeaponElementType.Darkness, playerManager, playerStrength, playerDexterity, playerIntelligence, currentWeaponInstance),
                   water: GetWeaponAttack(WeaponElementType.Water, playerManager, playerStrength, playerDexterity, playerIntelligence, currentWeaponInstance),
                   postureDamage: GetWeaponPostureDamage(playerManager, currentWeaponInstance),
                   poiseDamage: GetWeaponPoiseDamage(playerManager, currentWeaponInstance),
                   weaponAttackType: currentDamage.weaponAttackType,
                   statusEffects: GetWeaponStatusEffectsDamage(playerManager, currentWeaponInstance),
                   pushForce: GetWeaponPushForceDamage(playerManager, currentWeaponInstance),
                   canNotBeParried: currentDamage.canNotBeParried,
                   ignoreBlocking: currentDamage.ignoreBlocking
               );
        }

        int GetWeaponPostureDamage(PlayerManager playerManager, WeaponInstance weaponInstance)
        {
            int currentPostureDamage = GetCurrentDamage(weaponInstance, playerManager.gemstonesDatabase.GetAttachedGemstonesFromWeapon(weaponInstance)).postureDamage;

            if (playerManager.playerCombatController.isHeavyAttacking)
            {
                return (int)(currentPostureDamage * CombatSettings.twoHandingMultiplier);
            }

            if (playerManager.playerCombatController.isJumpAttacking)
            {
                return (int)(currentPostureDamage * CombatSettings.jumpAttackMultiplier);
            }

            return currentPostureDamage;
        }

        int GetWeaponPoiseDamage(PlayerManager playerManager, WeaponInstance weaponInstance)
        {
            int currentPoiseDamage = GetCurrentDamage(weaponInstance, playerManager.gemstonesDatabase.GetAttachedGemstonesFromWeapon(weaponInstance)).poiseDamage;

            if (playerManager.playerCombatController.isHeavyAttacking)
            {
                return (int)(currentPoiseDamage * CombatSettings.twoHandingMultiplier);
            }

            if (playerManager.playerCombatController.isJumpAttacking)
            {
                return (int)(currentPoiseDamage * CombatSettings.jumpAttackMultiplier);
            }

            return currentPoiseDamage;
        }

        float GetWeaponPushForceDamage(PlayerManager playerManager, WeaponInstance weaponInstance)
        {
            float currentPushForceDamage = GetCurrentDamage(weaponInstance, playerManager.gemstonesDatabase.GetAttachedGemstonesFromWeapon(weaponInstance)).pushForce;

            if (playerManager.playerCombatController.isHeavyAttacking)
            {
                return currentPushForceDamage * CombatSettings.heavyAttackMultiplier;
            }

            if (playerManager.playerCombatController.isJumpAttacking)
            {
                return currentPushForceDamage * CombatSettings.jumpAttackMultiplier;
            }

            return currentPushForceDamage;
        }

        StatusEffectEntry[] GetWeaponStatusEffectsDamage(PlayerManager playerManager, WeaponInstance weaponInstance)
        {
            List<StatusEffectEntry> effects = new();

            Damage currentDamage = GetCurrentDamage(weaponInstance, playerManager.gemstonesDatabase.GetAttachedGemstonesFromWeapon(weaponInstance));
            if (currentDamage == null)
            {
                return new List<StatusEffectEntry>().ToArray();
            }

            foreach (StatusEffectEntry status in currentDamage.statusEffects)
            {
                int newAmountPerHit = (int)status.amountPerHit;

                if (playerManager.playerCombatController.isHeavyAttacking)
                {
                    newAmountPerHit = (int)(newAmountPerHit * CombatSettings.heavyAttackMultiplier);
                }

                if (playerManager.playerCombatController.isJumpAttacking)
                {
                    newAmountPerHit = (int)(newAmountPerHit * CombatSettings.jumpAttackMultiplier);
                }

                effects.Add(new StatusEffectEntry() { statusEffect = status.statusEffect, amountPerHit = newAmountPerHit });
            }

            return effects.ToArray();
        }

        public int GetStrengthScalingBonus(int playerStrength, Weapon currentWeapon)
            => (int)WeaponScalingTable.GetScalingBonus(AttributeType.STRENGTH, currentWeapon.strengthScaling, playerStrength);

        public int GetDexterityScalingBonus(int playerDexterity, Weapon currentWeapon)
            => (int)WeaponScalingTable.GetScalingBonus(AttributeType.DEXTERITY, currentWeapon.dexterityScaling, playerDexterity);

        public int GetIntelligenceScalingBonus(int playerIntelligence, Weapon currentWeapon)
            => (int)WeaponScalingTable.GetScalingBonus(AttributeType.INTELLIGENCE, currentWeapon.intelligenceScaling, playerIntelligence);

        int GetScalingBonus(int playerStrength, int playerDexterity, int playerIntelligence, Weapon currentWeapon)
            => GetStrengthScalingBonus(playerStrength, currentWeapon) + GetDexterityScalingBonus(playerDexterity, currentWeapon) + GetIntelligenceScalingBonus(playerIntelligence, currentWeapon);

        public int GetWeaponAttack(
            WeaponElementType element, PlayerManager playerManager,
            int playerStrength, int playerDexterity, int playerIntelligence, WeaponInstance currentWeaponInstance)
        {
            int elementAttack = 0;

            Weapon currentWeapon = currentWeaponInstance.GetItem();

            Damage currentDamage = GetCurrentDamage(currentWeaponInstance, playerManager.gemstonesDatabase.GetAttachedGemstonesFromWeapon(currentWeaponInstance));

            if (element == WeaponElementType.Physical) elementAttack = currentDamage.physical;
            if (element == WeaponElementType.Fire) elementAttack = currentDamage.fire;
            if (element == WeaponElementType.Frost) elementAttack = currentDamage.frost;
            if (element == WeaponElementType.Magic) elementAttack = currentDamage.magic;
            if (element == WeaponElementType.Lightning) elementAttack = currentDamage.lightning;
            if (element == WeaponElementType.Darkness) elementAttack = currentDamage.darkness;
            if (element == WeaponElementType.Water) elementAttack = currentDamage.water;

            if (elementAttack <= 0)
            {
                return 0;
            }

            return (int)(
                (elementAttack
                    + GetScalingBonus(playerStrength, playerDexterity, playerIntelligence, currentWeapon)
                    + GetFaithBonuses(playerManager, currentDamage)
                    + GetAttackBonuses(playerManager)
                ) * GetAttackMultipliers(playerManager));
        }

        int GetFaithBonuses(PlayerManager playerManager, Damage currentDamage)
        {
            int currentReputation = playerManager.statsBonusController.GetCurrentReputation();

            if (currentDamage.lightning > 0 || currentDamage.darkness > 0)
            {
                return (int)Math.Min(100, Mathf.Pow(Mathf.Abs(currentReputation), 1.25f)); ;
            }

            return 0;
        }

        public string GetFormattedStatusDamages(PlayerManager playerManager, WeaponInstance currentWeaponInstance)
        {
            string result = "";

            foreach (var statusEffect in GetWeaponStatusEffectsDamage(playerManager, currentWeaponInstance))
            {
                if (statusEffect != null)
                {
                    result += $"+{statusEffect.amountPerHit} {statusEffect.statusEffect.GetName()} {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Inflicted per Hit")}\n";
                }
            }

            return result.TrimEnd();
        }

        private int GetAttackBonuses(PlayerManager playerManager)
        {
            int bonusValue = 0;

            // Reputation-based attack bonus
            if (HasAccessory(playerManager, x => x.increaseAttackPowerTheLowerTheReputation))
            {
                int reputation = playerManager.statsBonusController.GetCurrentReputation();
                if (reputation < 0)
                {
                    int extraAttackPower = Mathf.Min(CombatSettings.maxReputationAttackBonus, (int)(Mathf.Abs(reputation) * CombatSettings.reputationMultiplier));
                    bonusValue += extraAttackPower;
                }
            }

            // Health-based attack bonus
            if (HasAccessory(playerManager, x => x.increaseAttackPowerWithLowerHealth))
            {
                float healthMultiplier = (playerManager.health as PlayerHealth)?.GetExtraAttackBasedOnCurrentHealth() ?? 0;
                int extraAttackPower = (int)(bonusValue * healthMultiplier);
                bonusValue += extraAttackPower;
            }

            bonusValue += playerManager.equipmentDatabase.accessories
                .Sum(accessory => accessory.GetItem()?.physicalAttackBonus ?? 0);

            // Guard counter and parry bonuses
            if (playerManager.characterBlockController.IsWithinCounterAttackWindow())
            {
                float counterMultiplier = playerManager.characterBlockController.counterAttackMultiplier;
                bonusValue = (int)(bonusValue * counterMultiplier);
            }

            return bonusValue;
        }

        private bool HasAccessory(PlayerManager playerManager, Func<Accessory, bool> condition) =>
            playerManager.equipmentDatabase.accessories.Any(accessory => accessory.Exists() && condition(accessory.GetItem()));

        private float GetWeaponTypeBonus(WeaponInstance weaponInstance, PlayerManager playerManager) =>
            weaponInstance.GetItem().weaponDamage
                .GetCurrentDamage(weaponInstance, playerManager.gemstonesDatabase.GetAttachedGemstonesFromWeapon(weaponInstance)).weaponAttackType switch
            {
                WeaponAttackType.Pierce => playerManager.statsBonusController.pierceDamageMultiplier,
                WeaponAttackType.Slash => playerManager.statsBonusController.slashDamageMultiplier,
                WeaponAttackType.Blunt => playerManager.statsBonusController.bluntDamageMultiplier,
                _ => 0
            };

        private float GetAttackMultipliers(PlayerManager playerManager)
        {
            // Multipliers based on weapon and state
            float attackMultiplier = 1;

            if (playerManager.equipmentDatabase.isTwoHanding)
            {
                attackMultiplier += CombatSettings.twoHandingMultiplier + playerManager.statsBonusController.twoHandAttackBonusMultiplier;
            }

            if (playerManager.playerCombatController.isJumpAttacking)
            {
                attackMultiplier += CombatSettings.jumpAttackMultiplier;
            }

            if (playerManager.playerCombatController.isHeavyAttacking)
            {
                attackMultiplier += CombatSettings.heavyAttackMultiplier;
            }

            Weapon currentWeapon = playerManager.equipmentDatabase.GetCurrentWeapon().GetItem();

            if (currentWeapon == null)
            {
                if (playerManager.statsBonusController.increaseAttackPowerWhenUnarmed)
                {
                    attackMultiplier += CombatSettings.unarmedAttackMultiplier;
                }
            }
            else
            {
                attackMultiplier += GetWeaponTypeBonus(playerManager.equipmentDatabase.GetCurrentWeapon(), playerManager);
            }

            return attackMultiplier;
        }

        public bool HasHolyDamage(WeaponInstance weaponInstance, PlayerManager playerManager) =>
            GetCurrentDamage(weaponInstance, playerManager.gemstonesDatabase.GetAttachedGemstonesFromWeapon(weaponInstance)).lightning != 0;
        public bool HasHexDamage(WeaponInstance weaponInstance, PlayerManager playerManager) =>
            GetCurrentDamage(weaponInstance, playerManager.gemstonesDatabase.GetAttachedGemstonesFromWeapon(weaponInstance)).darkness != 0;

        public bool IsRangeWeapon(WeaponInstance weaponInstance, PlayerManager playerManager) =>
            GetCurrentDamage(weaponInstance, playerManager.gemstonesDatabase.GetAttachedGemstonesFromWeapon(weaponInstance)).weaponAttackType == WeaponAttackType.Range;
        public bool IsMagicStave(WeaponInstance weaponInstance, PlayerManager playerManager) =>
            GetCurrentDamage(weaponInstance, playerManager.gemstonesDatabase.GetAttachedGemstonesFromWeapon(weaponInstance)).weaponAttackType == WeaponAttackType.Staff;

    }
}
