namespace AFV2
{
    using UnityEngine;

    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] AudioSource audioSource;

        [Header("Sounds")]
        [SerializeField] AudioClip uiAppear;
        [SerializeField] AudioClip uiDisappear;
        [SerializeField] AudioClip confirm;

        [Header("UI")]
        [SerializeField] GameObject ui;

        void Awake()
        {
        }

        private void Start()
        {
            if (ui)
                ui.SetActive(false);

            // Attach SphereCollider at runtime
            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 2f; // Adjust the radius as needed
        }

        private InteractableListener FindInteractableListener(Transform targetTransform)
        {
            // First check the target (self)
            var interactableListener = targetTransform.GetComponent<InteractableListener>();

            if (interactableListener != null)
            {
                return interactableListener;
            }

            // Then check the parent objects
            interactableListener = targetTransform.GetComponentInParent<InteractableListener>();

            if (interactableListener != null)
            {
                return interactableListener;
            }

            // Finally, check the children objects
            interactableListener = targetTransform.GetComponentInChildren<InteractableListener>();

            return interactableListener;
        }

        private void OnTriggerEnter(Collider other)
        {
            var interactableListener = FindInteractableListener(other.transform);

            if (interactableListener != null)
            {
                interactableListener.OnCaptured(this);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            var interactableListener = FindInteractableListener(other.transform);

            if (interactableListener != null)
            {
                interactableListener.OnLost(this);
            }
        }

        public abstract void OnInteract(InteractableListener interactableListener);

        public void PlayConfirm()
        {
            if (confirm != null && audioSource != null)
            {
                audioSource.PlayOneShot(confirm);
            }
        }
        public void PlayUIAppear()
        {
            if (uiAppear != null && audioSource != null)
            {
                audioSource.PlayOneShot(uiAppear);
            }
        }
        public void PlayUIDisappear()
        {
            if (uiDisappear != null && audioSource != null)
            {
                audioSource.PlayOneShot(uiDisappear);
            }
        }

        public virtual void Enable()
        {
            if (ui != null)
            {
                UIUtils.FadeIn(ui);
                PlayUIAppear();
            }
        }

        public void Disable()
        {
            if (ui != null)
            {
                UIUtils.FadeOut(ui);
                PlayUIDisappear();
            }
        }
    }
}
