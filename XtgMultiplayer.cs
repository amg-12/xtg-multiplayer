using System.Linq;
using HarmonyLib;
using MelonLoader;

[assembly: MelonInfo(typeof(XtgMultiplayer.XtgMultiplayer), "Exit the Gungeon Multiplayer Mod", "0.0.1", "Amitai")]
[assembly: MelonGame("Dodge Roll", "Exit the Gungeon")]
namespace XtgMultiplayer
{
    public class XtgMultiplayer : MelonMod
    {
        [HarmonyPatch(typeof(GameManager), "Awake")]
        static class SpawnSecondPlayer
        {
            static bool IsFirstRun;

            public static bool Prefix ()
            {
                IsFirstRun = GameManager.IsFirstRun;
                return true;
            }

            public static void Postfix(GameManager __instance)
            {
                if (IsFirstRun)
                {
                    Character chara = Extensions.Instantiate<Character>(__instance.PlayerPrefabs[6], null);
                    chara.transform.SetXY(0f, -0.49f);
                    UnityEngine.Object.DontDestroyOnLoad(chara);
                    Controls.AssignSecondPlayer(chara.gameObject);
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

        [HarmonyPatch(typeof(Character), "OnCharacterDead")]
        static class HandleDeath
        {
            public static bool Prefix(Character __instance)
            {
                if (__instance.Type == CharacterType.Player)
                {
                    Helper.KillPlayer(__instance);
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(CharacterManager), "GetNearestCharacter")]
        static class ExcludeDeadTarget
        {
            public static bool Prefix(ref Character excludeTarget, ref CharacterType type)
            {
                return Helper.ExcludeDeadTarget(ref excludeTarget, type);
            }
        }

        [HarmonyPatch(typeof(CharacterManager), "GetNearestCharacterInScreen")]
        static class ExcludeDeadTargetInScreen
        {
            public static bool Prefix(ref Character excludeTarget, ref CharacterType type)
            {
                return Helper.ExcludeDeadTarget(ref excludeTarget, type);
            }
        }

        [HarmonyPatch(typeof(CharacterEventController_BossDead), "Coroutine_Event")]
        static class ReviveAfterBoss
        {
            public static bool Prefix() {
                Helper.ReviveAllPlayers();
                return true;
            }
        }

        [HarmonyPatch(typeof(Box_Room), "SpawnItem")]
        static class ReviveFromChest
        {
            public static bool Prefix(Box_Room __instance)
            {
                if (Helper.GetAllPlayers().Any(c => c.IsDead))
                {
                    Helper.ReviveAllPlayers(__instance.transform.GetXY());
                    return false;
                } else
                {
                    return true;
                }
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

        [HarmonyPatch(typeof(MobilePreIntroController), "Start")]
        static class InitialiseRewired
        {
            public static bool Prefix()
            {
                Controls.InitialiseRewired();
                return true;
            }
        }

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("hi");
        }
    }
}
