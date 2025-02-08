namespace AF
{
    using UnityEngine;

    public class DeactivateRendererOnStart : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            if (TryGetComponent<MeshRenderer>(out var meshRenderer)) meshRenderer.enabled = false;
        }
    }
}
