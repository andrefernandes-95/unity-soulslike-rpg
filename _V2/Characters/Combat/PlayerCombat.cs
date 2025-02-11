namespace AFV2
{
    using UnityEngine;

    public class PlayerCombat : CharacterCombat
    {
        [SerializeField] PlayerController playerController;
        [SerializeField] PlayerCombatDecision playerCombatDecision;

        void Awake()
        {
            onAttackBegin.AddListener(ResetCombatFlags);
            onNoDecisionMade.AddListener(ResetCombatFlags);
            onAttackEnd.AddListener(characterApi.characterMovement.EnableRotation);
        }

        void ResetCombatFlags()
        {
            playerController.ResetCombatFlags();
        }

        protected override bool CanCombo(float staminaCost, CombatDecision combatDecision)
        {
            if (!characterApi.characterStats.HasEnoughStamina(staminaCost))
                return false;

            if (combatDecision == CombatDecision.RIGHT_LIGHT_ATTACK)
                return playerController.HasRightAttackQueued;

            if (combatDecision == CombatDecision.LEFT_LIGHT_ATTACK)
                return playerController.HasLeftAttackQueued;

            return false;
        }
    }
}
