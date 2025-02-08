namespace AF
{
    using System.Collections.Generic;
    using AF.Combat;
    using AF.Health;
    using Unity.Cinemachine;
    using UnityEngine;

    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(CinemachineImpulseSource))]
    [RequireComponent(typeof(AudioSource))]
    public abstract class Hitbox : MonoBehaviour
    {
        [Header("Trails")]
        public TrailRenderer trailRenderer;
        private BoxCollider hitCollider => GetComponent<BoxCollider>();

        [Header("Components")]
        CharacterBaseManager hitboxOwner;

        [Header("Sounds")]
        public AudioClip swingSfx;
        public AudioClip hitSfx;
        private AudioSource audioSource => GetComponent<AudioSource>();
        readonly List<IDamageable> damageReceiversHit = new();

        private CinemachineImpulseSource cinemachineImpulseSource => GetComponent<CinemachineImpulseSource>();
        protected float impactForce = 0.1f;

        // Internal flags
        bool canPlayHitSfx = true;

        // Scene References
        private WeaponCollisionFXManager _weaponCollisionFXManager;

        void Awake()
        {
            SetupAudioSource();

            DisableHitbox();

            GetHitboxOwner();
        }

        public void EnableHitbox()
        {
            canPlayHitSfx = true;

            if (trailRenderer != null)
            {
                trailRenderer.Clear();
                trailRenderer.enabled = true;
            }

            if (hitCollider != null)
            {
                hitCollider.enabled = true;
            }

            PlaySwingSound();
        }

        public void DisableHitbox()
        {
            if (trailRenderer != null)
            {
                trailRenderer.enabled = false;
            }

            if (hitCollider != null)
            {
                hitCollider.enabled = false;
            }

            damageReceiversHit.Clear();
        }

        public void OnTriggerEnter(Collider other)
        {
            GetWeaponCollisionFXManager().EvaluateCollision(other, this.gameObject);

            if (other.TryGetComponent(out IDamageable damageable) && !damageReceiversHit.Contains(damageable))
            {
                if (cinemachineImpulseSource != null)
                {
                    cinemachineImpulseSource.GenerateImpulse(GetImpulseForce());
                }

                damageReceiversHit.Add(damageable);
                damageable.OnDamage(hitboxOwner, () =>
                {
                    if (hitSfx != null && canPlayHitSfx && hitboxOwner != null)
                    {
                        canPlayHitSfx = false;
                        PlayHitSound();
                    }
                });

                /*
                if (hitboxOwner != null && hitboxOwner is PlayerManager playerManager)
                {
                    playerManager.playerCombatController.HandlePlayerAttack(damageable, weapon);
                }*/
            }
        }

        void RandomizePitch()
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
        }

        void PlaySwingSound()
        {
            RandomizePitch();
            audioSource.PlayOneShot(swingSfx);
        }

        void PlayHitSound()
        {
            audioSource.PlayOneShot(hitSfx);
        }

        WeaponCollisionFXManager GetWeaponCollisionFXManager()
        {
            if (_weaponCollisionFXManager == null)
            {
                _weaponCollisionFXManager = FindAnyObjectByType<WeaponCollisionFXManager>(FindObjectsInactive.Include);
            }

            return _weaponCollisionFXManager;
        }

        void SetupAudioSource()
        {
            audioSource.playOnAwake = false;
            AudioUtils.Setup3DAudioSource(audioSource);
        }

        void GetHitboxOwner()
        {
            hitboxOwner = this.transform.GetComponentInParent<CharacterBaseManager>();
        }

        public abstract Damage GetDamage();

        public abstract float GetImpulseForce();
    }
}
