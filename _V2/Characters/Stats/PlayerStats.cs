namespace AFV2
{
    using CI.QuickSave;

    public class PlayerStats : CharacterStats, ISaveable
    {
        const string HEALTH = "health";
        const string STAMINA = "stamina";
        const string MANA = "mana";
        const string REPUTATION = "reputation";

        public void LoadData(QuickSaveReader reader)
        {
            if (reader.TryRead(HEALTH, out float health))
                SetHealth(health);

            if (reader.TryRead(STAMINA, out float stamina))
                CharacterStamina.SetStamina(stamina);

            if (reader.TryRead(MANA, out float mana))
                SetMana(mana);

            if (reader.TryRead(REPUTATION, out int reputation))
                SetReputation(reputation);
        }

        public void SaveData(QuickSaveWriter writer)
        {
            writer.Write(HEALTH, Health);
            writer.Write(STAMINA, CharacterStamina.Stamina);
            writer.Write(MANA, Mana);
            writer.Write(REPUTATION, Reputation);
        }
    }
}
