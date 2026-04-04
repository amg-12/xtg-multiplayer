using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace XtgMultiplayer
{
    static class ElevatorFlying
    {
        static FieldInfo carryField = AccessTools.Field(typeof(Elevator_Flying), "_carriedPlayer");
        static Elevator_Flying secondTurtle;

        static void Carry(Character player, Elevator_Flying turtle)
        {
            if (secondTurtle == null)
            {
                secondTurtle = Object.Instantiate(turtle);
                secondTurtle.transform.SetParent(turtle.transform.parent);
            }
            secondTurtle.gameObject.SetActive(true);
            carryField.SetValue(secondTurtle, player);
            player.SetFlying(true, "Elevator_Flying");
        }

        [HarmonyPatch(typeof(Elevator_Flying), "CarryPlayer")] 
        static class PatchCarryPlayer
        {
            public static bool Prefix(Elevator_Flying __instance)
            {
                return (Character)carryField.GetValue(__instance) == null;
            }

            public static void Postfix(Elevator_Flying __instance)
            {
                List<Character> players = Helper.GetAllLivingPlayers();
                foreach (Character player in players)
                {
                    if (player.MovingType != CharacterMovingType.Flying)
                    {
                        Carry(player, __instance);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Elevator_Flying), "DropPlayer")]
        static class DropPlayer
        {
            public static void Postfix(Elevator_Flying __instance)
            {
                if (secondTurtle != null && secondTurtle != __instance)
                {
                    secondTurtle.DropPlayer();
                    secondTurtle.gameObject.SetActive(false);
                }
            }
        }

        [HarmonyPatch(typeof(Elevator_Flying), "UpdateTurtlePosition")]
        static class PatchTurtlePosition
        {
            public static void Postfix(Elevator_Flying __instance)
            {
                Character player = (Character)carryField.GetValue(__instance);
                __instance.SpaceTurtleSprite.gameObject.SetActive(player.isActiveAndEnabled);
            }

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                MethodInfo getPrimaryPlayer = AccessTools.Method(typeof(CharacterManager), "GetPrimaryPlayer");
                FieldInfo carriedPlayer = AccessTools.Field(typeof(Elevator_Flying), "_carriedPlayer");
                return new CodeMatcher(instructions)
                    .MatchForward(false, new CodeMatch(OpCodes.Callvirt, getPrimaryPlayer))
                    .Set(OpCodes.Ldfld, carriedPlayer)
                    .Advance(-1)
                    .Set(OpCodes.Ldarg_0, null)
                    .InstructionEnumeration();
            }
        }
    }
}
