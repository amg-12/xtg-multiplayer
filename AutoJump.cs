using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Rewired;

namespace XtgMultiplayer
{
    [HarmonyPatch(typeof(GungeonActions))]
    static class AutoJump
    {
        class AutoData
        {
            public int _lastAutoDescendUpdateFrame = -1;
            public bool _lastAutoDescendIsPressed = false;
            public bool _currentAutoDescendIsPressed = false;
            public int _lastAutoAscendUpdateFrame = -1;
            public bool _lastAutoAscendIsPressed = false;
            public bool _currentAutoAscendIsPressed = false;

            static AutoData() { }
        }

        static readonly Dictionary<Player, AutoData> AutoDataByPlayer = new Dictionary<Player, AutoData>();

        static AutoData GetAutoData(Player inputPlayer)
        {
            if (!AutoDataByPlayer.TryGetValue(inputPlayer, out AutoData data))
                AutoDataByPlayer[inputPlayer] = data = new AutoData();
            return data;
        }

        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods() => new[]
        {
            AccessTools.Method(typeof(GungeonActions), "UpdateAutoDescendFrameData"),
            AccessTools.Method(typeof(GungeonActions), "AdditionalDropDown"),
            AccessTools.Method(typeof(GungeonActions), "AdditionalJumpDown"),
            AccessTools.Method(typeof(GungeonActions), "UpdateAutoAscendFrameData"),
            AccessTools.Method(typeof(GungeonActions), "AdditionalJumpUp"),
            AccessTools.Method(typeof(GungeonActions), "AdditionalJump"),
        };

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction>
            Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return new CodeMatcher(instructions, il)
                .MatchForward(useEnd: false,
                    new CodeMatch(i => i.operand is FieldInfo f
                                    && typeof(AutoData).GetField(f.Name) != null))
                .Repeat(matcher =>
                {
                    MethodInfo getAutoData = AccessTools.Method(typeof(AutoJump), "GetAutoData");
                    FieldInfo oldField = (FieldInfo)matcher.Instruction.operand;
                    FieldInfo newField = typeof(AutoData).GetField(oldField.Name);

                    if (matcher.Instruction.opcode == OpCodes.Ldsfld)
                    {
                        matcher.SetAndAdvance(OpCodes.Ldarg_0, null);
                        matcher.Insert(
                            new CodeInstruction(OpCodes.Call, getAutoData),
                            new CodeInstruction(OpCodes.Ldfld, newField));
                        matcher.Advance(2);
                    }
                    else // Stsfld
                    {
                        LocalBuilder tmp = il.DeclareLocal(newField.FieldType);
                        matcher.SetAndAdvance(OpCodes.Stloc, tmp);
                        matcher.Insert(
                            new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(OpCodes.Call, getAutoData),
                            new CodeInstruction(OpCodes.Ldloc, tmp),
                            new CodeInstruction(OpCodes.Stfld, newField));
                        matcher.Advance(4);
                    }
                })
                .InstructionEnumeration();
        }
    }
}