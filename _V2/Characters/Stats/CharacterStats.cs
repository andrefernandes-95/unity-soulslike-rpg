namespace AFV2
{
    using UnityEngine;
    using UnityEngine.Events;

    public class CharacterStats : MonoBehaviour
    {
        private float health;
        [SerializeField] private int maxHealth = 300;
        public int MaxHealth => maxHealth;

        [SerializeField] CharacterStamina characterStamina;
        public CharacterStamina CharacterStamina => characterStamina;

        private float mana;
        [SerializeField] private int maxMana = 100;
        public int MaxMana => maxMana;

        public int Reputation { get; private set; } = 1;

        [HideInInspector] public UnityEvent onHealthChange, onManaChange;

        public float Health
        {
            get => health;
            private set
            {
                health = Mathf.Clamp(value, 0, maxHealth);
                onHealthChange?.Invoke();
            }
        }

        public float Mana
        {
            get => mana;
            private set
            {
                mana = Mathf.Clamp(value, 0, maxMana);
                onManaChange?.Invoke();
            }
        }

        private void Start()
        {
            Health = maxHealth;
            Mana = maxMana;
        }

        public void SetHealth(float value) => Health = value;
        public void TakeDamage(float damage) => Health -= damage;
        public void Heal(float amount) => Health += amount;

        /// <summary>
        /// Normalize stamina between 0 and 1
        /// </summary>
        /// <returns></returns>
        public float GetNormalizedHealth() => health / maxHealth;

        public void SetMana(float value) => Mana = value;
        public void UseMana(float amount) => Mana -= amount;
        public void RecoverMana(float amount) => Mana += amount;

        public void SetReputation(int value) => Reputation = value;
        public void IncreaseReputation(int value) => Reputation += value;
        public void DecreaseReputation(int value) => Reputation -= value;

    }
}
