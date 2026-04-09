using System.Collections.Generic;
using HarmonyLib;
using Rewired;
using UnityEngine;

namespace XtgMultiplayer
{
    static class AutoDodge
    {
        class AutoData
        {
            public int lastAutoDescendUpdateFrame = -1;
            public bool lastAutoDescendIsPressed = false;
            public bool currentAutoDescendIsPressed = false;
            public int lastAutoAscendUpdateFrame = -1;
            public bool lastAutoAscendIsPressed = false;
            public bool currentAutoAscendIsPressed = false;

            static AutoData() {}
        }

        static Dictionary<Player, AutoData> AutoDataByPlayer = new Dictionary<Player, AutoData>
        {
            { ReInput.players.GetPlayer(0), new AutoData() },
            { ReInput.players.GetSystemPlayer(), new AutoData() }
        };

        static void UpdateAutoDescendFrameData(Player inputPlayer)
        {
            if (AutoDataByPlayer[inputPlayer].lastAutoDescendUpdateFrame < Time.frameCount)
            {
                AutoDataByPlayer[inputPlayer].lastAutoDescendIsPressed = AutoDataByPlayer[inputPlayer].currentAutoDescendIsPressed;
                AutoDataByPlayer[inputPlayer].currentAutoDescendIsPressed = false;
                if (inputPlayer.GetAxis(GungeonActions.Down) > 0.99f)
                {
                    AutoDataByPlayer[inputPlayer].currentAutoDescendIsPressed = true;
                }
                else
                {
                    Vector2 moveVector = GungeonActions.GetMoveVector(inputPlayer);
                    float magnitude = moveVector.magnitude;
                    float a = moveVector.ToAngle();
                    if (magnitude > 0.8f && BraveMathCollege.AbsAngleBetween(a, -90f) < 15f)
                    {
                        AutoDataByPlayer[inputPlayer].currentAutoDescendIsPressed = true;
                    }
                }
                AutoDataByPlayer[inputPlayer].lastAutoDescendUpdateFrame = Time.frameCount;
            }
        }

        static bool AdditionalDropDown(Player inputPlayer, bool specialIsFallingCheck = false)
        {
            if (specialIsFallingCheck && TutorialManager.Instance)
            {
                return false;
            }
            if (!MobileController.IsUsingTouchControls)
            {
                UpdateAutoDescendFrameData(inputPlayer);
                if (GameOptions.Instance.RapidDescent && specialIsFallingCheck && AutoDataByPlayer[inputPlayer].currentAutoDescendIsPressed)
                {
                    return true;
                }
                if (GungeonActions.IsAutoDescend(inputPlayer) && AutoDataByPlayer[inputPlayer].currentAutoDescendIsPressed && !AutoDataByPlayer[inputPlayer].lastAutoDescendIsPressed)
                {
                    return true;
                }
            }
            return false;
        }

        static bool AdditionalJumpDown(Player inputPlayer, PlayerController playerController)
        {
            if (GungeonActions.IsAutoAscend(inputPlayer) && !MobileController.IsUsingTouchControls && playerController.Owner.MovingType == CharacterMovingType.Ground)
            {
                UpdateAutoAscendFrameData(inputPlayer);
                return AutoDataByPlayer[inputPlayer].currentAutoAscendIsPressed && !AutoDataByPlayer[inputPlayer].lastAutoAscendIsPressed;
            }
            return false;
        }

        static void UpdateAutoAscendFrameData(Player inputPlayer)
        {
            if (AutoDataByPlayer[inputPlayer].lastAutoAscendUpdateFrame < Time.frameCount)
            {
                AutoDataByPlayer[inputPlayer].lastAutoAscendIsPressed = AutoDataByPlayer[inputPlayer].currentAutoAscendIsPressed;
                AutoDataByPlayer[inputPlayer].currentAutoAscendIsPressed = false;
                if (inputPlayer.GetAxis(GungeonActions.Up) > 0.99f)
                {
                    AutoDataByPlayer[inputPlayer].currentAutoAscendIsPressed = true;
                }
                else
                {
                    Vector2 moveVector = GungeonActions.GetMoveVector(inputPlayer);
                    float magnitude = moveVector.magnitude;
                    float a = moveVector.ToAngle();
                    if (magnitude > 0.75f && BraveMathCollege.AbsAngleBetween(a, 90f) < 55f)
                    {
                        AutoDataByPlayer[inputPlayer].currentAutoAscendIsPressed = true;
                    }
                }
                AutoDataByPlayer[inputPlayer].lastAutoAscendUpdateFrame = Time.frameCount;
            }
        }

        static bool AdditionalJumpUp(Player inputPlayer, global::PlayerController playerController)
        {
            if (GungeonActions.IsAutoAscend(inputPlayer) && !MobileController.IsUsingTouchControls && playerController.Owner.MovingType == CharacterMovingType.Ground)
            {
                UpdateAutoAscendFrameData(inputPlayer);
                return !AutoDataByPlayer[inputPlayer].currentAutoAscendIsPressed && AutoDataByPlayer[inputPlayer].lastAutoAscendIsPressed;
            }
            return false;
        }

        static bool AdditionalJump(Player inputPlayer, global::PlayerController playerController)
        {
            if (GungeonActions.IsAutoAscend(inputPlayer) && !MobileController.IsUsingTouchControls && playerController.Owner.MovingType == CharacterMovingType.Ground)
            {
                UpdateAutoAscendFrameData(inputPlayer);
                return AutoDataByPlayer[inputPlayer].currentAutoAscendIsPressed;
            }
            return false;
        }

        [HarmonyPatch(typeof(GungeonActions), "UpdateAutoDescendFrameData")]
        static class PatchUpdateAutoDescendFrameData
        {
            public static bool Prefix(Player inputPlayer)
            {
                UpdateAutoDescendFrameData(inputPlayer);
                return false;
            }
        }

        [HarmonyPatch(typeof(GungeonActions), "AdditionalDropDown")]
        static class PatchAdditionalDropDown
        {
            public static bool Prefix(Player inputPlayer, bool specialIsFallingCheck, ref bool __result)
            {
                __result = AdditionalDropDown(inputPlayer, specialIsFallingCheck);
                return false;
            }
        }

        [HarmonyPatch(typeof(GungeonActions), "AdditionalJumpDown")]
        static class PatchAdditionalJumpDown
        {
            public static bool Prefix(Player inputPlayer, PlayerController playerController, ref bool __result)
            {
                __result = AdditionalJumpDown(inputPlayer, playerController);
                return false;
            }
        }

        [HarmonyPatch(typeof(GungeonActions), "UpdateAutoAscendFrameData")]
        static class PatchUpdateAutoAscendFrameData
        {
            public static bool Prefix(Player inputPlayer)
            {
                UpdateAutoAscendFrameData(inputPlayer);
                return false;
            }
        }

        [HarmonyPatch(typeof(GungeonActions), "AdditionalJumpUp")]
        static class PatchAdditionalJumpUp
        {
            public static bool Prefix(Player inputPlayer, PlayerController playerController, ref bool __result)
            {
                __result = AdditionalJumpUp(inputPlayer, playerController);
                return false;
            }
        }

        [HarmonyPatch(typeof(GungeonActions), "AdditionalJump")]
        static class PatchAdditionalJump
        {
            public static bool Prefix(Player inputPlayer, PlayerController playerController, ref bool __result)
            {
                __result = AdditionalJump(inputPlayer, playerController);
                return false;
            }
        }
    }
}