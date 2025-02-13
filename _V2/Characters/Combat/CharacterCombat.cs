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

        [Header("Light Attacks")]
        [SerializeField] float lightAttackStaminaCost = 10f;
        public float LightAttackStaminaCost => lightAttackStaminaCost;

        [Header("Heavy Attacks")]
        [SerializeField] float heavyAttackStaminaCost = 30f;
        public float HeavyAttackStaminaCost => heavyAttackStaminaCost;

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
            if (!characterApi.characterStats.HasEnoughStamina(staminaCost))
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

                onAttackBegin?.Invoke();

                if (characterApi.characterGravity.Grounded)
                    characterApi.animatorManager.EnableRootMotion();

                // Play the attack animation
                characterApi.animatorManager.BlendTo(nextAttack, 0.05f);

                await characterApi.animatorManager.WaitForAnimationToFinish(nextAttack);

                characterApi.animatorManager.DisableRootMotion();

                onAttackEnd?.Invoke();
            }

            return Task.CompletedTask;
        }
    }
}
