namespace AF.Detection
{
    using System.Collections.Generic;
    using System.Linq;
    using AF.Characters;
    using AF.Combat;
    using UnityEngine;
    using UnityEngine.Events;
    using Vector3 = UnityEngine.Vector3;

    public class Sight : MonoBehaviour
    {
        public float viewDistance = 10f;
        public float sphereRadius = 5f; // Radius for the sphere cast
        public LayerMask targetLayer;

        [Header("Components")]
        public Transform origin;
        public TargetManager targetManager;
        public CharacterManager characterManager;

        [Header("Tags")]
        public List<string> tagsToDetect = new();

        [Header("Factions")]
        public List<Combatant> factionsToIgnore = new();

        [Header("Events")]
        public UnityEvent OnTargetSighted;

        [Header("Settings")]
        public bool debug = false;

        [Header("Flags")]
        [SerializeField] bool isSighted = false;
        public bool canSight = true;

        public Transform IsTargetInSight()
        {
            Vector3 originPosition = origin.transform.position;
            Vector3 direction = origin.transform.forward;

            // Perform the raycast
            if (Physics.Raycast(originPosition, direction, out RaycastHit hit, viewDistance, targetLayer))
            {
                if (debug) Debug.DrawLine(originPosition, hit.point, Color.red); // Draw a red line for the raycast
                return hit.transform;
            }

            // Draw a green debug line if no target is hit
            if (debug) Debug.DrawRay(originPosition, direction * viewDistance, Color.green);
            return null;
        }

        public Transform IsTargetInSphereSight()
        {
            Vector3 originPosition = origin.transform.position;
            Vector3 direction = origin.transform.forward;

            // Perform the sphere cast
            if (Physics.SphereCast(originPosition, sphereRadius, direction, out RaycastHit hit, viewDistance, targetLayer))
            {
                if (debug) Debug.DrawLine(originPosition, hit.point, Color.blue); // Draw a blue line for the sphere cast
                return hit.transform;
            }

            // Draw a yellow debug sphere if no target is hit
            if (debug) Debug.DrawRay(originPosition, direction * viewDistance, Color.yellow);
            return null;
        }

        public void CastSight()
        {
            if (!canSight)
            {
                return;
            }

            Transform hit = IsTargetInSight(); // Check raycast first
            if (hit == null)
            {
                hit = IsTargetInSphereSight(); // Fallback to sphere cast if raycast fails
            }

            if (hit != null)
            {
                if (hit.transform.CompareTag("Player"))
                {
                    characterManager.greetingMessageController.ShowGreeting();
                }

                // Check if the hit object's tag is in the list of tags to detect
                if (tagsToDetect.Count > 0)
                {
                    isSighted = tagsToDetect.Contains(hit.transform.gameObject.tag);
                }

                if (isSighted && hit.TryGetComponent(out CharacterBaseManager target))
                {
                    if (
                        factionsToIgnore == null
                        || factionsToIgnore.Count == 0
                        || target.combatant == characterManager.combatant
                        || !target.combatant.friendlies.Any(faction => factionsToIgnore.Contains(faction)))
                    {
                        targetManager.SetTarget(target, () =>
                        {
                            HandleTargetSighted();
                        }, false);
                    }
                }
            }
        }

        void HandleTargetSighted()
        {
            OnTargetSighted?.Invoke();
        }

        public void SetDetectionLayer(string layerName)
        {
            this.targetLayer = LayerMask.GetMask(layerName);
        }

        public void SetTagsToDetect(List<string> tagsToDetect)
        {
            this.tagsToDetect.Clear();
            this.tagsToDetect = tagsToDetect;
        }

        public void Set_CanSight(bool value)
        {
            canSight = value;
        }
    }
}
