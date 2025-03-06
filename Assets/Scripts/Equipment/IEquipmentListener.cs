namespace AFV2
{
    public interface IEquipmentListener
    {
        void OnEquipmentChanged(EquipmentSlotType slot, Item item);
    }
}
