using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace XtgMultiplayer
{
    static class Music
    {
        public static void MaybeFlushAudio()
        {
            if (Helper.GetAllLivingPlayers().Count == 0)
            {
                AudioManager.FlushAudio();
            }
        }

        static IEnumerable<CodeInstruction> FlushAudioTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo flushAudio = AccessTools.Method(typeof(AudioManager), "FlushAudio");
            MethodInfo maybeFlush = AccessTools.Method(typeof(Music), "MaybeFlushAudio");
            return new CodeMatcher(instructions)
                .MatchForward(false, new CodeMatch(OpCodes.Call, flushAudio))
                .SetOperandAndAdvance(maybeFlush)
                .InstructionEnumeration();
        }

        [HarmonyPatch(typeof(PlayerController), "OnHPChanged")]
        static class FixAudioStop
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                return FlushAudioTranspiler(instructions);
            }
        }

        [HarmonyPatch(typeof(PlayerController), "OnArmorChanged")]
        static class FixAudioStopRobot
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                return FlushAudioTranspiler(instructions);
            }
        }
    }
}
