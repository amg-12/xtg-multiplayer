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

        // magic numbers wooo
        // probably change the anchors instead
        public static void Position()
        {
            UI_PlayerStat[] stats = UIManager.Instance.PlayerStats;
            if (stats.Length >= 2)
            {
                stats[1].transform.parent = stats[0].transform.parent;
                stats[1].GetComponent<RectTransform>().anchoredPosition3D = new Vector2(429, 0);
                foreach (UI_PlayerStat stat in stats)
                {
                    stat.ComboUI.transform.parent = stat.transform;
                    stat.ComboUI.transform.localPosition = new Vector2(43, -845);
                }
                stats[0].transform.Find("Coin Text").GetComponent<RectTransform>().anchoredPosition3D = new Vector2(60, -26);
                stats[1].transform.Find("Panel_HP").GetComponent<RectTransform>().anchoredPosition3D = new Vector2(630, -5);
                stats[1].transform.Find("Panel_Blanks").GetComponent<RectTransform>().anchoredPosition3D = new Vector2(626, -15);
                stats[1].transform.Find("Coin Text").gameObject.SetActive(false);
                stats[1].transform.Find("Panel_HP").localScale = new Vector3(-1, 1, 1);
                stats[1].transform.Find("Panel_Blanks").localScale = new Vector3(-1, 1, 1);
                stats[1].transform.Find("Weapon Panel/WeaponUIBG/WeaponSprite").localScale = new Vector3(-1, 1, 1);
            }
        }
    }
}
