namespace AFV2
{
    using UnityEngine;

    public class AssetCredits : MonoBehaviour
    {
        [TextArea(minLines: 5, maxLines: 10)] public string description;
        public string assetURL;
    }
}
