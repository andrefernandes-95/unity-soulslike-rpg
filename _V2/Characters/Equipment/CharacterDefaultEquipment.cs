namespace AFV2
{
    using UnityEngine;

    public class CharacterDefaultEquipment : MonoBehaviour
    {
        [SerializeField] Weapon defaultLeftWeapon;
        [SerializeField] Weapon defaultRightWeapon;

        [Header("Components")]
        [SerializeField] CharacterWeapons characterWeapons;

        private void Awake()
        {
            characterWeapons.EquipRightWeapon(defaultRightWeapon ?? characterWeapons.FallbackWeapon, 0);
            characterWeapons.EquipLeftWeapon(defaultLeftWeapon ?? characterWeapons.FallbackWeapon, 0);
        }
    }
}
