namespace AFV2
{
    using System.Threading.Tasks;
    using UnityEngine;

    public class PlayerAirAttackState : AirAttackState
    {
        [Header("Movement While In Air")]
        public float AirAttackMoveSpeed = 3f;
        public float SprintSpeedMultiplier = 1.2f;

        [Header("Components")]
        [SerializeField] CharacterApi characterApi;
        [SerializeField] PlayerController playerController;

        public override Task OnStateExit()
        {
            playerController.ResetCombatFlags();
            return base.OnStateExit();
        }

        public override State Tick()
        {
            if (playerController.IsMoving())
                characterApi.characterMovement.Move(
                    AirAttackMoveSpeed * (playerController.IsSprinting() ? SprintSpeedMultiplier : 1), playerController.GetPlayerRotation());

            return base.Tick();
        }
    }
}
