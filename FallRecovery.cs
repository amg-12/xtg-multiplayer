using HarmonyLib;

namespace XtgMultiplayer
{
    static class FallRecovery
    {
        public static void EndRecovery(Character character)
        {
            PlayerController pc = character.gameObject.GetComponent<PlayerController>();
            pc.IsReturning = false;
            character.VelocityY = 0;
            character.SetFlying(false, "FallRecover");
            character.SetIncorporeality(0);
        }

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
                __result = __result && !__instance.IsReturning;
            }
        }
    }
}
