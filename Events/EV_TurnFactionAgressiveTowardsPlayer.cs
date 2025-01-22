namespace AF
{
    using System.Collections;
    using System.Linq;
    using AF.Characters;
    using UnityEngine;

    public class EV_TurnFactionAgressiveTowardsPlayer : EventBase
    {
        public Combatant faction;

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
                        && aiCharacter.combatant.friendlies != null
                        && aiCharacter.combatant.friendlies.Length > 0
                        && aiCharacter.combatant.friendlies.Contains(faction))
                    {
                        aiCharacter.targetManager.SetPlayerAsTarget();
                    }
                }
            }

            yield return null;
        }
    }

}
