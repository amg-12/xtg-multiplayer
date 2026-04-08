using System.Collections.Generic;
using System.Linq;

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
    }
}
