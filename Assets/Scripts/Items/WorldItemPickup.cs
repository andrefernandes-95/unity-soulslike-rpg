namespace AFV2
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [RequireComponent(typeof(SphereCollider))]
    public class WorldItemPickup : MonoBehaviour
    {
        [SerializeField] Item itemToPickup;
        [SerializeField] int amount = 1;

        [Header("Events")]
        public UnityEvent onPickup;

        [Header("UI")]
        [SerializeField] Canvas pickupCanvas;
        [SerializeField] Image itemImage;
        [SerializeField] TextMeshProUGUI itemName;
        [SerializeField] TextMeshProUGUI itemDescription;
        [SerializeField] TextMeshProUGUI itemValue;
        [SerializeField] TextMeshProUGUI itemWeight;

        [Header("Components")]
        InputListener inputListener;
        PlayerController _playerController;
        CharacterApi pickupReceiver;

        bool CanPickup = true;

        void Awake()
        {
            HidePickupCanvas();

            inputListener = FindFirstObjectByType<InputListener>(FindObjectsInactive.Include);

            if (inputListener != null)
            {
                inputListener.onInteract.AddListener(HandleItemPickupOnInput);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (!CanPickup)
            {
                return;
            }

            if (other.TryGetComponent<CharacterApi>(out var characterApi))
            {
                this.pickupReceiver = characterApi;

                ShowPickupCanvas();
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<CharacterApi>(out var characterApi) && pickupReceiver != null && pickupReceiver.IsCharacter(characterApi))
            {
                HidePickupCanvas();
                pickupReceiver = null;
            }
        }

        void ShowPickupCanvas()
        {
            pickupCanvas.gameObject.SetActive(true);

            if (itemToPickup != null)
            {
                itemName.text = itemToPickup.DisplayName;

                if (amount > 1)
                {
                    itemName.text += $" (x{amount})";
                }

                itemDescription.text = itemToPickup.Description;
                itemValue.text = (itemToPickup.Value * amount).ToString();
                itemWeight.text = (itemToPickup.Weight * amount).ToString();
                itemImage.sprite = itemToPickup.Sprite;
            }
        }

        void HidePickupCanvas()
        {
            pickupCanvas.gameObject.SetActive(false);
        }

        void HandleItemPickupOnInput()
        {
            if (
                CanPickup
                && pickupReceiver != null
                && pickupReceiver.gameObject.CompareTag("Player")
                && TryGetPlayerController(out PlayerController playerController)
                && playerController.CanControlPlayer()
            )
            {
                for (int i = 0; i < amount; i++)
                {
                    pickupReceiver.characterInventory.AddItem(itemToPickup);
                }

                onPickup?.Invoke();
                CanPickup = false;
                HidePickupCanvas();
            }
        }

        public void SetWorldItem(Item item, int amount)
        {
            this.itemToPickup = item;
            this.amount = amount;
        }

        bool TryGetPlayerController(out PlayerController playerController)
        {
            if (_playerController == null)
            {
                _playerController = FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);
            }

            playerController = _playerController;

            return playerController != null;
        }
    }
}
