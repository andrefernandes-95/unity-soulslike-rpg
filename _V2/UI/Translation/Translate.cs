namespace AFV2
{
    using AF;
    using TMPro;
    using UnityEngine;

    [RequireComponent(typeof(TextMeshProUGUI))]
    public class Translate : MonoBehaviour
    {
        TextMeshProUGUI textMeshProUGUI => GetComponent<TextMeshProUGUI>();

        [SerializeField][TextArea(minLines: 3, maxLines: 10)] public string english;
        [SerializeField][TextArea(minLines: 3, maxLines: 10)] public string portuguese;

        void Awake()
        {
            if (Glossary.IsPortuguese()) textMeshProUGUI.text = portuguese;
            else textMeshProUGUI.text = english;
        }

    }
}