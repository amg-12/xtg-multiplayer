using HarmonyLib;

namespace XtgMultiplayer
{
    static class FallRecovery
    {
        [HarmonyPatch(typeof(SpaceTurtle_FallRecover), "Initialize")]
        static class UnblockOtherPlayerWhenRecovering
        {
            public static bool Prefix(ref Character carrying)
            {
                PlayerController.InputBlockStackCount--;
                return true;
            }
        }

        [HarmonyPatch(typeof(PlayerController), "IsInputAvailable", MethodType.Getter)]
        static class BlockThisPlayerWhenRecovering
        {
            public static void Postfix(PlayerController __instance, ref bool __result)
            {
                __result = __result || __instance.IsReturning;
            }
        }
    }
}
