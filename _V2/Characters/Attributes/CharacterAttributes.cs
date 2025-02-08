namespace AFV2
{
    using UnityEngine;

    public class CharacterAttributes : MonoBehaviour
    {
        [SerializeField] private int vitality = 1;
        [SerializeField] private int endurance = 1;
        [SerializeField] private int intelligence = 1;
        [SerializeField] private int strength = 1;
        [SerializeField] private int dexterity = 1;

        // Properties with clamping
        public int Vitality
        {
            get => vitality;
            private set => vitality = Mathf.Clamp(value, 1, 99); // Min 1, Max 99
        }

        public int Endurance
        {
            get => endurance;
            private set => endurance = Mathf.Clamp(value, 1, 99);
        }

        public int Intelligence
        {
            get => intelligence;
            private set => intelligence = Mathf.Clamp(value, 1, 99);
        }

        public int Strength
        {
            get => strength;
            private set => strength = Mathf.Clamp(value, 1, 99);
        }

        public int Dexterity
        {
            get => dexterity;
            private set => dexterity = Mathf.Clamp(value, 1, 99);
        }


        // Methods to modify attributes safely
        public void SetVitality(int value) => Vitality = value;
        public void IncreaseVitality(int amount) => Vitality += amount;
        public void DecreaseVitality(int amount) => Vitality -= amount;

        public void SetEndurance(int value) => Endurance = value;
        public void IncreaseEndurance(int amount) => Endurance += amount;
        public void DecreaseEndurance(int amount) => Endurance -= amount;

        public void SetIntelligence(int value) => Intelligence = value;
        public void IncreaseIntelligence(int amount) => Intelligence += amount;
        public void DecreaseIntelligence(int amount) => Intelligence -= amount;

        public void SetStrength(int value) => Strength = value;
        public void IncreaseStrength(int amount) => Strength += amount;
        public void DecreaseStrength(int amount) => Strength -= amount;

        public void SetDexterity(int value) => Dexterity = value;
        public void IncreaseDexterity(int amount) => Dexterity += amount;
        public void DecreaseDexterity(int amount) => Dexterity -= amount;
    }
}
