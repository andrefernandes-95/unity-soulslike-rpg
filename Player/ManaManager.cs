using System.Collections;
using AF.Events;
using AF.Stats;
using TigerForge;
using UnityEngine;

namespace AF
{
    public class ManaManager : MonoBehaviour
    {

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

        [Header("Components")]
        public StatsBonusController playerStatsBonusController;

        public StarterAssetsInputs inputs;

        public EquipmentGraphicsHandler equipmentGraphicsHandler;

        public PlayerManager playerManager;
        public PlayerManaUI playerManaUI;

        [Header("Regeneration Settings")]
        public float MANA_REGENERATION_RATE = 20f;

        private void Start()
        {
            // Initialize Mana
            if (playerStatsDatabase.currentMana == -1)
            {
                SetCurrentMana(GetMaxMana());
            }
        }

        private void Update()
        {
            if (playerStatsBonusController.shouldRegenerateMana && playerStatsDatabase.currentMana < playerManager.combatant.mana)
            {
                HandleManaRegen();
            }
        }

        void HandleManaRegen()
        {
            var finalRegenerationRate = MANA_REGENERATION_RATE + playerStatsBonusController.staminaRegenerationBonus;

            SetCurrentMana(Mathf.Clamp(playerStatsDatabase.currentMana + finalRegenerationRate * Time.deltaTime, 0f, GetMaxMana()));
        }

        public int GetMaxMana()
        {
            return Formulas.CalculateStatForLevel(
                 playerManager.combatant.mana + playerStatsBonusController.magicBonus,
                 playerStatsBonusController.GetCurrentIntelligence(),
                 playerStatsDatabase.levelMultiplierForMana);
        }

        public void DecreaseMana(float amount)
        {
            SetCurrentMana(Mathf.Clamp(playerStatsDatabase.currentMana - amount, 0, GetMaxMana()));
        }

        public bool HasEnoughManaForSpell(Spell spell)
        {
            if (spell == null)
            {
                return false;
            }

            return HasEnoughManaForAction((int)spell.manaCostPerCast);
        }

        public bool HasEnoughManaForAction(int actionCost)
        {
            bool canPerform = playerStatsDatabase.currentMana - actionCost > 0;
            if (!canPerform)
            {
                playerManaUI.DisplayInsufficientMana();
            }

            return canPerform;
        }

        public void RestoreFullMana()
        {
            SetCurrentMana(GetMaxMana());
        }

        public void RestoreManaPercentage(float amount)
        {
            var percentage = this.GetMaxMana() * amount / 100;
            var nextValue = Mathf.Clamp(playerStatsDatabase.currentMana + percentage, 0, this.GetMaxMana());
            SetCurrentMana(nextValue);
        }

        public void RestoreManaPoints(float amount)
        {
            var nextValue = Mathf.Clamp(playerStatsDatabase.currentMana + amount, 0, this.GetMaxMana());

            SetCurrentMana(nextValue);
        }

        public float GetManaPointsForGivenIntelligence(int intelligence)
        {
            return playerManager.combatant.mana + (int)Mathf.Ceil(intelligence * playerStatsDatabase.levelMultiplierForMana);
        }


        public float GetCurrentManaPercentage()
        {
            return playerStatsDatabase.currentMana * 100 / GetMaxMana();
        }

        void SetCurrentMana(float currentMana)
        {
            playerStatsDatabase.currentMana = currentMana;
            playerManaUI.UpdateUI();
        }
    }
}
