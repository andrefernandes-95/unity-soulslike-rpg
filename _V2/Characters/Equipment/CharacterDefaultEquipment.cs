namespace AFV2
{
    using UnityEngine;

    public class CharacterDefaultEquipment : MonoBehaviour
    {
        public Weapon defaultRightWeapon;


        [Header("Components")]
        public CharacterWeapons characterWeapons;
        public CharacterEquipment characterEquipment;

        private void Awake()
        {
            if (defaultRightWeapon != null) characterWeapons.EquipRightWeapon(defaultRightWeapon, 0);
        }
    }
}
