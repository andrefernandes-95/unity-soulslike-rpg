namespace AF
{
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(OnDamageTriggerManager))]
    public class ForwardProjectile : MonoBehaviour
    {
        [Header("Projectile Settings")]
        public float Speed = 15f;
        public float MaxLifetime = 5f; // Maximum lifetime before destruction

        [Header("Damage Particle")]
        OnDamageTriggerManager onDamageTriggerManager => GetComponent<OnDamageTriggerManager>();

        [Header("Impact VFX")]
        public GameObject impact;
        Rigidbody rigidBody => GetComponent<Rigidbody>();

        private float spawnTime;

        private void Awake()
        {
            onDamageTriggerManager?.onParticleDamage?.AddListener(OnCollision);
            spawnTime = Time.time; // Store the time when the projectile is spawned
        }

        void FixedUpdate()
        {
            rigidBody.linearVelocity = transform.forward * Speed;

            // Check if the projectile's lifetime has exceeded MaxLifetime
            if (Time.time - spawnTime >= MaxLifetime)
            {
                Destroy(gameObject);
            }
        }

        public void OnCollision()
        {
            Instantiate(impact, transform.position, transform.rotation, null);
            Destroy(gameObject); // Destroy projectile on impact
        }
    }
}
