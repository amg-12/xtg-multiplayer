using System;
using HarmonyLib;
using Rewired;

namespace XtgMultiplayer
{
    static class Interacting
    {
        static Character lastInteractor;

        static PlayerController LastPlayerController()
        { 
            return lastInteractor.CharacterController as PlayerController;
        }

        static Player LastInputPlayer()
        {
            return LastPlayerController().InputPlayer;
        }

        [HarmonyPatch(typeof(InteractiveObject), "Interact")]
        static class SetLastInteractor
        {
            public static bool Prefix(ref Character character)
            {
                lastInteractor = character;
                AccessTools.Field(typeof(UI_Dialogue), "m_player")
                    .SetValue(UI_Dialogue.Instance, LastInputPlayer());
                return true;
            }
        }

        [HarmonyPatch(typeof(PlayerController), "Update")]
        static class PatchOpenMenu
        {
            public static bool Prefix(PlayerController __instance)
            {
                if (__instance.InputPlayer.GetButtonDown(GungeonActions.Start) &&
                    GungearManager.IsOpenable)
                {
                    lastInteractor = __instance.Owner;
                    AccessTools.Field(typeof(GungearManager), "m_cachedUIPlayer")
                        .SetValue(GungearManager.Instance, LastPlayerController());
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(CharacterManager), "GetPrimaryPlayer")]
        static class PatchPrimaryPlayer
        {
            public static void Postfix(ref Character __result)
            {
                if (lastInteractor != null)
                {
                    __result = lastInteractor;
                }
            }
        }

        [HarmonyPatch(typeof(CharacterManager), "GetPrimaryPlayerController")]
        static class PatchPrimaryPlayerController
        {
            public static void Postfix(ref PlayerController __result)
            {
                if (lastInteractor != null)
                {
                    __result = LastPlayerController();
                }
            }
        }

        [HarmonyPatch(typeof(ReInput.PlayerHelper), "GetPlayer", new Type[] { typeof(int) })]
        static class PatchInputPlayer
        {
            public static void Postfix(ref Player __result)
            {
                if (lastInteractor != null)
                {
                    __result = LastInputPlayer();
                }
            }
        }
    }
}
