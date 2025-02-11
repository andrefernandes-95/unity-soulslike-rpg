namespace AFV2
{
    public class CharacterEquipmentScreen : UIScreen
    {
        public UIScreen EquipmentSlots;
        public UIScreen ItemList;

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ShowEquipmentSlots()
        {
            EquipmentSlots.Show(CharacterApi);
            ItemList.Hide();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ShowItemList()
        {
            ItemList.Show(CharacterApi);
            EquipmentSlots.Hide();
        }

    }
}
