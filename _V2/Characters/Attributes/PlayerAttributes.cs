namespace AFV2
{
    using CI.QuickSave;

    public class PlayerAttributes : CharacterAttributes, ISaveable
    {
        const string VITALITY = "vitality";
        const string ENDURANCE = "endurance";
        const string INTELLIGENCE = "intelligence";
        const string STRENGTH = "strength";
        const string DEXTERITY = "dexterity";

        public void LoadData(QuickSaveReader reader)
        {
            if (reader.TryRead(VITALITY, out int vitality))
                SetVitality(vitality);

            if (reader.TryRead(ENDURANCE, out int endurance))
                SetEndurance(endurance);

            if (reader.TryRead(INTELLIGENCE, out int intelligence))
                SetIntelligence(intelligence);

            if (reader.TryRead(STRENGTH, out int strength))
                SetStrength(strength);

            if (reader.TryRead(DEXTERITY, out int dexterity))
                SetDexterity(dexterity);
        }

        public void SaveData(QuickSaveWriter writer)
        {
            writer.Write(VITALITY, Vitality);
            writer.Write(ENDURANCE, Endurance);
            writer.Write(INTELLIGENCE, Intelligence);
            writer.Write(STRENGTH, Strength);
            writer.Write(DEXTERITY, Dexterity);
        }
    }
}
