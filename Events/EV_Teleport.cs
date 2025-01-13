namespace AF
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class EV_Teleport : EventBase
    {
        public SceneTeleport currentSceneTeleports;
        public Location targetLocation;

        // Scene Refs
        TeleportManager teleportManager;

        public override IEnumerator Dispatch()
        {
            yield return null;
            Teleport();
        }

        public void Teleport()
        {
            // Find the first match where the key's name is the destination scene
            var match = currentSceneTeleports.teleports
                .FirstOrDefault(entry => entry.Key.name == targetLocation.name);

            // Check if match is valid and match.Value is not null
            if (!match.Equals(default(KeyValuePair<Location, string>)) && match.Value != null)
            {
                // Use TeleportManager to teleport to the destination scene
                GetTeleportManager().Teleport(targetLocation.name, match.Value);
            }
        }

        TeleportManager GetTeleportManager()
        {
            if (teleportManager == null)
            {
                teleportManager = FindAnyObjectByType<TeleportManager>(FindObjectsInactive.Include);
            }

            return teleportManager;
        }
    }
}
