using HarmonyLib;
using UnityEngine;

namespace XtgMultiplayer
{
    static class UI
    {
        [HarmonyPatch(typeof(UIManager), "Awake")]
        static class MakeSecondUI
        {
            public static bool Prefix(UIManager __instance)
            {
                UI_PlayerStat[] stats = __instance.PlayerStats;
                stats = new UI_PlayerStat[]
                {
                    stats[0],
                    Extensions.Instantiate<UI_PlayerStat>(stats[0])
                };
                stats[1].ComboUI = Object.Instantiate<UI_PlayerCombo>(stats[0].ComboUI);
                __instance.PlayerStats = stats;
                return true;
            }
        }

        public static void Position()
        {
            UI_PlayerStat[] stats = UIManager.Instance.PlayerStats;
            if (stats.Length >= 2)
            {
                stats[1].transform.parent = stats[0].transform.parent;
                stats[1].transform.position = stats[0].transform.position;
                stats[1].transform.MoveY(-50);
                stats[1].ComboUI.transform.parent = stats[0].ComboUI.transform.parent;
                stats[1].ComboUI.transform.position = stats[0].ComboUI.transform.position;
                stats[1].ComboUI.transform.MoveY(-50);
                stats[1].ComboUI.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
