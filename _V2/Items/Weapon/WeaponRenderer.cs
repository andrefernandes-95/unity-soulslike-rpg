namespace AFV2
{
    using UnityEngine;

    public class WeaponRenderer : MonoBehaviour
    {
        [SerializeField] MeshRenderer meshRenderer;
        public MeshRenderer MeshRenderer => meshRenderer;

        private void Awake()
        {
            if (meshRenderer == null) return;
            meshRenderer.enabled = false;
        }

        public void EnableRenderer()
        {
            if (meshRenderer == null) return;
            meshRenderer.enabled = true;
        }
    }
}
