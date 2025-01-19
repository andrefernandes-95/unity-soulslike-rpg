
namespace AF
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UIElements;
    using System.Linq;
    using DG.Tweening;
    using UnityEngine.Events;
    using AF.Dialogue;
    using AF.Flags;
    using AF.UI;

    public class UIDocumentDialogueWindow : MonoBehaviour
    {
        [Header("UI Documents")]
        public UIDocument uiDocument;
        public VisualTreeAsset messageBodyItem;
        public VisualTreeAsset dialogueChoiceItem;

        VisualElement root;
        VisualElement dialogueChoicePanel;
        VisualElement tutorialMessageRoot;

        // Flags
        [HideInInspector] public bool hasFinishedTypewriter = false;

        [Header("Settings")]
        public float textDelay = 0.05f;

        [Header("Components")]
        public StarterAssetsInputs inputs;
        public Soundbank soundbank;
        public CursorManager cursorManager;
        public PlayerManager playerManager;
        public FlagsDatabase flagsDatabase;

        [Header("Unity Events")]
        public UnityEvent onEnableEvent;
        public UnityEvent onDisableEvent;

        // Internal
        Label actorNameLabel, actorTitleLabel, messageTextLabel;
        VisualElement actorContainer;
        VisualElement actorSprite;
        VisualElement actorInfoContainer;

        [Header("Input")]
        VisualElement continueActionButtonContainer;
        public StarterAssetsInputs starterAssetsInputs;
        public ActionButton continueButton;

        private void Awake()
        {
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            this.root = uiDocument.rootVisualElement;
            this.dialogueChoicePanel = this.root.Q<VisualElement>("ChoiceContainer");
            this.actorNameLabel = this.root.Q<Label>("ActorName");
            this.actorTitleLabel = this.root.Q<Label>("ActorTitle");
            this.tutorialMessageRoot = this.root.Q<VisualElement>("TutorialMessageRoot");
            this.actorContainer = this.root.Q<VisualElement>("ActorContainer");
            this.actorSprite = this.root.Q<VisualElement>("ActorSprite");
            this.actorInfoContainer = this.root.Q<VisualElement>("ActorInfoContainer");
            this.messageTextLabel = this.root.Q<Label>("MessageText");

            continueActionButtonContainer = this.root.Q<VisualElement>("ContinueButtonContainer");
            continueActionButtonContainer.Clear();

            continueActionButtonContainer.Add(continueButton.GetKey(starterAssetsInputs));

            onEnableEvent?.Invoke();

            playerManager.uIDocumentPlayerHUDV2.HideHUD();
        }

        private void OnDisable()
        {
            onDisableEvent?.Invoke();
            playerManager.uIDocumentPlayerHUDV2.ShowHUD();
        }

        public IEnumerator DisplayMessage(
                    Character character,
                    string message,
                    Response[] responses
                )
        {
            gameObject.SetActive(true);

            ShowMessage(character, message);

            // Clone responses to prevent mutation
            Response[] clonedResponses = responses.ToArray();

            // Wait until the interact button is not pressed
            yield return new WaitUntil(() => inputs.interact == false);

            // Handle typewriter effect or immediate display
            while (hasFinishedTypewriter == false)
            {
                if (inputs.interact)
                {
                    ShowAllTextAtOnce(message);
                }

                yield return null;
            }

            // Wait until the interact button is not pressed again
            yield return new WaitUntil(() => inputs.interact == false);
            yield return new WaitUntil(() => inputs.interact == true);

            // Display responses (if any)
            if (clonedResponses != null && clonedResponses.Length > 0)
            {
                yield return ShowResponses(clonedResponses);
            }

            // Wait until the interact button is not pressed again before moving forward
            yield return new WaitUntil(() => inputs.interact == false);

            gameObject.SetActive(false);
        }

        public IEnumerator DisplayTutorialMessage(
            Character character,
            TutorialMessage[] tutorialMessages,
            Response[] responses
        )
        {
            gameObject.SetActive(true);
            ShowTutorialMessage(tutorialMessages);
            // Clone responses to prevent mutation

            Response[] clonedResponses = responses.ToArray();

            // Wait until the interact button is not pressed
            yield return new WaitUntil(() => inputs.interact == false);

            // Handle typewriter effect or immediate display
            while (hasFinishedTypewriter == false)
            {
                if (inputs.interact)
                {
                    ShowAllTextAtOnce(tutorialMessages[0].message);
                }
                else
                {
                    hasFinishedTypewriter = true;
                }

                yield return null;
            }

            // Wait until the interact button is not pressed again
            yield return new WaitUntil(() => inputs.interact == false);
            yield return new WaitUntil(() => inputs.interact == true);

            // Display responses (if any)
            if (clonedResponses != null && clonedResponses.Length > 0)
            {
                yield return ShowResponses(clonedResponses);
            }

            // Wait until the interact button is not pressed again before moving forward
            yield return new WaitUntil(() => inputs.interact == false);

            gameObject.SetActive(false);
        }


        void SetupMessage(Character character)
        {
            hasFinishedTypewriter = false;

            soundbank.PlaySound(soundbank.uiDialogue);

            DOTween.To(
                  () => root.contentContainer.style.opacity.value,
                  (value) => root.contentContainer.style.opacity = value,
                  1,
                  .05f
            );

            // Set the starting position below the screen
            Vector3 startPosition = new Vector3(root.contentContainer.transform.position.x, root.contentContainer.transform.position.y - 10, root.contentContainer.transform.position.z);

            // Set the ending position (original position)
            Vector3 endPosition = root.contentContainer.transform.position;

            // Tween the position from the starting position to the ending position
            DOTween.To(() => startPosition, position => root.contentContainer.transform.position = position, endPosition, 0.5f);

            dialogueChoicePanel.Clear();
            dialogueChoicePanel.style.display = DisplayStyle.None;

            if (character != null && string.IsNullOrEmpty(character.name) == false)
            {
                actorNameLabel.style.display = DisplayStyle.Flex;
                actorNameLabel.text = character.isPlayer ? playerManager.playerAppearance.GetPlayerName() : character.GetCharacterName();
                actorInfoContainer.style.display = DisplayStyle.Flex;
            }
            else
            {
                actorInfoContainer.style.display = DisplayStyle.None;
                actorNameLabel.style.display = DisplayStyle.None;
            }

            if (character != null && string.IsNullOrEmpty(character.title) == false)
            {
                actorTitleLabel.style.display = DisplayStyle.Flex;
                actorTitleLabel.text = character.GetCharacterTitle();
            }
            else
            {
                actorTitleLabel.style.display = DisplayStyle.None;
            }

            if (character != null && character.avatar != null)
            {
                actorContainer.style.display = DisplayStyle.Flex;
                actorSprite.style.backgroundImage = new StyleBackground(
                                   character.isPlayer ? playerManager.playerAppearance.GetPlayerPortrait() : character.avatar);
                actorSprite.style.display = DisplayStyle.Flex;
            }
            else
            {
                actorContainer.style.display = DisplayStyle.None;
                actorSprite.style.display = DisplayStyle.None;
            }
        }

        private void ShowTutorialMessage(TutorialMessage[] tutorialMessages)
        {
            messageTextLabel.text = "";

            SetupMessage(null);

            tutorialMessageRoot.Clear();

            foreach (var tutorialMessage in tutorialMessages)
            {
                VisualElement messageBodyCopy = messageBodyItem.CloneTree();
                VisualElement actionButtonContainer = messageBodyCopy.Q<VisualElement>("ActionButtonContainer");

                actionButtonContainer.Clear();
                actionButtonContainer.style.display = tutorialMessages.Length > 0 ? DisplayStyle.Flex : DisplayStyle.None;

                if (tutorialMessage.actionButton != null)
                {
                    VisualElement actionButtonElement = tutorialMessage.actionButton.GetKey(starterAssetsInputs);
                    actionButtonElement.Q<Label>("Description").style.display = DisplayStyle.None;
                    actionButtonContainer.Add(actionButtonElement);
                }

                messageBodyCopy.Q<Label>("TutorialMessageText").text = tutorialMessage.message;

                tutorialMessageRoot.Add(messageBodyCopy);
            }

        }

        private void ShowMessage(Character character, string message)
        {
            SetupMessage(character);

            StartCoroutine(Typewrite(message, messageTextLabel));
        }

        public IEnumerator Typewrite(string dialogueText, Label messageTextLabel)
        {
            for (int i = 0; i < dialogueText.Length + 1; i++)
            {
                var letter = dialogueText.Substring(0, i);
                messageTextLabel.text = letter;
                yield return new WaitForSeconds(textDelay);
            }

            hasFinishedTypewriter = true;
        }

        public void ShowAllTextAtOnce(string dialogueText)
        {
            StopAllCoroutines();
            hasFinishedTypewriter = true;
            messageTextLabel.text = dialogueText;
        }

        public IEnumerator ShowResponses(Response[] responses)
        {
            dialogueChoicePanel.Clear();
            dialogueChoicePanel.style.display = DisplayStyle.None;

            if (responses.Length <= 0)
            {
                dialogueChoicePanel.style.display = DisplayStyle.None;
                yield break;
            }

            dialogueChoicePanel.style.display = DisplayStyle.Flex;

            Response selectedResponse = null;
            Button elementToFocus = null;

            foreach (var response in responses)
            {
                var newDialogueChoiceItem = dialogueChoiceItem.CloneTree();
                newDialogueChoiceItem.Q<Button>().text = response.text;

                UIUtils.SetupButton(newDialogueChoiceItem.Q<Button>(), () =>
                {
                    cursorManager.HideCursor();
                    selectedResponse = response;
                    response.onResponseSelected?.Invoke();
                }, soundbank);

                elementToFocus ??= newDialogueChoiceItem.Q<Button>();

                dialogueChoicePanel.Add(newDialogueChoiceItem);
            }

            cursorManager.ShowCursor();
            elementToFocus?.Focus();
            playerManager.thirdPersonController.LockCameraPosition = true;

            yield return new WaitUntil(() => selectedResponse != null);

            playerManager.thirdPersonController.LockCameraPosition = false;

            selectedResponse.AwardReputation(flagsDatabase, playerManager.playerReputation);

            // Use Sub Events Option
            if (selectedResponse.subEventPage != null)
            {
                EventBase[] choiceEvents = selectedResponse.subEventPage.GetComponents<EventBase>();

                if (choiceEvents.Length > 0)
                {
                    foreach (EventBase subEvent in choiceEvents)
                    {
                        yield return subEvent.Dispatch();
                    }
                }
            }
            else if (string.IsNullOrEmpty(selectedResponse.reply) == false)
            {
                yield return DisplayMessage(selectedResponse.replier, selectedResponse.reply, new Response[] { });
            }

            selectedResponse.onResponseFinished?.Invoke();
        }
    }
}
