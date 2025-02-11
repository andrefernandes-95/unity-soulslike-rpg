namespace AFV2
{
    using UnityEngine;

    public class CharacterApi : MonoBehaviour
    {
        public AnimatorManager animatorManager;
        public StateMachine stateMachine;
        public CharacterController characterController;


        public CharacterStats characterStats;

        public CharacterMovement characterMovement;
        public CharacterGravity characterGravity;
        public CharacterEquipment characterEquipment;
    }
}
