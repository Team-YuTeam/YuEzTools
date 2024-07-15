using AmongUs.Data;
using AmongUs.GameOptions;
using HarmonyLib;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using YuAntiCheat.Modules;
using UnityEngine;
using YuAntiCheat.Get;
using static YuAntiCheat.Translator;
using Object = UnityEngine.Object;
using AmongUs.GameOptions;
using Epic.OnlineServices.Presence;

namespace YuAntiCheat.Patches;

public class GameStartManagerPatch
{
    private static float timer = 600f;
    private static TextMeshPro warningText;
    private static TextMeshPro EndText;
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
            
            EndText = Object.Instantiate(__instance.GameStartText, __instance.transform);
            EndText.name = "EndText";
            EndText.transform.localPosition = new(0f, 0f - __instance.transform.localPosition.y, -1f);
            EndText.gameObject.SetActive(false);
            
            Logger.Info("EndText instantiated and configured", "test");

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
        public static float EndTimer = -1f;
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
            string EndMessage = "";
            if(Toggles.AutoExit && PingTracker_Update.fps <= 10)
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
                            PingTracker_Update.fps, Math.Round(5 - exitTimer).ToString()));
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
            
            if (StartPatch.s != "结算：")
            {
                EndTimer += Time.deltaTime;
                // if (EndTimer >= 5)
                // {
                //     EndTimer = 0;
                // }

                if (EndTimer != 0 &&  EndTimer <= 5)
                    EndMessage = StartPatch.s;
            }
            
            if (EndMessage == "")
            {
                EndText.gameObject.SetActive(false);
            }
            else
            {
                EndText.text = EndMessage;
                EndText.gameObject.SetActive(true);
            }
            
            if (Main.isFirstSendEnd && StartPatch.s != "结算：" && GetPlayer.IsLobby && EndTimer >= 5)
            {
                Logger.Info("ISFIRSTSENDEND IS TRUE", "结算");
                PlayerControl.LocalPlayer.RpcSendChat(StartPatch.s);
                Main.isFirstSendEnd = false;
                Logger.Info("ISFIRSTSENDEND IS "+Main.isFirstSendEnd, "结算");
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