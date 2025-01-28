using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AF.Health
{

    [System.Serializable]
    public class StatusEffectEntry
    {
        public StatusEffect statusEffect;
        public float amountPerHit;
    }

    [System.Serializable]
    public enum DamageType
    {
        NORMAL,
        COUNTER_ATTACK,
        ENRAGED,
    }

    [System.Serializable]
    public class Damage
    {
        public int physical;
        public int fire;
        public int frost;
        public int magic;
        public int lightning;
        public int darkness;
        public int water;
        public int postureDamage;
        public int poiseDamage;
        public float pushForce = 0;

        public WeaponAttackType weaponAttackType;

        public StatusEffectEntry[] statusEffects;

        public bool ignoreBlocking = false;
        public bool canNotBeParried = false;
        public DamageType damageType = DamageType.NORMAL;

        public Damage()
        {
        }

        public Damage(
            int physical,
            int fire,
            int frost,
            int magic,
            int lightning,
            int darkness,
            int water,
            int postureDamage,
            int poiseDamage,
            WeaponAttackType weaponAttackType,
            StatusEffectEntry[] statusEffects,
            float pushForce,
            bool ignoreBlocking,
            bool canNotBeParried)
        {
            this.physical = physical;
            this.fire = fire;
            this.frost = frost;
            this.magic = magic;
            this.lightning = lightning;
            this.darkness = darkness;
            this.water = water;
            this.postureDamage = postureDamage;
            this.poiseDamage = poiseDamage;
            this.weaponAttackType = weaponAttackType;
            this.statusEffects = statusEffects;
            this.pushForce = pushForce;
            this.ignoreBlocking = ignoreBlocking;
            this.canNotBeParried = canNotBeParried;
        }

        public int GetTotalDamage()
        {
            return physical + fire + frost + magic + lightning + darkness + water;
        }

        public Damage ScaleSpell(
            AttackStatManager attackStatManager,
            float multiplier)
        {

            WeaponInstance currentWeaponInstance = attackStatManager.equipmentDatabase.GetCurrentWeapon();

            Damage currentWeaponDamage = currentWeaponInstance.GetItem()
                .weaponDamage.GetCurrentDamage(
                attackStatManager.playerManager,
                attackStatManager.playerManager.statsBonusController.GetCurrentStrength(),
                attackStatManager.playerManager.statsBonusController.GetCurrentDexterity(),
                attackStatManager.playerManager.statsBonusController.GetCurrentIntelligence(),
                currentWeaponInstance
            );

            var damage = this.Clone().ApplyMultiplier(multiplier);

            return ScaleWithMagicStaff(damage, currentWeaponDamage);
        }

        Damage ScaleWithMagicStaff(Damage currentDamage, Damage magicStaffDamage)
        {
            Damage damage = currentDamage.Clone();

            if (damage.fire > 0)
            {
                damage.fire += magicStaffDamage.magic;
            }

            if (damage.frost > 0)
            {
                damage.frost += magicStaffDamage.magic;
            }

            if (damage.magic > 0)
            {
                damage.magic += magicStaffDamage.magic;
            }

            if (damage.lightning > 0)
            {
                damage.lightning += magicStaffDamage.magic;
            }

            if (damage.darkness > 0)
            {
                damage.darkness += magicStaffDamage.magic;
            }

            if (damage.water > 0)
            {
                damage.water += magicStaffDamage.magic;
            }

            if (damage.statusEffects != null && damage.statusEffects.Length > 0)
            {
                List<StatusEffectEntry> updatedStatusEffects = new();

                // First, copy all existing status effects
                foreach (StatusEffectEntry existingEffect in damage.statusEffects)
                {
                    updatedStatusEffects.Add(new StatusEffectEntry() { amountPerHit = existingEffect.amountPerHit, statusEffect = existingEffect.statusEffect });
                }

                // Then, combine with weapon status effects
                foreach (StatusEffectEntry weaponStatusEffectEntry in magicStaffDamage.statusEffects)
                {
                    StatusEffectEntry existingEffect = updatedStatusEffects.Find(x => x.statusEffect == weaponStatusEffectEntry.statusEffect);

                    if (existingEffect != null)
                    {
                        // Create a new entry with combined amount
                        int index = updatedStatusEffects.IndexOf(existingEffect);
                        updatedStatusEffects[index] = new StatusEffectEntry()
                        {
                            statusEffect = existingEffect.statusEffect,
                            amountPerHit = existingEffect.amountPerHit + weaponStatusEffectEntry.amountPerHit
                        };
                    }
                    else
                    {
                        // Add a new copy of the weapon status effect
                        updatedStatusEffects.Add(new StatusEffectEntry()
                        {
                            amountPerHit = weaponStatusEffectEntry.amountPerHit,
                            statusEffect = weaponStatusEffectEntry.statusEffect
                        });
                    }
                }

                damage.statusEffects = updatedStatusEffects.ToArray();
            }

            return damage;
        }

        public Damage ScaleProjectile(AttackStatManager attackStatManager, WeaponInstance currentWeaponInstance)
        {
            Weapon currentWeapon = currentWeaponInstance.GetItem();

            Damage currentWeaponDamage = currentWeapon.weaponDamage.GetCurrentDamage(
                attackStatManager.playerManager,
                attackStatManager.playerManager.statsBonusController.GetCurrentStrength(),
                attackStatManager.playerManager.statsBonusController.GetCurrentDexterity(),
                attackStatManager.playerManager.statsBonusController.GetCurrentIntelligence(),
                currentWeaponInstance
            );

            float projectileMultiplierBonus = attackStatManager.playerManager.statsBonusController.projectileMultiplierBonus;
            float multiplier = projectileMultiplierBonus > 0
                ? projectileMultiplierBonus : 1f;

            return this.Clone().ApplyMultiplier(multiplier).Combine(currentWeaponDamage);
        }


        public Damage Clone()
        {
            return (Damage)this.MemberwiseClone();
        }

        public void ScaleDamageForNewGamePlus(GameSession gameSession)
        {
            this.physical = Utils.ScaleWithCurrentNewGameIteration(this.physical, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.fire = Utils.ScaleWithCurrentNewGameIteration(this.fire, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.frost = Utils.ScaleWithCurrentNewGameIteration(this.frost, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.lightning = Utils.ScaleWithCurrentNewGameIteration(this.lightning, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.magic = Utils.ScaleWithCurrentNewGameIteration(this.magic, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.darkness = Utils.ScaleWithCurrentNewGameIteration(this.darkness, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.water = Utils.ScaleWithCurrentNewGameIteration(this.water, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.poiseDamage = Utils.ScaleWithCurrentNewGameIteration(this.poiseDamage, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.postureDamage = Utils.ScaleWithCurrentNewGameIteration(this.postureDamage, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
        }

        public Damage Copy()
        {
            Damage newDamage = new()
            {
                physical = this.physical,
                fire = this.fire,
                frost = this.frost,
                lightning = this.lightning,
                magic = this.magic,
                darkness = this.darkness,
                water = this.water,
                canNotBeParried = this.canNotBeParried,
                damageType = this.damageType,
                ignoreBlocking = this.ignoreBlocking,
                poiseDamage = this.poiseDamage,
                postureDamage = this.postureDamage,
                pushForce = this.pushForce,
                weaponAttackType = this.weaponAttackType,
                statusEffects = this.statusEffects
            };

            return newDamage;
        }

        public Damage Combine(Damage damageToCombine)
        {
            List<StatusEffectEntry> statusEffectsCombined = this.statusEffects.ToList();

            foreach (StatusEffectEntry s in damageToCombine.statusEffects)
            {
                int idx = statusEffectsCombined.FindIndex(x => x == s);
                if (idx != -1)
                {
                    statusEffectsCombined[idx].amountPerHit += s.amountPerHit;
                }
                else
                {
                    statusEffectsCombined.Add(s);
                }
            }


            Damage newDamage = new()
            {
                physical = this.physical + damageToCombine.physical,
                fire = this.fire + damageToCombine.fire,
                frost = this.frost + damageToCombine.frost,
                lightning = this.lightning + damageToCombine.lightning,
                magic = this.magic + damageToCombine.magic,
                darkness = this.darkness + damageToCombine.darkness,
                water = this.water + damageToCombine.water,
                canNotBeParried = damageToCombine.canNotBeParried,
                damageType = damageToCombine.damageType,
                ignoreBlocking = damageToCombine.ignoreBlocking,
                poiseDamage = this.poiseDamage + damageToCombine.poiseDamage,
                postureDamage = this.postureDamage + damageToCombine.postureDamage,
                pushForce = this.pushForce + damageToCombine.pushForce,
                weaponAttackType = damageToCombine.weaponAttackType,
                statusEffects = statusEffectsCombined.ToArray(),
            };

            return newDamage;
        }

        public Damage ApplyMultiplier(float multiplier)
        {
            // Clone and modify the status effects with the multiplier
            List<StatusEffectEntry> statusEffectsCombined = new List<StatusEffectEntry>();

            foreach (StatusEffectEntry statusEffectEntry in this.statusEffects)
            {
                statusEffectEntry.amountPerHit = (int)(statusEffectEntry.amountPerHit * multiplier);

                statusEffectsCombined.Add(statusEffectEntry);
            }

            // Create the new Damage object with all values scaled by the multiplier
            Damage newDamage = new()
            {
                physical = (int)(this.physical * multiplier),
                fire = (int)(this.fire * multiplier),
                frost = (int)(this.frost * multiplier),
                lightning = (int)(this.lightning * multiplier),
                magic = (int)(this.magic * multiplier),
                darkness = (int)(this.darkness * multiplier),
                water = (int)(this.water * multiplier),
                canNotBeParried = this.canNotBeParried,
                damageType = this.damageType,
                ignoreBlocking = this.ignoreBlocking,
                poiseDamage = (int)(this.poiseDamage * multiplier),
                postureDamage = (int)(this.postureDamage * multiplier),
                pushForce = (int)(this.pushForce * multiplier),
                weaponAttackType = this.weaponAttackType,
                statusEffects = statusEffectsCombined.ToArray(),
            };

            return newDamage;
        }

    }
}
