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

        protected virtual (List<string> availableAttacks, float staminaCost, CombatDecision combatDecision1) GetNextAttack()
        {
            CombatDecision combatDecision = characterCombatDecision.GetCombatDecision();
            float staminaCost = 0f;

            List<string> availableAttacks = new();

            characterApi.characterEquipment.characterWeapons.TryGetActiveRightWeapon(out Weapon rightHandWeapon);
            characterApi.characterEquipment.characterWeapons.TryGetActiveLeftWeapon(out Weapon leftHandWeapon);

            if (combatDecision == CombatDecision.RIGHT_LIGHT_ATTACK)
            {
                if (rightHandWeapon != null)
                {
                    availableAttacks = rightHandWeapon.GetAttacksForCombatDecision(combatDecision);
                }

                staminaCost = lightAttackStaminaCost;
            }
            else if (combatDecision == CombatDecision.LEFT_LIGHT_ATTACK)
            {
                if (leftHandWeapon != null)
                {
                    availableAttacks = leftHandWeapon.GetAttacksForCombatDecision(combatDecision);
                }

                staminaCost = lightAttackStaminaCost;
            }
            else if (combatDecision == CombatDecision.HEAVY_ATTACK)
            {
                List<string> heavyAttacks = new();

                if (rightHandWeapon != null)
                {
                    heavyAttacks = rightHandWeapon.GetAttacksForCombatDecision(combatDecision);
                }
                else if (leftHandWeapon != null)
                {
                    heavyAttacks = leftHandWeapon.GetAttacksForCombatDecision(combatDecision);
                }

                availableAttacks = heavyAttacks;
                staminaCost = heavyAttackStaminaCost;
            }

            return (availableAttacks, staminaCost, combatDecision);
        }

        public async virtual Task Attack()
        {
            (List<string> availableAttacks, float staminaCost, CombatDecision combatDecision) = GetNextAttack();

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
            bool isFirst = true;

            while (CanCombo(staminaCost, combatDecision) && attacks.Count > 0)
            {
                string nextAttack = attacks[0]; // Get the first attack in the list
                attacks.RemoveAt(0); // Remove the used attack

                onAttackBegin?.Invoke();

                characterApi.animatorManager.EnableRootMotion();

                // Play the attack animation
                characterApi.animatorManager.BlendTo(nextAttack);

                await characterApi.animatorManager.WaitForAnimationToFinish(nextAttack, isFirst ? 1f : 0.85f);

                characterApi.animatorManager.DisableRootMotion();

                onAttackEnd?.Invoke();

                isFirst = false;
            }

            return Task.CompletedTask;
        }
    }
}
