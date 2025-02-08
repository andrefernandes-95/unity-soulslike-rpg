namespace AFV2
{
    using UnityEngine;

    public class CharacterStats : MonoBehaviour
    {
        private float health;
        [SerializeField] private int maxHealth = 300;
        private float stamina;
        [SerializeField] private int maxStamina = 200;
        private float mana;
        [SerializeField] private int maxMana = 100;

        public int Reputation { get; private set; } = 1;

        public float Health
        {
            get => health;
            private set => health = Mathf.Clamp(value, 0, maxHealth);
        }

        public float Stamina
        {
            get => stamina;
            private set => stamina = Mathf.Clamp(value, 0, maxStamina);
        }

        public float Mana
        {
            get => mana;
            private set => mana = Mathf.Clamp(value, 0, maxMana);
        }

        private void Start()
        {
            Health = maxHealth;
            Stamina = maxStamina;
            Mana = maxMana;
        }

        public void SetHealth(float value) => Health = value;
        public void TakeDamage(float damage) => Health -= damage;
        public void Heal(float amount) => Health += amount;

        public void SetStamina(float value) => Stamina = value;
        public void UseStamina(float amount) => Stamina -= amount;
        public void RecoverStamina(float amount) => Stamina += amount;

        public void SetMana(float value) => Mana = value;
        public void UseMana(float amount) => Mana -= amount;
        public void RecoverMana(float amount) => Mana += amount;

        public void SetReputation(int value) => Reputation = value;
        public void IncreaseReputation(int value) => Reputation += value;
        public void DecreaseReputation(int value) => Reputation -= value;

    }
}
