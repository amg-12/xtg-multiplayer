using HarmonyLib;
using UnityEngine;

namespace XtgMultiplayer
{
    static class ElevatorDragun
    {
        [HarmonyPatch(typeof(Elevator_Bouncing), "LateUpdateBoss")]
        static class HidePlayers
        {
            public static bool Prefix(Elevator_Bouncing __instance)
            {
                bool ready = (bool)AccessTools.Field(typeof(Elevator_Bouncing), "m_readyToEnterStage").GetValue(__instance);
                bool started = (bool)AccessTools.Field(typeof(Elevator_Bouncing), "m_hasStartedDragunIntro").GetValue(__instance);
                if (!ready && !started)
                {
                    foreach (Character player in Helper.GetAllPlayers())
                    {
                        player.Hide();
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Elevator_Bouncing), "ReadyToEnterStage", MethodType.Setter)]
        static class ShowPlayers
        {
            public static void Postfix(Elevator_Bouncing __instance)
            {
                Helper.PositionPlayers(__instance.transform.position + new Vector3(0f, 0.14f, 0f));
            }
        }
    }
}
