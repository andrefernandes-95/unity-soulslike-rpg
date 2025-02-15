namespace AFV2
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Boot : ArmorBase
    {
        [Header("Footstep")]
        public List<AudioClip> footstepOverrides = new();
    }
}
