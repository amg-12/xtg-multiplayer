using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace XtgMultiplayer
{
    static class Camera
    {
        static bool updatingCamera = false;

        [HarmonyPatch(typeof(GameSceneManager), "UpdateCamera")]
        static class SetUpdatingCamera
        {
            public static bool Prefix()
            {
                updatingCamera = true;
                return true;
            }

            public static void Postfix()
            {
                updatingCamera = false;
            }
        }

        [HarmonyPatch(typeof(Character), "Center", MethodType.Getter)]
        static class PositionCamera
        {
            public static bool Prefix(ref Vector2 __result)
            {
                if (updatingCamera && GameManager.Instance != null)
                {
                    List<Vector2> centers = Helper.GetAllLivingPlayers().Select(p => p.transform.GetXY()).ToList();
                    if (centers.Count > 0)
                    {
                        __result = new Vector2(
                            centers.Average(c => c.x),
                            centers.Average(c => c.y)
                        );
                        return false;
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Room), "HandleArrival")]
        static class ChangeOffsetInShop
        {
            public static void Postfix(Room __instance)
            {
                if (__instance.IsShop)
                {
                    __instance.AdditionalCameraOffset.Set(0f, 0.25f);
                }
            }
        }
    }
}
