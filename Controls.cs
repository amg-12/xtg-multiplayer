using HarmonyLib;
using Rewired;
using UnityEngine;

namespace XtgMultiplayer
{
    static class Controls
    {
        static Player player1 = ReInput.players.GetPlayer(0);
        static Player player2 = ReInput.players.GetSystemPlayer();

        public static void InitialiseRewired()
        {
            player2.controllers.hasKeyboard = false;
            player2.controllers.hasMouse = false;
            foreach (ControllerMap map in player1.controllers.maps.GetAllMaps())
            {
                player2.controllers.maps.AddMap(map.controllerType, map.categoryId, map);
            }
        }

        public static void AssignSecondPlayer(GameObject character)
        {
            PlayerController pc = character.GetComponent<PlayerController>();
            AccessTools.Field(typeof(PlayerController), "_inputPlayer").SetValue(pc, player2);
            if (player2.controllers.joystickCount < 1)
            {
                player2.controllers.AddController(ControllerType.Joystick, 0, true);
            }
            ReInput.ControllerConnectedEvent += OnControllerConnected;
        }

        static void OnControllerConnected(ControllerStatusChangedEventArgs args)
        {
            if (args.controllerType != ControllerType.Joystick) return;
            if (player2.controllers.joystickCount < player1.controllers.joystickCount)
            {
                player2.controllers.AddController(args.controllerType, args.controllerId, true);
            }
        }

        [HarmonyPatch(typeof(MobilePreIntroController), "Start")]
        static class Initialise
        {
            public static bool Prefix()
            {
                InitialiseRewired();
                return true;
            }
        }
    }
}
