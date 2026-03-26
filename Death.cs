using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace XtgMultiplayer
{
    static class Death
    {
        public static Character lastDeadPlayer;

        public static void SetGhost(Character character, bool on) // wip
        {
            character.gameObject.GetComponent<WeaponUser>().enabled = !on;
            character.gameObject.transform.Find("WeaponAttachPoints").gameObject.SetActive(!on);
            character.SetFlying(on, "");
        }

        public static void OnPlayerDead(Character character)
        {
            character.gameObject.SetActive(false);
            lastDeadPlayer = character;
            FallRecover.EndRecovery(character);
            AkSoundEngine.PostEvent("Play_UI_gameover_start_01", character.gameObject);
            if (Helper.GetAllLivingPlayers().Count == 0)
            {
                Helper.DestroyAllPlayers();
            }
        }

        public static void RevivePlayer(Character character, Nullable<Vector2> location = null)
        {
            character.gameObject.SetActive(true);
            lastDeadPlayer = null;
            FallRecover.EndRecovery(character);
            character.HP = 3;
            if (location != null)
            {
                character.transform.SetXY(location.Value);
            }
            character.WeaponUser.ChangeToNextWeapon();
        }

        public static void ReviveAllPlayers(Nullable<Vector2> location = null)
        {
            foreach (Character c in Helper.GetAllPlayers())
            {
                if (c.IsDead)
                {
                    RevivePlayer(c, location);
                }
            }
        }

        [HarmonyPatch(typeof(PlayerController), "OnHPChanged")]
        static class RegisterPlayerDead
        {
            public static bool Prefix(PlayerController __instance)
            {
                Character character = __instance.gameObject.GetComponent<Character>();
                if (character.IsDead)
                {
                    OnPlayerDead(character);
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Character), "OnCharacterDead")]
        static class DontDestroyPlayer
        {
            public static bool Prefix(Character __instance)
            {
                return __instance.Type != CharacterType.Player || Helper.GetAllLivingPlayers().Count == 0;
            }
        }

        [HarmonyPatch(typeof(CharacterEventController_BossDead), "Coroutine_Event")]
        static class ReviveAfterBoss
        {
            public static bool Prefix()
            {
                ReviveAllPlayers();
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
                    ReviveAllPlayers(__instance.transform.GetXY());
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public static bool ExcludeDeadTarget(ref Character excludeTarget, CharacterType type)
        {
            if (type == CharacterType.Player)
            {
                excludeTarget = lastDeadPlayer;
            }
            return true;
        }

        [HarmonyPatch(typeof(CharacterManager), "GetNearestCharacter")]
        static class ExcludeTarget
        {
            public static bool Prefix(ref Character excludeTarget, ref CharacterType type)
            {
                return ExcludeDeadTarget(ref excludeTarget, type);
            }
        }

        [HarmonyPatch(typeof(CharacterManager), "GetNearestCharacterInScreen")]
        static class ExcludeTargetInScreen
        {
            public static bool Prefix(ref Character excludeTarget, ref CharacterType type)
            {
                return ExcludeDeadTarget(ref excludeTarget, type);
            }
        }
    }
}
