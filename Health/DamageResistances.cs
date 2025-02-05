namespace AF
{
    using System;
    using AF.Health;
    using UnityEngine;

    [Serializable]
    public class DamageResistances : MonoBehaviour
    {
        public DamageReceiver damageReceiver;
        Combatant combatant;

        public float loweredDamageBonusMultiplier = 1;

        private void Awake()
        {
            damageReceiver.onDamageEvent += OnDamageEvent;
            combatant = damageReceiver.character.combatant;
        }

        public Damage OnDamageEvent(CharacterBaseManager attacker, CharacterBaseManager receiver, Damage damage)
        {
            if (damage == null)
            {
                return null;
            }

            if (combatant == null)
            {
                return damage;
            }

            return FilterIncomingDamage(damage);
        }

        float GetPhysicalDamageMultiplier(Damage incomingDamage)
        {
            float physicalMultiplier = 1f;

            if (incomingDamage.weaponAttackType == WeaponAttackType.Slash)
            {
                physicalMultiplier *= combatant.slashAbsorption;
                physicalMultiplier *= combatant.slashBonus;
            }
            else if (incomingDamage.weaponAttackType == WeaponAttackType.Blunt)
            {
                physicalMultiplier *= combatant.bluntAbsorption;
                physicalMultiplier *= combatant.bluntBonus;
            }
            else if (incomingDamage.weaponAttackType == WeaponAttackType.Pierce)
            {
                physicalMultiplier *= combatant.pierceAbsorption;
                physicalMultiplier *= combatant.pierceBonus;
            }
            else if (incomingDamage.weaponAttackType == WeaponAttackType.Range)
            {
                physicalMultiplier *= combatant.rangeWeaponsAbsorption;
                physicalMultiplier *= combatant.rangeWeaponsBonus;
            }
            else if (incomingDamage.weaponAttackType == WeaponAttackType.Staff)
            {
                physicalMultiplier *= combatant.magicSpellsAbsorption;
                physicalMultiplier *= combatant.magicSpellsBonus;
            }

            return physicalMultiplier;
        }

        public virtual Damage FilterIncomingDamage(Damage incomingDamage)
        {
            float physicalDamageMultiplier = GetPhysicalDamageMultiplier(incomingDamage);

            Damage filteredDamage = new()
            {
                physical = (int)(incomingDamage.physical * physicalDamageMultiplier),
                fire = (int)(incomingDamage.fire * combatant.fireAbsorption * combatant.fireBonus),
                frost = (int)(incomingDamage.frost * combatant.frostAbsorption * combatant.frostBonus),
                magic = (int)(incomingDamage.magic * combatant.magicAbsorption * combatant.magicBonus),
                lightning = (int)(incomingDamage.lightning * combatant.lightningAbsorption * combatant.lightningBonus),
                darkness = (int)(incomingDamage.darkness * combatant.darknessAbsorption * combatant.darknessBonus),
                water = (int)(incomingDamage.water * combatant.waterAbsorption * combatant.waterBonus),
                postureDamage = incomingDamage.postureDamage,
                poiseDamage = incomingDamage.poiseDamage,
                pushForce = incomingDamage.pushForce,
                weaponAttackType = incomingDamage.weaponAttackType,
                statusEffects = incomingDamage.statusEffects,
                damageType = incomingDamage.damageType
            };

            return filteredDamage;
        }


        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void SetLoweredDamageBonusMultiplier(float value)
        {
            this.loweredDamageBonusMultiplier = value;
        }
    }
}
