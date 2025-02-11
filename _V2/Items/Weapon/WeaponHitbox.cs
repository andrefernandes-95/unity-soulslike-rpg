namespace AFV2
{
    using UnityEngine;

    [RequireComponent(typeof(Collider))]
    public class WeaponHitbox : MonoBehaviour
    {
        new Collider collider => GetComponent<Collider>();

        public void EnableHitbox()
        {
            collider.enabled = true;
        }

        public void DisableHitbox()
        {
            collider.enabled = false;
        }
    }
}