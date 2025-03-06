namespace AFV2
{
    using UnityEngine;

    [RequireComponent(typeof(UniqueID))]
    public class CharacterApi : MonoBehaviour
    {
        #region Core Components
        public AnimatorManager animatorManager;
        public CharacterController characterController;
        #endregion

        #region AI
        public StateMachine stateMachine;
        #endregion

        #region Audio
        public CharacterSound characterSound;
        #endregion

        #region Stats
        public CharacterHealth characterHealth;
        public CharacterStamina characterStamina;
        public CharacterMana characterMana;
        #endregion

        #region Movement
        public CharacterMovement characterMovement;
        public CharacterGravity characterGravity;
        #endregion

        #region Inventory
        public CharacterEquipment characterEquipment;
        public CharacterInventory characterInventory;
        #endregion

        #region  ID
        UniqueID uniqueID => GetComponent<UniqueID>();
        public string GetCharacterId() => uniqueID.ID;
        #endregion

        #region Save System
        [Header("💾 Save")]
        public bool isSaveable = true;
        #endregion
    }
}
