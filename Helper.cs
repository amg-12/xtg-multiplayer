using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XtgMultiplayer
{
    public static class Helper
    {
        public static Character lastDeadPlayer;

        public static List<Character> GetAllPlayers()
        {
            return GameManager.Instance.CharacterManager.GetAliveCharacters(CharacterType.Player);
        }

        public static void DestroyAllPlayers()
        {
            foreach (Character c in GetAllPlayers())
            {
                UnityEngine.Object.Destroy(c.gameObject);
            }
        }

        public static List<Character> GetAllLivingPlayers()
        {
            return GetAllPlayers().Where(p => p.IsAlive).ToList();
        }

        public static void SetGhost(Character character, bool on) // wip
        {
            character.gameObject.GetComponent<WeaponUser>().enabled = !on;
            character.gameObject.transform.Find("WeaponAttachPoints").gameObject.SetActive(!on);
            character.SetFlying(on, "");
        }

        public static void KillPlayer(Character character)
        {
            character.gameObject.SetActive(false);
            lastDeadPlayer = character;
            AkSoundEngine.PostEvent("Play_UI_gameover_start_01", character.gameObject);
            if (GetAllLivingPlayers().Count == 0)
            {
                DestroyAllPlayers();
            }
        }

        public static void RevivePlayer(Character character, Nullable<Vector2> location = null)
        {
            character.gameObject.SetActive(true);
            lastDeadPlayer = null;
            character.HP = 3;
            if (location != null)
            {
                character.transform.SetXY(location.Value);
            }
            character.WeaponUser.ChangeToNextWeapon();
        }

        public static void ReviveAllPlayers(Nullable<Vector2> location = null)
        {
            foreach (Character c in GetAllPlayers())
            {
                if (c.IsDead)
                {
                    RevivePlayer(c, location);
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
    }
}
