namespace AFV2
{
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Animations;

    public abstract class PlayerAimState : State
    {
        [SerializeField] float blendBetweenActions = 0.05f;

        [Header("Components")]
        [SerializeField] protected CharacterApi characterApi;
        [SerializeField] protected PlayerController playerController;
        [SerializeField] CameraController cameraController;
        [SerializeField] PlayerCamera playerCamera;

        [Header("Look At Constraints")]
        [SerializeField] protected LookAtConstraint lookAtConstraint;

        [Header("Transition State")]
        public IdleState idleState;

        protected bool isMoving = false;


        public override void OnStateEnter()
        {
            characterApi.animatorManager.BlendTo(GetHashAimIdle(), blendBetweenActions);
            isMoving = false;
            cameraController.rotateWithCamera = true;
            playerCamera.BeginAiming();
            lookAtConstraint.constraintActive = true;

            OnEnter();
        }

        public override Task OnStateExit()
        {
            OnExit();
            cameraController.rotateWithCamera = false;
            playerCamera.EndAiming();
            lookAtConstraint.constraintActive = false;
            return Task.CompletedTask;
        }

        public abstract void OnEnter();
        public abstract void OnExit();

        public override State Tick()
        {
            HandleAimMovement();

            if (playerController.IsLightAttacking())
            {
                playerController.ResetCombatFlags();
                OnLightAttackInput();
            }

            if (!playerController.IsAiming())
            {
                return idleState;
            }

            return this;
        }

        protected virtual void HandleAimMovement()
        {
            if (playerController.IsMoving() && !isMoving)
            {
                characterApi.animatorManager.BlendTo(GetHashAimWalk(), blendBetweenActions);
                isMoving = true;
            }
            else if (playerController.IsMoving() == false && isMoving)
            {
                characterApi.animatorManager.BlendTo(GetHashAimIdle(), blendBetweenActions);
                isMoving = false;
            }

            if (isMoving)
            {
                characterApi.characterMovement.Move(GetMoveSpeed(), playerController.GetPlayerRotation());
            }
        }

        public abstract void OnLightAttackInput();
        public abstract string GetHashAimIdle();
        public abstract string GetHashAimWalk();
        public abstract string GetHashAimShot();
        public abstract float GetMoveSpeed();
    }
}
