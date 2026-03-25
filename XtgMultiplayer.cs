using System.Linq;
using HarmonyLib;
using MelonLoader;

[assembly: MelonInfo(typeof(XtgMultiplayer.XtgMultiplayer), "Exit the Gungeon Multiplayer Mod", "0.0.1", "Amitai")]
[assembly: MelonGame("Dodge Roll", "Exit the Gungeon")]
namespace XtgMultiplayer
{
    public class XtgMultiplayer : MelonMod
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
                return !BypassFlushAudio || Helper.GetAllPlayers().All(p => p.IsDead);
            }
        }

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
                    RouteData routeData = CharacterManager.Instance.GetPrimaryPlayerController().RouteData;
                    Character chara = Extensions.Instantiate<Character>(__instance.PlayerPrefabs[6], null);
                    chara.transform.SetXY(0f, -0.49f);
                    Controls.AssignSecondPlayer(chara.gameObject);
                    PlayerController pc = chara.gameObject.GetComponent<PlayerController>();
                    AccessTools.Field(typeof(PlayerController), "m_cachedRouteData").SetValue(pc, routeData);
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

        [HarmonyPatch(typeof(SpaceTurtle_FallRecover), "Initialize")]
        static class UnblockOtherPlayerWhenRecovering
        {
            public static bool Prefix(ref Character carrying)
            {
                PlayerController.InputBlockStackCount--;
                return true;
            }
        }

        [HarmonyPatch(typeof(PlayerController), "IsInputAvailable", MethodType.Getter)]
        static class BlockThisPlayerWhenRecovering
        {
            public static void Postfix(PlayerController __instance, ref bool __result)
            {
                __result = __result || __instance.IsReturning;
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

        [HarmonyPatch(typeof(PlayerController), "OnHPChanged")]
        static class HandleDeath
        {
            public static bool Prefix(PlayerController __instance)
            {
                Character character = __instance.gameObject.GetComponent<Character>();
                if (character.IsDead)
                {
                    Helper.KillPlayer(character);
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Character), "OnCharacterDead")]
        static class StopPlayerDeadEvent
        {
            public static bool Prefix(Character __instance)
            {
                return __instance.Type != CharacterType.Player;
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
