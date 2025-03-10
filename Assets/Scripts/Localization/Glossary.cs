namespace AFV2
{

    public static class Glossary
    {
        public static string Get(string key)
        {
            return "";
        }

        public static bool IsPortuguese()
        {
            return false;
        }


        public static string AND()
        {
            if (IsPortuguese()) { return "e"; }
            return "and";
        }

        public static string SMALL()
        {
            if (IsPortuguese()) { return "Pequena"; }
            return "Small";
        }

        public static string GREAT()
        {
            if (IsPortuguese()) { return "Grande"; }
            return "Great";
        }

        public static string COMBINE_UP_TO_X_INGREDIENTS_TO_CREATE_A_UNIQUE_ITEM(int maxSlots)
        {
            if (IsPortuguese()) { return $"Combina até {maxSlots} ingredientes para criar um item novo"; }
            return $"Combine up to {maxSlots} ingredients to create a new item";
        }

    }
}
