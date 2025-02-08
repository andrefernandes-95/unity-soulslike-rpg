namespace AFV2
{
    using UnityEngine;

    public class PlayerFallState : FallState
    {
        [Header("Movement While Falling")]
        public float FallMoveSpeed = 3f;

        [Header("Components")]
        [SerializeField] PlayerController playerController;

        public override State Tick()
        {
            if (playerController.IsMoving())
                playerController.Move(FallMoveSpeed * (playerController.IsSprinting() ? 1.5f : 1));

            return base.Tick();
        }
    }
}
