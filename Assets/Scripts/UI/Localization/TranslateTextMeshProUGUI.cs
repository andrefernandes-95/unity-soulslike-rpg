namespace AFV2
{
    using TMPro;
    using UnityEngine;

    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TranslateTextMeshProUGUI : MonoBehaviour
    {
        TextMeshProUGUI textMeshProUGUI => GetComponent<TextMeshProUGUI>();

        [Header("Other Languages")]
        [SerializeField][TextArea(minLines: 3, maxLines: 10)] public string portuguese;

        string originalText = "";

        void Awake()
        {
            originalText = textMeshProUGUI.text;
            if (Glossary.IsPortuguese()) textMeshProUGUI.text = portuguese;
        }

    }
}