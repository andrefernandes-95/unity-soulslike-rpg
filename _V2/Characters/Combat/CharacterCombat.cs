using UnityEngine;

namespace AFV2
{
    public class CharacterCombat : MonoBehaviour
    {
        [SerializeField] int comboCount = 0;
        [SerializeField] string HASH_ATTACK_A = "Attack A";
        [SerializeField] string HASH_ATTACK_B = "Attack B";
        [SerializeField] string HASH_ATTACK_C = "Attack C";
        [SerializeField] string HASH_ATTACK_D = "Attack D";

        public string GetAttackAnimation()
        {
            if (comboCount == 1)
                return HASH_ATTACK_B;
            if (comboCount == 2)
                return HASH_ATTACK_C;
            if (comboCount == 3)
                return HASH_ATTACK_D;

            return HASH_ATTACK_A;
        }

        public void IncreaseComboCount()
        {
            comboCount++;

            if (comboCount > 3) comboCount = 0;
        }

        public void ResetComboCount() => comboCount = 0;

    }
}