namespace AFV2
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class ItemPickupInteractable : Interactable
    {
        [SerializeField] Item item;
        [SerializeField] int amount = 1;

        [Header("Graphic")]
        [SerializeField] MeshRenderer itemGraphic;
        [SerializeField] Image itemIcon;
        [SerializeField] TextMeshProUGUI itemLabel;

        public override void OnInteract(InteractableListener interactableListener)
        {
            CharacterApi characterApi = interactableListener.GetComponentInParent<CharacterApi>();
            if (characterApi == null) return;


            characterApi.characterInventory.AddItem(item);

            PlayConfirm();

            DisableInteractable();
        }

        public override void Enable()
        {
            base.Enable();

            if (item != null)
            {
                itemIcon.sprite = item.Sprite;
                itemLabel.text = item.DisplayName;
            }
        }

        void DisableInteractable()
        {
            Destroy(this.gameObject);
        }
    }
}
