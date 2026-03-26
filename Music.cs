using HarmonyLib;

namespace XtgMultiplayer
{
    static class Music
    {
        static bool bypassFlushAudio = false;

        [HarmonyPatch(typeof(PlayerController), "OnHPChanged")]
        static class SetBypassFlushAudio
        {
            public static bool Prefix()
            {
                bypassFlushAudio = true;
                return true;
            }

            public static void Postfix()
            {
                bypassFlushAudio = false;
            }
        }

        [HarmonyPatch(typeof(AudioManager), "FlushAudio")]
        static class FixMusicStoppingOnDeath
        {
            public static bool Prefix()
            {
                return !bypassFlushAudio || Helper.GetAllLivingPlayers().Count == 0;
            }
        }
    }
}
