namespace AF
{
    using System.Collections;
    using System.Linq;
    using AF.Dialogue;
    using AF.UI;
    using UnityEngine;
    using UnityEngine.Localization;

    [System.Serializable]
    public class TutorialMessage
    {
        [TextAreaAttribute(minLines: 10, maxLines: 20)]
        public string message;
        public ActionButton actionButton;
    }

    public class EV_TutorialMessage : EventBase
    {
        [HideInInspector] public Character character;

        public LocalizedString tutorialName;
        public TutorialMessage[] tutorialMessages;

        [Header("Responses")]
        [HideInInspector] public Response[] responses;

        // Scene Refs
        UIDocumentDialogueWindow uIDocumentDialogueWindow;

        public override IEnumerator Dispatch()
        {
            // Only consider responses that are active - we hide responses based on composition of nested objects
            Response[] filteredResponses = responses.Where(response => response.gameObject.activeInHierarchy).ToArray();

            yield return GetUIDocumentDialogueWindow().DisplayTutorialMessage(
                character,
                tutorialName,
                tutorialMessages,
                filteredResponses
                );
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        UIDocumentDialogueWindow GetUIDocumentDialogueWindow()
        {
            if (uIDocumentDialogueWindow == null)
            {
                uIDocumentDialogueWindow = FindAnyObjectByType<UIDocumentDialogueWindow>(FindObjectsInactive.Include);
            }

            return uIDocumentDialogueWindow;
        }
    }
}
