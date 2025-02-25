namespace AFV2
{
    using UnityEngine;
    using UnityEngine.Events;

    public class CharacterHealth : MonoBehaviour, ISaveable
    {
        private float _health;
        public float Health
        {
            get => _health;
            private set
            {
                _health = Mathf.Clamp(value, 0, maxHealth);
                onHealthChange?.Invoke();
            }
        }

        public int MaxHealth => maxHealth;
        [SerializeField] private int maxHealth = 300;

        [HideInInspector] public UnityEvent onHealthChange;

        [Header("Components")]
        [SerializeField] CharacterApi characterApi;

        private void Start()
        {
            Health = maxHealth;
        }

        public void SetHealth(float value) => Health = value;
        public void TakeDamage(float damage) => Health -= damage;
        public void Heal(float amount) => Health += amount;

        public float GetNormalizedHealth() => Health / maxHealth;

        public void SaveData(SaveWriter writer)
        {
            writer.Write(GetSavePath(), Health);
        }

        public void LoadData(SaveReader reader)
        {
            if (reader.TryRead(GetSavePath(), out float health))
                SetHealth(health);
        }

        string GetSavePath() => $"{characterApi.GetCharacterId()}.health";
    }
}
