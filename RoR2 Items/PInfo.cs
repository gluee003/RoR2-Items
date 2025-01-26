using HarmonyLib;

namespace RoR2_Items
{
    public static class PInfo
    {
        // each loaded plugin needs to have a unique GUID. usually author+generalCategory+Name is good enough
        public const string GUID = "gluee.lbol.ror2items";
        public const string Name = "RoR2 Items";
        public const string version = "0.0.1";
        public static readonly Harmony harmony = new Harmony(GUID);

    }
}
