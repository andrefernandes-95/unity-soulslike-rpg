namespace AFV2
{
    using AF;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;


#if UNITY_EDITOR

    [CustomEditor(typeof(Weapon), editorForChildClasses: true)]
    public class WeaponEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Weapon weapon = target as Weapon;

            CharacterEquipment characterEquipment = weapon
                ?.GetComponentInParent<CharacterApi>()
                ?.characterEquipment;

            if (characterEquipment == null) return;

            if (GUILayout.Button("Equip Right Weapon (Slot 0)"))
                characterEquipment.EquipRightWeapon(weapon);

            if (GUILayout.Button("Unequip Right Weapon (Slot 0)"))
                characterEquipment.UnequipRightWeapon();

            if (GUILayout.Button("Equip Left Weapon (Slot 0)"))
                characterEquipment.EquipLeftWeapon(weapon);

            if (GUILayout.Button("Unequip Left Weapon (Slot 0)"))
                characterEquipment.UnequipLeftWeapon();
        }
    }
#endif

    public class Weapon : Item
    {
        [Header("Scaling")]
        public WeaponScaling strengthScaling = WeaponScaling.E;
        public WeaponScaling dexterityScaling = WeaponScaling.E;
        public WeaponScaling intelligenceScalling = WeaponScaling.E;

        [Header("Events")]
        public UnityEvent OnEquip;
        public UnityEvent OnUnequip;

        [Header("Pivots")]
        public Vector3 rightHandPivot;
        public Vector3 rightHandRotation;
        public Vector3 leftHandPivot;
        public Vector3 leftHandRotation;

        MeshRenderer meshRenderer => GetComponent<MeshRenderer>();

        private void Awake()
        {
            if (meshRenderer != null) meshRenderer.enabled = false;
        }

        public void Equip(Transform parent, bool isRightHand)
        {
            Weapon weaponChild = Instantiate(this, parent);
            weaponChild.name += " " + (isRightHand ? "(Right)" : "(Left)");
            weaponChild.transform.localPosition = isRightHand ? rightHandPivot : leftHandPivot;
            weaponChild.transform.localEulerAngles = isRightHand ? rightHandRotation : leftHandRotation;
        }

        public void Unequip(Transform parent)
        {
            Weapon weaponChild = parent.GetComponentInChildren<Weapon>();
            if (weaponChild != null)
                Destroy(weaponChild.gameObject);
        }
    }
}
