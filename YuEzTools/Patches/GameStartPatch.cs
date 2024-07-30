using AmongUs.Data;
using AmongUs.GameOptions;
using HarmonyLib;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using YuEzTools.Modules;
using UnityEngine;
using static YuEzTools.Translator;
using Object = UnityEngine.Object;
using AmongUs.GameOptions;
using Epic.OnlineServices.Presence;
using YuEzTools.Get;
using static YuEzTools.Logger;

namespace YuEzTools.Patches;

public class GameStartManagerPatch
{
    public static float timer = 600f;
    private static TextMeshPro warningText;
    public static TextMeshPro GameCountdown;
    private static PassiveButton cancelButton;
    public static string countDown = "";

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
    public class GameStartManagerStartPatch
    {
        public static void Postfix(GameStartManager __instance)
        {
            __instance.MinPlayers = 1;

            __instance.GameRoomNameCode.text = GameCode.IntToGameName(AmongUsClient.Instance.GameId);
            // Reset lobby countdown timer
            timer = 600f;

            warningText = Object.Instantiate(__instance.GameStartText, __instance.transform);
            warningText.name = "WarningText";
            warningText.transform.localPosition = new(0f, 0f - __instance.transform.localPosition.y, -1f);
            warningText.gameObject.SetActive(false);
            
            Logger.Info("WarningText instantiated and configured", "test");
            

            cancelButton = Object.Instantiate(__instance.StartButton, __instance.transform);
            var cancelLabel = cancelButton.GetComponentInChildren<TextMeshPro>();
            cancelLabel.text = GetString("Cancel");
            cancelButton.transform.localScale = new(0.4f, 0.4f, 1f);
            cancelButton.transform.localPosition = new(0f, -0.37f, 0f);

            cancelButton.OnClick = new();
            cancelButton.OnClick.AddListener((Action)(() => __instance.ResetStartState()));
            cancelButton.gameObject.SetActive(false);

            Logger.Info("CancelButton instantiated and configured", "test");
            
            if (!AmongUsClient.Instance.AmHost) return;
        }
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public class GameStartManagerUpdatePatch
    {
        private static int updateTimer = 0;
        public static float exitTimer = -1f;
        public static void Prefix(GameStartManager __instance)
        {
            if (Toggles.AutoStartGame)
            {
                updateTimer++;
                if (updateTimer >= 50)
                {
                    updateTimer = 0;
                    if (GameData.Instance.PlayerCount >= 14 && !GetPlayer.IsCountDown)
                    {
                        GameStartManager.Instance.startState = GameStartManager.StartingStates.Countdown;
                        GameStartManager.Instance.countDownTimer = 10;
                    }
                }
            }
            
        }
        public static void Postfix(GameStartManager __instance)
        {
            string warningMessage = "";
            if(Toggles.AutoExit && PingTrackerUpdatePatch.fps <= 10)
            {
                exitTimer += Time.deltaTime;
                if (exitTimer >= 5)
                {
                    exitTimer = 0;
                    AmongUsClient.Instance.ExitGame(DisconnectReasons.ExitGame);
                    SceneChanger.ChangeScene("MainMenu");
                }

                if (exitTimer != 0)
                    warningMessage = Utils.Utils.ColorString(Color.red,
                        string.Format(GetString("Warning.AutoExitAtMismatchedFPS"),
                            PingTrackerUpdatePatch.fps, Math.Round(5 - exitTimer).ToString()));
            }
            if (warningMessage == "")
            {
                warningText.gameObject.SetActive(false);
            }
            else
            {
                warningText.text = warningMessage;
                warningText.gameObject.SetActive(true);
            }
            
            
            // Lobby timer
            if (
                !AmongUsClient.Instance.AmHost ||
                !GameData.Instance ||
                AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame)
            {
                return;
            }

            timer = Mathf.Max(0f, timer -= Time.deltaTime);
            int minutes = (int)timer / 60;
            int seconds = (int)timer % 60;
            countDown = $"{minutes:00}:{seconds:00}";
            if (timer <= 60) countDown = Utils.Utils.ColorString(Color.red, countDown);
            //timerText.text = countDown;
        }
    }
}

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CreatePlayer))]
class CreatePlayerPatch
{
    public static void Postfix( AmongUsClient __instance, [HarmonyArgument(0)] ClientData client)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        
        Logger.Msg($"Create player data: ID {client.Id}: {client.PlayerName}", "CreatePlayer");

        if (GetPlayer.isNormalGame)
        {
            _ = new LateTask(() =>
            {
                if (!AmongUsClient.Instance.IsGameStarted && client.Character != null && StartPatch.s != GetString("EndMessage") && Main.isFirstSendEnd)
                {
                    Main.isChatCommand = true;
                    Info("发送：结算信息", "JoinPatch");
                    PlayerControl.LocalPlayer.RpcSendChat(StartPatch.s);
                    Main.isChatCommand = false;
                    Main.isFirstSendEnd = false;
                }
            }, 3.1f, "DisplayLastRoles");
        }
    }
}