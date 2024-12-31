namespace AF
{
    using AF.Stats;
    using UnityEngine;

    public class DefenseStatManager : MonoBehaviour
    {
        [Header("Physical Defense")]
        public int basePhysicalDefense = 30;
        [Tooltip("Increases with endurance level")]
        public float levelMultiplier = 3.25f;

        [Header("Status defense bonus")]
        [Tooltip("Increased by buffs like potions, or equipment like accessories")]
        public float physicalDefenseBonus = 0f;
        [Range(0, 100f)] public float physicalDefenseAbsorption = 0f;

        [Header("Components")]
        public StatsBonusController playerStatsBonusController;

        [Header("Database")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

        public bool ignoreDefense = false;

        public float GetDefenseAbsorption()
        {
            if (ignoreDefense)
            {
                return 0f;
            }

            return (int)(
                GetCurrentPhysicalDefense()
                + playerStatsBonusController.equipmentPhysicalDefense // Equipment Bonus
                + physicalDefenseBonus
                + (playerStatsBonusController.enduranceBonus * levelMultiplier)
            );
        }

        public int GetCurrentPhysicalDefense()
        {
            return (int)(this.basePhysicalDefense + playerStatsDatabase.endurance * levelMultiplier) / 2;
        }

        public int GetCurrentPhysicalDefenseForGivenEndurance(int endurance)
        {
            return (int)(this.basePhysicalDefense + ((endurance * levelMultiplier) / 2));
        }

        public float GetMaximumStatusResistanceBeforeSufferingStatusEffect(StatusEffect statusEffect)
        {
            return 1f;
        }

        public float GetMagicDefense()
        {
            return playerStatsBonusController.magicDefenseBonus;
        }

        public float GetDarknessDefense()
        {
            return playerStatsBonusController.darkDefenseBonus;
        }

        public float GetWaterDefense()
        {
            return playerStatsBonusController.waterDefenseBonus;
        }

        public float GetFireDefense()
        {
            return playerStatsBonusController.fireDefenseBonus;
        }

        public float GetFrostDefense()
        {
            return playerStatsBonusController.frostDefenseBonus;
        }

        public float GetLightningDefense()
        {
            return playerStatsBonusController.lightningDefenseBonus;
        }

        public int CompareHelmet(HelmetInstance helmetInstance)
        {
            if (equipmentDatabase.helmet.IsEmpty())
            {
                return 1;
            }

            if (helmetInstance.GetItem().physicalDefense > equipmentDatabase.helmet.GetItem().physicalDefense)
            {
                return 1;
            }

            if (equipmentDatabase.helmet.GetItem().physicalDefense == helmetInstance.GetItem().physicalDefense)
            {
                return 0;
            }

            return -1;
        }

        public int CompareArmor(ArmorInstance armorInstance)
        {
            if (equipmentDatabase.armor.IsEmpty())
            {
                return 1;
            }

            if (armorInstance.GetItem().physicalDefense > equipmentDatabase.armor.GetItem().physicalDefense)
            {
                return 1;
            }

            if (equipmentDatabase.armor.GetItem().physicalDefense == armorInstance.GetItem().physicalDefense)
            {
                return 0;
            }

            return -1;
        }

        public int CompareGauntlet(GauntletInstance gauntletInstance)
        {
            if (equipmentDatabase.gauntlet.IsEmpty())
            {
                return 1;
            }

            if (gauntletInstance.GetItem().physicalDefense > equipmentDatabase.gauntlet.GetItem().physicalDefense)
            {
                return 1;
            }

            if (equipmentDatabase.gauntlet.GetItem().physicalDefense == gauntletInstance.GetItem().physicalDefense)
            {
                return 0;
            }

            return -1;
        }

        public int CompareLegwear(Legwear legwear)
        {
            if (equipmentDatabase.legwear.IsEmpty())
            {
                return 1;
            }

            if (legwear.physicalDefense > equipmentDatabase.legwear.GetItem()?.physicalDefense)
            {
                return 1;
            }

            if (equipmentDatabase.legwear.GetItem()?.physicalDefense == legwear.physicalDefense)
            {
                return 0;
            }

            return -1;
        }

        public void SetDefenseAbsorption(int value)
        {
            physicalDefenseAbsorption = value;
        }

        public void ResetDefenseAbsorption()
        {
            physicalDefenseAbsorption = 0f;
        }
    }
}
