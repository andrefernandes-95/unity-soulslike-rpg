namespace AFV2
{
    using UnityEngine;

    [RequireComponent(typeof(CharacterAttack))]
    [RequireComponent(typeof(CharacterCombatDecision))]
    public class CharacterCombat : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] protected CharacterApi characterApi;
        [SerializeField] CharacterCombatDecision characterCombatDecision => GetComponent<CharacterCombatDecision>();
        public CharacterCombatDecision CharacterCombatDecision => characterCombatDecision;
        [SerializeField] CharacterAttack characterAttack;
        public CharacterAttack CharacterAttack => characterAttack;

        public virtual bool CanCombo(float staminaCost, CombatDecision combatDecision)
        {
            if (!characterApi.characterStamina.HasEnoughStamina(staminaCost))
            {
                return false;
            }

            // If target is close

            // More combos when health is high
            return Random.Range(0, 1f) <= MathHelpers.PositiveSigmoid(characterApi.characterHealth.GetNormalizedHealth());
        }
    }
}
