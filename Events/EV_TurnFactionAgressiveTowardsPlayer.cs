namespace AF
{
    using System.Collections;
    using System.Linq;
    using AF.Characters;
    using UnityEngine;

    public class EV_TurnFactionAgressiveTowardsPlayer : EventBase
    {
        public CharacterFaction faction;

        public override IEnumerator Dispatch()
        {
            CharacterBaseManager[] allCharactersBelongingToFactionInScene = FindObjectsByType<CharacterBaseManager>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            if (allCharactersBelongingToFactionInScene.Length > 0)
            {
                foreach (var characterInScene in allCharactersBelongingToFactionInScene)
                {
                    if (
                        characterInScene is CharacterManager aiCharacter
                        && aiCharacter.targetManager != null
                        && aiCharacter.combatant.characterFactions != null
                        && aiCharacter.combatant.characterFactions.Length > 0
                        && aiCharacter.combatant.characterFactions.Contains(faction))
                    {
                        aiCharacter.targetManager.SetPlayerAsTarget();
                    }
                }
            }

            yield return null;
        }
    }

}
