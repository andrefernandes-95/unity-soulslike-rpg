namespace AF
{
    using System.Collections;
    using System.Collections.Generic;
    using AF.Health;
    using AF.Shooting;
    using UnityEngine;
    using UnityEngine.Events;

    public class Projectile : MonoBehaviour, IProjectile
    {
        [Header("Lifespan")]
        public float timeBeforeDestroying = 10;

        [Header("Velocity")]
        public ForceMode forceMode;
        public float forwardVelocity = 10f;
        public bool useOwnDirection = false;

        [Header("Stats")]
        public Damage damage;
        public bool scaleWithIntelligence = false;

        public Rigidbody rigidBody;
        public GameObject disappearingFx;

        [Header("Events")]

        [Tooltip("Fires immediately after instatied")] public UnityEvent onFired;
        [Tooltip("Fires after 0.1ms")] public UnityEvent onFired_After;
        public UnityEvent onCollision;
        public float onFired_AfterDelay = 0.1f;

        readonly List<Collider> hitColliders = new();
        CharacterBaseManager shooter;
        Vector3 previousPosition;

        [Header("Collision Options")]
        public bool collideWithAnything = false;
        public UnityEvent onAnyCollision;

        [Header("SFX")]
        public AudioSource audioSource => GetComponent<AudioSource>();
        public AudioClip releaseSfx;
        public AudioClip onCollisionSfx;
        bool hasPlayedCollisionSfx = false;

        private void OnEnable()
        {
            onFired?.Invoke();

            StartCoroutine(HandleOnFiredAfter_Coroutine());

            PlayReleaseSFX();
        }

        private void Update()
        {
            AlignToMovement();
        }

        void AlignToMovement()
        {
            // Get the direction of movement by comparing the current position to the previous position
            Vector3 direction = (transform.position - previousPosition).normalized;

            // Only update the rotation if there's movement
            if (direction != Vector3.zero)
            {
                // Rotate the arrow to face the direction of movement
                transform.rotation = Quaternion.LookRotation(direction);
            }

            // Update the previous position for the next frame
            previousPosition = transform.position;
        }

        private void OnDisable()
        {
            if (disappearingFx != null)
            {
                Instantiate(disappearingFx, transform.position, Quaternion.identity);
            }
        }

        public void Shoot(CharacterBaseManager shooter, Vector3 aimForce, ForceMode forceMode)
        {
            this.shooter = shooter;

            if (useOwnDirection)
            {
                transform.rotation = Quaternion.LookRotation(aimForce - transform.position);
            }

            rigidBody.AddForce(useOwnDirection ? (transform.forward * GetForwardVelocity()) : aimForce, forceMode);
        }

        public void ShootForward()
        {
            rigidBody.AddForce(transform.forward * GetForwardVelocity(), forceMode);
        }

        void OnTriggerEnter(Collider other)
        {
            if (hitColliders.Contains(other))
            {
                return;
            }

            hitColliders.Add(other);

            other.TryGetComponent(out DamageReceiver damageReceiver);

            HandleCollision(damageReceiver);
        }


        public void HandleCollision(DamageReceiver damageReceiver)
        {
            if (!CanCollide(damageReceiver))
            {
                return;
            }

            PlayOnCollisionSFX();

            if (collideWithAnything)
            {
                onAnyCollision?.Invoke();
                return;
            }

            if (AreFriends(damageReceiver, shooter))
            {
                return;
            }

            ApplyDamage(damageReceiver);
            ApplyTarget(damageReceiver);

            onCollision?.Invoke();

            StartCoroutine(HandleDestroy_Coroutine());
        }

        void ApplyDamage(DamageReceiver damageReceiver)
        {
            if (shooter is PlayerManager playerManager && playerManager.attackStatManager.equipmentDatabase.GetCurrentWeapon().Exists())
            {
                if (scaleWithIntelligence)
                {
                    damage.ScaleSpell(
                        playerManager.attackStatManager, playerManager.attackStatManager.equipmentDatabase.GetCurrentWeapon(), 0, false, false, false);
                }
                else if (playerManager.attackStatManager.HasBowEquipped())
                {
                    damage.ScaleProjectile(playerManager.attackStatManager, playerManager.attackStatManager.equipmentDatabase.GetCurrentWeapon());
                }
            }
            else if (shooter is CharacterManager enemy)
            {
                damage.ScaleDamageForNewGamePlus(enemy.gameSession);
            }

            damageReceiver.ApplyDamage(shooter, damage);
        }

        void ApplyTarget(DamageReceiver damageReceiver)
        {
            if (shooter == null)
            {
                return;
            }

            if (damageReceiver == null || damageReceiver.character is not CharacterManager characterManager)
            {
                return;
            }

            characterManager.targetManager.SetTarget(shooter);
        }

        bool CanCollide(DamageReceiver damageReceiver)
        {
            if (damageReceiver == null)
            {
                return collideWithAnything;
            }

            return damageReceiver.character != shooter;
        }

        bool AreFriends(DamageReceiver damageReceiver, CharacterBaseManager shooter)
        {
            if (shooter == null || damageReceiver == null || damageReceiver.character == null)
            {
                return false;
            }

            return damageReceiver.character.combatant.IsFriendsWith(shooter.combatant);
        }

        public float GetForwardVelocity()
        {
            return forwardVelocity;
        }

        public ForceMode GetForceMode()
        {
            return forceMode;
        }

        void PlayReleaseSFX()
        {
            if (releaseSfx != null && audioSource != null)
            {
                audioSource.PlayOneShot(releaseSfx);
            }
        }

        void PlayOnCollisionSFX()
        {
            if (onCollisionSfx != null && audioSource != null && !hasPlayedCollisionSfx)
            {
                hasPlayedCollisionSfx = true;
                audioSource.PlayOneShot(onCollisionSfx);
            }
        }

        IEnumerator HandleOnFiredAfter_Coroutine()
        {
            yield return new WaitForSeconds(onFired_AfterDelay);
            onFired_After?.Invoke();
        }

        IEnumerator HandleDestroy_Coroutine()
        {
            yield return new WaitForSeconds(timeBeforeDestroying);

            Destroy(this.gameObject);
        }
    }
}
