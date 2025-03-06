namespace AFV2
{
    using UnityEngine;

    public class PlayerAttack : CharacterAttack
    {
        [SerializeField] PlayerController playerController;

        protected override void FinishAttack()
        {
            base.FinishAttack();

            playerController.ResetCombatFlags();
        }

        protected override bool ShouldJumpToNextCombo(string currentAttack)
        {
            bool shouldJumpToNextCombo = base.ShouldJumpToNextCombo(currentAttack);

            if (shouldJumpToNextCombo)
            {
                return playerController.HasLeftAttackQueued || playerController.HasRightAttackQueued;
            }

            return false;
        }
    }
}
