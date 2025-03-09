namespace AFV2
{
    using UnityEngine;

    public class PlayerBowAimingState : PlayerAimState
    {
        [SerializeField] private string HASH_AIM_IDLE = "Aim Idle";
        [SerializeField] private string HASH_AIM_WALK = "Aim Walk";
        [SerializeField] private string HASH_AIM_SHOT = "Aim Shot";

        [Header("Archery Settings")]
        [SerializeField] float aimWalkSpeed = 2f;

        bool isShooting = false;

        void Awake()
        {
            characterApi.characterArchery.onShotFinished.AddListener(ResetState);
        }

        public override string GetHashAimIdle()
        {
            return HASH_AIM_IDLE;
        }

        public override string GetHashAimWalk()
        {
            return HASH_AIM_WALK;
        }

        public override string GetHashAimShot()
        {
            return HASH_AIM_SHOT;
        }

        public override float GetMoveSpeed()
        {
            return aimWalkSpeed;
        }

        public override void OnLightAttackInput()
        {
            if (isShooting)
            {
                return;
            }

            characterApi.characterArchery.Shoot();

            characterApi.characterArchery.HideArrowWorld();

            characterApi.animatorManager.BlendTo(GetHashAimShot(), 0.05f);

            isShooting = true;
        }

        public override void OnEnter()
        {
            characterApi.characterArchery.ShowArrowWorld();
            isShooting = false;
        }

        public override void OnExit()
        {
            characterApi.characterArchery.HideArrowWorld();
        }

        void ResetState()
        {
            if (playerController.IsAiming())
            {
                OnEnter();

                isMoving = false;
                HandleAimMovement();
            }
        }

        protected override void HandleAimMovement()
        {
            // If shooting, ignore aim idle / aim walk transitions 
            if (isShooting)
            {
                return;
            }

            base.HandleAimMovement();
        }
    }
}
