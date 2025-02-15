namespace AFV2
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(CharacterCombatDecision))]
    public class CharacterCombat : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] protected CharacterApi characterApi;
        [SerializeField] CharacterCombatDecision characterCombatDecision => GetComponent<CharacterCombatDecision>();
        public CharacterCombatDecision CharacterCombatDecision => characterCombatDecision;

        // Callbacks
        [HideInInspector] public UnityEvent onAttackBegin, onAttackEnd, onNoDecisionMade;

        void Awake()
        {
            onAttackEnd.AddListener(characterApi.characterEquipment.characterWeapons.DisableAllHitboxes);
        }

        public async virtual Task Attack(
            List<string> availableAttacks, float staminaCost, CombatDecision combatDecision)
        {
            if (combatDecision == CombatDecision.NONE)
            {
                onNoDecisionMade?.Invoke();
                return;
            }

            await RunAttacks(availableAttacks, staminaCost, combatDecision);
        }

        protected virtual bool CanCombo(float staminaCost, CombatDecision combatDecision)
        {
            if (!characterApi.characterStats.CharacterStamina.HasEnoughStamina(staminaCost))
                return false;

            // If target is close

            // More combos when health is high
            return Random.Range(0, 1f) <= MathHelpers.PositiveSigmoid(characterApi.characterStats.GetNormalizedHealth());
        }

        protected async Task<Task> RunAttacks(
            List<string> attacks,
            float staminaCost,
            CombatDecision combatDecision
        )
        {
            while (CanCombo(staminaCost, combatDecision) && attacks.Count > 0)
            {
                string nextAttack = attacks[0]; // Get the first attack in the list
                attacks.RemoveAt(0); // Remove the used attack

                // If the combat decision is an air attack, but the player is grounded, exit
                if (IsAirCombatDecision(combatDecision) && characterApi.characterGravity.Grounded)
                {
                    return Task.CompletedTask;
                }

                onAttackBegin?.Invoke();

                if (characterApi.characterGravity.Grounded)
                    characterApi.animatorManager.EnableRootMotion();

                // Play the attack animation
                characterApi.animatorManager.BlendTo(nextAttack, 0.05f);

                // **Wait for animation while checking if the player lands mid-attack**
                Task animationTask = characterApi.animatorManager.WaitForAnimationToFinish(nextAttack);

                while (!animationTask.IsCompleted)
                {
                    await Task.Yield(); // Yield control to avoid blocking

                    // **If player lands mid-air attack, cancel early**
                    if (IsAirCombatDecision(combatDecision) && characterApi.characterGravity.Grounded)
                    {
                        break;
                    }
                }

                characterApi.animatorManager.DisableRootMotion();

                onAttackEnd?.Invoke();

                // **If we exited early due to landing, stop the attack chain**
                if (IsAirCombatDecision(combatDecision) && characterApi.characterGravity.Grounded)
                {
                    return Task.CompletedTask;
                }
            }

            return Task.CompletedTask;
        }

        bool IsAirCombatDecision(CombatDecision combatDecision) => combatDecision == CombatDecision.RIGHT_AIR_ATTACK || combatDecision == CombatDecision.LEFT_AIR_ATTACK;
    }
}
