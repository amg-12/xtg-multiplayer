using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace XtgMultiplayer
{
    static class UI
    {
        [HarmonyPatch(typeof(UIManager), "Awake")]
        static class MakeSecondUI
        {
            public static void Postfix(UIManager __instance)
            {
                List<UI_PlayerStat> stats = __instance.PlayerStats.ToList();
                stats.Add(Extensions.Instantiate<UI_PlayerStat>(stats[0]));
                stats[1].ComboUI = Object.Instantiate<UI_PlayerCombo>(stats[0].ComboUI);
                __instance.PlayerStats = stats.ToArray();
            }
        }

        [HarmonyPatch(typeof(UI_PlayerStat), "Init")]
        static class PositionUI
        {
            public static void Postfix(UI_PlayerStat __instance, ref Character player)
            {
                Position(__instance, player);
            }
        }

        static void Position(UI_PlayerStat stat, Character player)
        {
            RectTransform rect = stat.GetComponent<RectTransform>();
            RectTransform combo = stat.ComboUI.GetComponent<RectTransform>();
            combo.SetParent(rect);
            combo.anchoredPosition = new Vector2(8, -35);
            combo.anchorMin = new Vector2(0, 1);
            combo.anchorMax = new Vector2(0, 1);
            combo.pivot     = new Vector2(0, 1);
            RectTransform coins = rect.Find("Coin Text").GetComponent<RectTransform>();
            coins.anchoredPosition = new Vector2(60, -26);
            if (player.PlayerIndex == 1)
            {
                rect.SetParent(UIManager.Instance.transform);
                rect.anchoredPosition3D = Vector3.zero;
                rect.anchorMin = new Vector2(1, 1);
                rect.localRotation = Quaternion.Euler(0, 180, 0);
                foreach (Transform text in
                    rect.GetComponentsInChildren<TextMeshProUGUI>()
                    .Select(t => t.transform))
                {
                    text.localRotation = Quaternion.Euler(0, 180, 0);
                    text.MoveX(text.name.Contains("Score") ? -0.5f : -1);
                }
                coins.gameObject.SetActive(false);
            }
        }
    }
}
