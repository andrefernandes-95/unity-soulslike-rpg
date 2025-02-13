namespace AFV2
{
    public enum CombatDecision
    {
        NONE,
        RIGHT_LIGHT_ATTACK,
        LEFT_LIGHT_ATTACK,
        HEAVY_ATTACK,
        RIGHT_AIR_ATTACK,
        LEFT_AIR_ATTACK,

        // AI Related, Player will be able to perform these on demand
        DODGE,
        STRAFE,
        GUARD,
        PARRY,
    }
}
