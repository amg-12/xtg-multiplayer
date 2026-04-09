using System;
using HarmonyLib;
using UnityEngine;

namespace XtgMultiplayer
{
    static class Doors
    {
        static bool isExitingDoor = false;

        [HarmonyPatch(typeof(DoorBase), "ExitTheDoor")]
        static class SetExiting
        {
            public static bool Prefix()
            {
                isExitingDoor = true;
                return true;
            }
        }

        //[HarmonyPatch(typeof(DoorBase), "OnEnterAnimationCompleted")]
        [HarmonyPatch(typeof(Character), "Show", new Type[] { typeof(Vector2) })]
        static class UnsetExiting
        {
            public static void Postfix(Character __instance)
            {
                if (__instance.Type == CharacterType.Player)
                {
                    isExitingDoor = false;
                }
            }
        }

        //[HarmonyPatch(typeof(PlayerController), "Interact", MethodType.Getter)]
        [HarmonyPatch(typeof(InteractiveObject), "InteractionEnabled", MethodType.Getter)]
        static class StopInteraction
        {
            public static void Postfix(ref bool __result)
            {
                __result = __result && !isExitingDoor;
            }
        }

    }
}
