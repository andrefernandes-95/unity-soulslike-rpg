namespace AFV2
{
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Animations;

    public class PlayerAimState : State
    {

        [SerializeField] private string HASH_AIM_IDLE = "Aim Idle";
        [SerializeField] private string HASH_AIM_WALK = "Aim Walk";
        [SerializeField] float blend = 0.05f;

        [Header("Components")]
        [SerializeField] CharacterApi characterApi;
        [SerializeField] PlayerController playerController;
        [SerializeField] CameraController cameraController;
        [SerializeField] PlayerCamera playerCamera;

        [SerializeField] LookAtConstraint lookAtConstraint;

        [Header("Transition State")]
        public IdleState idleState;

        bool isMoving = false;


        [Header("Projectile Settings")]
        public GameObject projectilePrefab; // Assign a Cube prefab in the Inspector
        public Transform bowTransform;      // The point where the cube is spawned
        public float shootForce = 15f;      // The initial launch force

        public override async void OnStateEnter()
        {
            characterApi.animatorManager.BlendTo(HASH_AIM_IDLE, blend);
            isMoving = false;

            cameraController.rotateWithCamera = true;
            playerCamera.BeginAiming();
            lookAtConstraint.constraintActive = true;
        }

        public override async Task OnStateExit()
        {
            cameraController.rotateWithCamera = false;
            playerCamera.EndAiming();
            lookAtConstraint.constraintActive = false;
        }

        public override State Tick()
        {
            if (playerController.IsMoving() && !isMoving)
            {
                characterApi.animatorManager.BlendTo(HASH_AIM_WALK, blend);
                isMoving = true;
            }
            else if (playerController.IsMoving() == false && isMoving)
            {
                characterApi.animatorManager.BlendTo(HASH_AIM_IDLE, blend);
                isMoving = false;
            }

            if (isMoving)
            {
                characterApi.characterMovement.Move(2f, playerController.GetPlayerRotation());
            }

            if (!playerController.IsAiming())
            {
                return idleState;
            }

            if (playerController.IsLightAttacking())
            {
                playerController.ResetCombatFlags();

                ShootProjectile();
            }

            return this;
        }



        void ShootProjectile()
        {
            if (projectilePrefab == null || bowTransform == null) return;

            // Spawn the cube at the bow position
            GameObject projectile = Instantiate(projectilePrefab, bowTransform.position, lookAtConstraint.GetSource(0).sourceTransform.transform.rotation);


            Rigidbody rb = projectile.AddComponent<Rigidbody>(); // Add Rigidbody for physics

            // Apply force to launch projectile
            rb.linearVelocity = rb.transform.forward * shootForce;

            Vector3 flyRotation = lookAtConstraint.GetSource(0).sourceTransform.transform.position - characterApi.transform.position;
            flyRotation.y = 0;
            rb.transform.rotation = Quaternion.LookRotation(flyRotation);
        }


    }
}
