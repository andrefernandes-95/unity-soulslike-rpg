namespace AFV2
{
    using UnityEngine;

    public class PlayerInteractableListener : InteractableListener
    {
        [SerializeField] PlayerController playerController;
        [SerializeField] InputListener inputListener;

        void Awake()
        {
            inputListener.onInteract.AddListener(HandleOnInteract);
        }

        void HandleOnInteract()
        {
            if (!playerController.CanControlPlayer())
                return;

            Interact();
        }
    }
}
