namespace AFV2
{
    using UnityEngine;
    using UnityEngine.Events;

    public class CharacterMana : MonoBehaviour, ISaveable
    {
        [SerializeField] private int maxMana = 100;
        public int MaxMana => maxMana;

        private float _mana;
        public float Mana
        {
            get => _mana;
            private set
            {
                _mana = Mathf.Clamp(value, 0, maxMana);
                onManaChange?.Invoke();
            }
        }

        [HideInInspector] public UnityEvent onManaChange;

        [Header("Components")]
        [SerializeField] CharacterApi characterApi;

        private void Start()
        {
            Mana = maxMana;
        }

        public void SetMana(float value) => Mana = value;
        public void UseMana(float amount) => Mana -= amount;
        public void RecoverMana(float amount) => Mana += amount;

        public void SaveData(SaveWriter writer)
        {
            if (characterApi.isSaveable)
            {
                writer.Write(GetSavePath(), Mana);
            }
        }

        public void LoadData(SaveReader reader)
        {
            if (characterApi.isSaveable)
            {
                if (reader.TryRead(GetSavePath(), out float mana))
                {
                    SetMana(mana);
                }
            }

        }

        string GetSavePath() => $"{characterApi.GetCharacterId()}.mana";
    }
}
