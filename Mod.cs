using HarmonyLib;
using MelonLoader;

[assembly: MelonInfo(typeof(XtgMultiplayer.Mod), "Exit the Gungeon Multiplayer Mod", "0.0.1", "Amitai")]
[assembly: MelonGame("Dodge Roll", "Exit the Gungeon")]
namespace XtgMultiplayer
{
    public class Mod : MelonMod
    {
        [HarmonyPatch(typeof(GameManager), "Awake")]
        static class SpawnSecondPlayer
        {
            static bool isFirstRun;

            public static bool Prefix ()
            {
                isFirstRun = GameManager.IsFirstRun;
                return true;
            }

            public static void Postfix(GameManager __instance)
            {
                if (isFirstRun)
                {
                    Character char1 = CharacterManager.Instance.GetPrimaryPlayer();
                    Character char2 = Extensions.Instantiate<Character>(__instance.PlayerPrefabs[6], null);
                    PlayerController pc1 = CharacterManager.Instance.GetPrimaryPlayerController();
                    PlayerController pc2 = char2.gameObject.GetComponent<PlayerController>();
                    Controls.AssignSecondPlayer(char2.gameObject);
                    char2.transform.SetXY(char1.Center);
                    AccessTools.Field(typeof(PlayerController), "m_cachedRouteData").SetValue(pc2, pc1.RouteData);                    
                }
            }
        }

        [HarmonyPatch(typeof(GameManager), "Coroutine_Area")]
        static class FixNewAreaPause
        {
            public static bool Prefix(GameManager __instance)
            {
                foreach (Character character in Helper.GetAllPlayers())
                {
                    character.IsPaused = false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(GameManager), "HandleGameplayStateEndLogic")]
        static class DestroyPlayersAfterRun
        {
           public static bool Prefix(GameManager __instance)
           {
                Helper.DestroyAllPlayers();
                return true;
           }
        }

        [HarmonyPatch(typeof(PlayerController), "Start")]
        static class FixSecondPlayerSettingIdentity
        {
            static PlayerController.PlayableCharacters LastPlayedCharacter;

            public static bool Prefix()
            {
                LastPlayedCharacter = SaveData.Current.LastPlayedCharacter;
                return true;
            }

            public static void Postfix(PlayerController __instance)
            {
                if (__instance.InputPlayer.id != 0)
                {
                    SaveData.Current.LastPlayedCharacter = LastPlayedCharacter;
                }
            }
        }

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("hi");
        }
    }
}
