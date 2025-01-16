namespace AF
{
    using System.Collections;
    using System.Linq;
    using AF.Dialogue;
    using AF.UI;
    using UnityEngine;

    public class EV_SimpleMessage : EventBase
    {
        public Character character;

        [TextAreaAttribute(minLines: 10, maxLines: 20)]
        public string message;

        [Header("Responses")]
        public Response[] responses;

        [Header("Action Button")]
        public ActionButton actionButton;

        // Scene Refs
        UIDocumentDialogueWindow uIDocumentDialogueWindow;

        public override IEnumerator Dispatch()
        {
            // Only consider responses that are active - we hide responses based on composition of nested objects
            Response[] filteredResponses = responses.Where(response => response.gameObject.activeInHierarchy).ToArray();

            yield return GetUIDocumentDialogueWindow().DisplayMessage(
                character,
                message,
                filteredResponses,
                actionButton
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
