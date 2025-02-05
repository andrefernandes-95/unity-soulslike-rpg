namespace AF
{
    using UnityEngine;
    using UnityEngine.AI;
    using UnityEngine.Events;

    public class CharacterTeleportManager : MonoBehaviour
    {
        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Teleport Options")]
        public float minimumTeleportRadiusFromTarget = 5f;
        public float maximumTeleportRadiusFromTarget = 10f;
        public bool teleportNearPlayer = false;

        [Header("VFX")]
        public DestroyableParticle teleportVfx;

        PlayerManager _playerManager;

        public UnityEvent onTeleport;

        [Header("Layer Mask")]
        public string areaName = "";

        /// <summary>
        /// UnityEvent
        /// </summary>
        public void TeleportEnemy()
        {
            Vector3 randomPoint = teleportNearPlayer
                ? Camera.main.transform.position + Camera.main.transform.forward * -2f
                : RandomNavmeshPoint(GetPlayerManager().transform.position, maximumTeleportRadiusFromTarget, minimumTeleportRadiusFromTarget);

            Teleport(randomPoint, Quaternion.identity);

            Vector3 lookRot = randomPoint - characterManager.transform.position;
            lookRot.y = 0;
            characterManager.transform.rotation = Quaternion.LookRotation(lookRot);

            onTeleport?.Invoke();
        }

        Vector3 RandomNavmeshPoint(Vector3 center, float radius, float minDistance)
        {
            for (int i = 0; i < 10; i++) // You can adjust the number of attempts
            {
                Vector3 randomDirection = Random.insideUnitSphere * radius;
                randomDirection += center;

                if (NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, radius, string.IsNullOrEmpty(areaName) ? -1 : NavMesh.GetAreaFromName(areaName)) && Vector3.Distance(navHit.position, center) >= minDistance)
                {
                    return new Vector3(navHit.position.x, GetPlayerManager().transform.position.y, navHit.position.z);
                }
            }

            Debug.LogWarning("Failed to find a valid teleportation position after multiple attempts.");
            return GetPlayerManager().transform.position + GetPlayerManager().transform.forward * -1; // Return zero if no valid position is found after attempts
        }

        PlayerManager GetPlayerManager()
        {
            if (_playerManager == null) { _playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include); }
            return _playerManager;
        }

        public void TeleportNearPlayer()
        {
            Vector3 desiredPosition = GetPlayerManager().transform.position + (GetPlayerManager().transform.forward * -4.5f);
            NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, 15f, NavMesh.AllAreas);

            if (IsValidPosition(hit.position))
            {
                HandleTeleport(hit.position, Quaternion.identity);
            }
        }

        public void Teleport(Vector3 desiredPosition, Quaternion rotation)
        {
            NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);

            if (IsValidPosition(hit.position))
            {
                HandleTeleport(hit.position, rotation);
            }
        }

        void HandleTeleport(Vector3 desiredPosition, Quaternion desiredRotation)
        {
            PlayVfx();

            characterManager.characterController.enabled = false;
            characterManager.agent.enabled = false;
            characterManager.transform.position = desiredPosition;
            characterManager.transform.rotation = desiredRotation;
            characterManager.agent.nextPosition = desiredPosition;
            characterManager.agent.enabled = true;
            characterManager.characterController.enabled = true;

            PlayVfx();
        }

        void PlayVfx()
        {
            if (teleportVfx == null)
            {
                return;
            }

            Instantiate(teleportVfx, characterManager.transform.position, Quaternion.identity);
        }

        bool IsValidPosition(Vector3 position)
        {
            // Check for Infinity or NaN values
            return !float.IsInfinity(position.x) && !float.IsInfinity(position.y) && !float.IsInfinity(position.z) &&
                   !float.IsNaN(position.x) && !float.IsNaN(position.y) && !float.IsNaN(position.z);
        }

    }
}
