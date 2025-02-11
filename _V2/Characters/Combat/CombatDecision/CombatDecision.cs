namespace AFV2
{
    public enum CombatDecision
    {
        NONE,
        RIGHT_LIGHT_ATTACK,
        LEFT_LIGHT_ATTACK,
        HEAVY_ATTACK,
        AIR_ATTACK,

        // AI Related, Player will be able to perform these on demand
        DODGE,
        STRAFE,
        GUARD,
        PARRY,
    }
}
