using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MelonLoader;

[assembly: MelonInfo(typeof(XtgMultiplayer.Mod), "Exit the Gungeon Multiplayer Mod", "0.0.1", "Amitai")]
[assembly: MelonGame("Dodge Roll", "Exit the Gungeon")]
namespace XtgMultiplayer
{
    public class Mod : MelonMod
    {
        static int nextPlayerIndex;

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
                    nextPlayerIndex = 1;
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

        [HarmonyPatch(typeof(Character), "Awake")]
        static class SetPlayerIndex
        {
            public static bool Prefix(Character __instance)
            {
                if (__instance.Type == CharacterType.Player)
                {
                    __instance.PlayerIndex = nextPlayerIndex;
                    nextPlayerIndex = 0;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(CharacterManager), "RegisterCharacter")]
        static class OrderPlayers
        {
            public static void Postfix(CharacterManager __instance, ref Character character)
            {
                if (character.Type == CharacterType.Player)
                {
                    List<Character> players = __instance.AliveCharacters[(int)CharacterType.Player];
                    players = players.OrderBy(p => p.PlayerIndex).ToList();
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
