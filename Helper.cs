using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XtgMultiplayer
{
    public static class Helper
    {
        public static List<Character> GetAllPlayers()
        {
            return CharacterManager.Instance.GetAliveCharacters(CharacterType.Player);
        }

        public static List<PlayerController> GetAllPlayerControllers()
        {
            return GetAllPlayers().Select(p => p.gameObject.GetComponent<PlayerController>()).ToList();
        }

        public static List<Character> GetAllLivingPlayers()
        {
            return GetAllPlayers().Where(p => p.IsAlive).ToList();
        }

        public static void DestroyAllPlayers()
        {
            foreach (Character c in GetAllPlayers())
            {
                UnityEngine.Object.Destroy(c.gameObject);
            }
        }

        public static void PositionPlayers(Nullable<Vector2> pos=null)
        {
            List<Character> players = GetAllPlayers();
            foreach (Character player in players)
            {
                player.Show(pos == null
                    ? Field.Instance.Elevator.SpawnPosition
                    : pos.Value
                );
            }
            if (GetAllLivingPlayers().Count == 2)
            {
                players[0].transform.MoveX(-0.2f);
                players[1].transform.MoveX(0.2f);
            }
        }
    }
}
