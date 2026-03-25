using HarmonyLib;

namespace XtgMultiplayer
{
    static class Music
    {
        static bool BypassFlushAudio;

        [HarmonyPatch(typeof(PlayerController), "OnHPChanged")]
        static class SetBypassFlushAudio
        {
            public static bool Prefix()
            {
                BypassFlushAudio = true;
                return true;
            }

            public static void Postfix()
            {
                BypassFlushAudio = false;
            }
        }

        [HarmonyPatch(typeof(AudioManager), "FlushAudio")]
        static class FixMusicStoppingOnDeath
        {
            public static bool Prefix()
            {
                return !BypassFlushAudio || Helper.GetAllLivingPlayers().Count == 0;
            }
        }
    }
}
