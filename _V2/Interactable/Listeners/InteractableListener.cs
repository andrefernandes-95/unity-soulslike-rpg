namespace AFV2
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class InteractableListener : MonoBehaviour
    {
        public HashSet<Interactable> captured = new();

        public void OnCaptured(Interactable interactable)
        {
            if (captured.Contains(interactable)) return;

            captured.Add(interactable);
            interactable.Enable();
        }

        public void Interact()
        {
            // Copy to list because OnLost modifies captured collection 
            foreach (var interactable in captured.ToList())
            {
                interactable.OnInteract(this);
                OnLost(interactable);
            }
        }

        public void OnLost(Interactable interactable)
        {
            if (!captured.Contains(interactable)) return;
            captured.Remove(interactable);
            interactable.Disable();
        }

    }
}