using HarmonyLib;
using MelonLoader;

namespace XtgMultiplayer
{
    internal class Debug
    {
        [HarmonyPatch(typeof(PlayerController), "OnHPChanged")]
        static class DebugHP
        {
            public static bool Prefix(PlayerController __instance, ref float lastHP, ref float currentHP)
            {
                MelonLogger.Msg($"{__instance.name}, {lastHP}, {currentHP}, {__instance.gameObject.GetComponent<Character>().IsDead}");
                return true;
            }
        }

        [HarmonyPatch(typeof(Character), "OnCharacterDead")]
        static class DebugCharacterDead
        {
            public static bool Prefix(Character __instance)
            {
                if (__instance.Type == CharacterType.Player)
                {
                    MelonLogger.Msg($"{__instance.name}, {__instance.IsDead}");
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Character), "Awake")]
        static class DebugCharacterDead2
        {
            public static void Postfix(Character __instance)
            {
                if (__instance.Type == CharacterType.Player)
                {
                    __instance.OnDead += (_ => MelonLogger.Msg("OnDead"));
                }
            }
        }
    }
}
