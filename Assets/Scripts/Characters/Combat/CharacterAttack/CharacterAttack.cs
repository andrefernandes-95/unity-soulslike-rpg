namespace AFV2
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;

    public class CharacterAttack : MonoBehaviour
    {
        [SerializeField] CharacterCombat characterCombat;
        [SerializeField] CharacterCombatDecision characterCombatDecision;
        [SerializeField] CharacterApi characterApi;

        [SerializeField][Range(0f, 1f)] float minTimeToAllowCombo = 0.65f;

        protected void FinishCombo()
        {
            characterApi.characterMovement.EnableRotation();
            characterApi.characterEquipment.characterWeapons.DisableAllHitboxes();
            characterApi.animatorManager.DisableRootMotion();
        }

        protected virtual void FinishAttack()
        {
            FinishCombo();
        }

        public async virtual Task Attack(
            List<string> availableAttacks, float staminaCost, CombatDecision combatDecision)
        {
            if (combatDecision == CombatDecision.NONE)
            {
                FinishAttack();
                return;
            }

            await RunAttacks(availableAttacks, staminaCost, combatDecision);
        }

        protected async Task<Task> RunAttacks(
            List<string> attacks,
            float staminaCost,
            CombatDecision combatDecision
        )
        {
            bool shouldExitEarly = false;
            while (shouldExitEarly == false && characterCombat.CanCombo(staminaCost, combatDecision) && attacks.Count > 0)
            {
                FinishAttack();

                string nextAttack = attacks[0];
                attacks.RemoveAt(0);

                if (characterApi.characterGravity.Grounded)
                    characterApi.animatorManager.EnableRootMotion();

                // Play the attack animation
                characterApi.animatorManager.BlendTo(nextAttack, 0.05f);

                // **Wait for animation while checking if the player lands mid-attack**
                Task animationTask = characterApi.animatorManager.WaitForAnimationToFinish(nextAttack);

                shouldExitEarly = IsAirAttackCancelled(combatDecision);
                while (!animationTask.IsCompleted)
                {
                    await Task.Yield(); // Yield control to avoid blocking

                    shouldExitEarly = IsAirAttackCancelled(combatDecision);

                    if (shouldExitEarly || ShouldJumpToNextCombo(nextAttack))
                    {
                        break;
                    }
                }

                FinishCombo();
            }

            if (shouldExitEarly)
            {
                FinishAttack();
            }

            return Task.CompletedTask;
        }



        bool IsAirCombatDecision(CombatDecision combatDecision)
        {
            return combatDecision == CombatDecision.RIGHT_AIR_ATTACK || combatDecision == CombatDecision.LEFT_AIR_ATTACK;
        }

        bool IsAirAttackCancelled(CombatDecision combatDecision)
        {
            return IsAirCombatDecision(combatDecision) && characterApi.characterGravity.Grounded;
        }

        protected virtual bool ShouldJumpToNextCombo(string currentAttack)
        {
            if (characterApi.animatorManager.GetAnimationProgress(currentAttack) >= minTimeToAllowCombo)//&&
                                                                                                        //(playerController.HasLeftAttackQueued || playerController.HasRightAttackQueued))
            {
                return true;
            }

            return false;
        }
    }
}
