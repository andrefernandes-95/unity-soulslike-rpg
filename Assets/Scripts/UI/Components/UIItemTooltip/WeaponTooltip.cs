namespace AFV2
{
    using TMPro;
    using UnityEngine;

    public class WeaponTooltip : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI weaponType;
        [SerializeField] TextMeshProUGUI physicalAttack;

        public void Show(WeaponInstance weaponInstance)
        {
            Weapon weapon = weaponInstance.item as Weapon;

            RenderWeaponType(weapon);
            RenderPhysicalAttack(weapon);
        }

        void RenderWeaponType(Weapon weapon)
        {
            weaponType.text = weapon.weaponType.DisplayName;
        }

        void RenderPhysicalAttack(Weapon weapon)
        {
            string physicalAttackLabel = "Physical Attack";

            if (Glossary.IsPortuguese())
            {
                physicalAttackLabel = "Ataque Físico";
            }

            physicalAttack.text = $"+{weapon.damage.physical} {physicalAttackLabel}";
        }
    }
}
