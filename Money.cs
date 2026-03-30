using HarmonyLib;

namespace XtgMultiplayer
{
    static class Money
    {
        [HarmonyPatch(typeof(PlayerController), "Coin", MethodType.Setter)]
        static class ShareMoney
        {
            public static void Postfix(ref int value)
            {
                foreach (PlayerController player in Helper.GetAllPlayerControllers())
                {
                    AccessTools.Field(typeof(PlayerController), "_coin").SetValue(player, value);
                }
            }
        }

        [HarmonyPatch(typeof(CollectibleItem_Coin), "Snatch")]
        static class SharePickup
        {
            public static bool Prefix(CollectibleItem_Coin __instance, ref Character character)
            {
                character = CharacterManager.Instance.GetNearestCharacter(
                    __instance.transform.position, CharacterType.Player, null);
                return true;
            }
        }
    }
}
