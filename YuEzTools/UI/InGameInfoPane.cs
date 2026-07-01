using HarmonyLib;
using UnityEngine;
using TMPro;
using System;
using AmongUs.Data;
using AmongUs.GameOptions;
using InnerNet;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using YuEzTools.Utils;
using YuEzTools.Helpers;
using YuEzTools.Modules;

namespace YuEzTools.UI;

[HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
public static class InGameInfoPane
{
    private static bool isAspectSizeVisible = true;
    private static GameObject aspectSizeCache;
    private static PassiveButton startButtonCache;
    private static TextMeshPro startButtonTextCache;
    private static bool isEventBound = false;
    private static bool isButtonInstantiated = false;
    private static GameObject shareRoomBtnCache;
    private static GameObject shareRoomTips;
    private static float tipHideTime = -1f;
    private static GameObject warningTips;
    private const float SendCooldown = 120f;
    private static float lastSendTime = -999f;

    private static void HideTip()
    {
        if (shareRoomTips != null)
            shareRoomTips.SetActive(false);
        if (warningTips != null)
            warningTips.SetActive(false);
        tipHideTime = -1f;
    }

    public static void Postfix(GameStartManager __instance)
    {

        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleAspectSizeVisibility();
        }

        if (tipHideTime > 0 && Time.time >= tipHideTime)
        {
            HideTip();
        }

        if (shareRoomBtnCache != null && shareRoomTips != null && warningTips != null)
            return;

        GameObject aspectSizeObj = GameObject.Find("AspectSize");
        if (aspectSizeObj == null) return;

        Transform codeSection = aspectSizeObj.transform.Find("GameCodeSection");
        if (codeSection == null) return;

        Transform sendtips = codeSection.Find("CopiedGameCode");
        sendtips.transform.GetChild(1).GetComponent<TextMeshPro>().text = $"<b>{GetString("RoomCodeCopied")}</b>"; 
        Object.Destroy(sendtips.transform.GetChild(1).GetComponent<TextTranslatorTMP>()); 
        Transform copyCodeBtnTrans = codeSection.Find("CopyGameCodeButton");

        if (copyCodeBtnTrans == null || !GetPlayer.IsOnlineGame) return;
        GameObject buttonTemplate = copyCodeBtnTrans.gameObject;
        GameObject shareRoomBtn = Object.Instantiate(buttonTemplate);
        shareRoomBtn.transform.SetParent(codeSection, false);
        shareRoomBtn.transform.localPosition += new Vector3(-0.65f, 0, 0);
        shareRoomBtn.name = "ShareRoomButton";

        SpriteRenderer spr = shareRoomBtn.transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (spr != null)
            spr.sprite = LoadSprite("YuEzTools.Resources.ShareRoom.png", 115f);

        PassiveButton btn = shareRoomBtn.GetComponent<PassiveButton>();
        btn.OnClick = new Button.ButtonClickedEvent();
        btn.OnClick.AddListener((Action)(() =>
        {
            if (shareRoomTips == null || warningTips == null) return;

            float now = Time.time;
            float passTime = now - lastSendTime;
            int res = QQHelper.AddRoom(GameStartManager.Instance.GameRoomNameCode.text,GameData.Instance.PlayerCount,(Main.NormalOptions.TryGetInt(Int32OptionNames.MaxPlayers, out var a) ? a : 0),ServerManager.Instance.CurrentRegion.Name,DataManager.player.customization.name,GameData.Instance.GetHost().PlayerName,GetPlayer.GetImpNums,"YuEZ");
            Info(
                $"Res:{res}\n" +
                $"Resq:{GameStartManager.Instance.GameRoomNameCode.text},{GameData.Instance.PlayerCount},{a},{ServerManager.Instance.CurrentRegion.Name},{DataManager.player.customization.name},{GameData.Instance.GetHost().PlayerName},{GetPlayer.GetImpNums},YuEZ",
                "InGameInfoPane");
            if (passTime < SendCooldown || res == 429)
            {
                float leftSec = SendCooldown - passTime;
                int leftMin = Mathf.CeilToInt(leftSec / 60f);

                TextMeshPro warnText = warningTips.transform.GetChild(1).GetComponent<TextMeshPro>();
                if (warnText != null)
                    warnText.text = string.Format(GetString("Lobby.WarningTips"), leftMin);

                warningTips.SetActive(true);
                shareRoomTips.SetActive(false);
                tipHideTime = now + 2f;
                return;
            }
            else if (res == 200)
            {
                var lobbyInfoPane = LobbyInfoPane.Instance;
                if (lobbyInfoPane != null && lobbyInfoPane.CopyCodeSound != null)
                {
                    SoundManager.Instance.PlaySoundImmediate(lobbyInfoPane.CopyCodeSound, false, 1f, 1f, null);
                }
            
                lastSendTime = now;
                shareRoomTips.SetActive(true);
                warningTips.SetActive(false);
                tipHideTime = now + 2f;
            }
            else if (res == 201)
            {
                float leftSec = SendCooldown - passTime;
                int leftMin = Mathf.CeilToInt(leftSec / 60f);

                TextMeshPro warnText = warningTips.transform.GetChild(1).GetComponent<TextMeshPro>();
                if (warnText != null)
                    warnText.text = GetString("Lobby.RoomFilled");

                warningTips.SetActive(true);
                shareRoomTips.SetActive(false);
                tipHideTime = now + 2f;
                return;
            }else
            {
                float leftSec = SendCooldown - passTime;
                int leftMin = Mathf.CeilToInt(leftSec / 60f);

                TextMeshPro warnText = warningTips.transform.GetChild(1).GetComponent<TextMeshPro>();
                if (warnText != null)
                    warnText.text = string.Format(GetString("Lobby.NotCorrect"), res);

                warningTips.SetActive(true);
                shareRoomTips.SetActive(false);
                tipHideTime = now + 2f;
                return;
            }
        }));

        shareRoomBtnCache = shareRoomBtn;

        if (sendtips == null) return;
        GameObject template = sendtips.gameObject;

        GameObject Sendtips = Object.Instantiate(template);
        Sendtips.transform.SetParent(codeSection, false);
        Sendtips.transform.localPosition += new Vector3(0, -1.3f, 0);
        Sendtips.name = "SendTips";
        Sendtips.SetActive(false);
        TextTranslatorTMP trans1 = Sendtips.transform.GetChild(1).GetComponent<TextTranslatorTMP>();
        if (trans1 != null) Object.Destroy(trans1);
        Sendtips.transform.GetChild(1).GetComponent<TextMeshPro>().text = GetString("Lobby.SendingTips");
        Sendtips.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.blue;
        shareRoomTips = Sendtips;

        GameObject WarningTips = Object.Instantiate(template);
        WarningTips.transform.SetParent(codeSection, false);
        WarningTips.transform.localPosition += new Vector3(0, -1.3f, 0);
        WarningTips.name = "WarningTips";
        WarningTips.SetActive(false);
        TextTranslatorTMP trans2 = WarningTips.transform.GetChild(1).GetComponent<TextTranslatorTMP>();
        if (trans2 != null) Object.Destroy(trans2);
        WarningTips.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.yellow;
        warningTips = WarningTips;


        InitCaches(__instance);
        if (aspectSizeCache == null || startButtonCache == null || startButtonTextCache == null) return;
        if (!isEventBound)
        {
            startButtonCache.OnClick.AddListener((Action)(() => ToggleAspectSizeVisibility()));
            isEventBound = true;
        }
    }
    private static void InitCaches(GameStartManager __instance)
    {
        if (aspectSizeCache == null)
        {
            aspectSizeCache = GameObject.Find("AspectSize");

            if (aspectSizeCache != null)
            {
                isAspectSizeVisible = aspectSizeCache.activeSelf;
            }
        }

        if ((!isButtonInstantiated || startButtonCache == null || !startButtonCache.gameObject.activeInHierarchy)
            && __instance.StartButton != null)
        {
            if (startButtonCache != null)
            {
                Object.Destroy(startButtonCache.gameObject);
            }

            GameObject newButtonObj = Object.Instantiate(__instance.StartButton.gameObject, __instance.StartButton.transform.parent);
            newButtonObj.name = "ShowHideButton";
            newButtonObj.SetActive(true);
            startButtonCache = newButtonObj.GetComponent<PassiveButton>();
            startButtonCache.transform.Find("Inactive")?.gameObject.SetActive(true);
            startButtonCache.enabled = true;
            startButtonCache.gameObject.SetActive(true);
            startButtonCache.transform.GetChild(5).gameObject.SetActive(false);
            startButtonCache.OnClick = new Button.ButtonClickedEvent();
            startButtonTextCache = newButtonObj.GetComponentInChildren<TextMeshPro>();
            startButtonCache.transform.localPosition = new Vector3(1.1073f, -0.26f, 0f);
            startButtonCache.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            startButtonCache.OnClick.AddListener((Action)(() => ToggleAspectSizeVisibility()));
            isEventBound = true;
            UpdateStartButtonText();
            isButtonInstantiated = true;
        }
    }

    private static void ToggleAspectSizeVisibility()
    {
        isAspectSizeVisible = !isAspectSizeVisible;
        if (aspectSizeCache != null)
        {
            aspectSizeCache.SetActive(isAspectSizeVisible);
        }
        UpdateStartButtonText();
    }

    private static void UpdateStartButtonText()
    {
        if (startButtonTextCache != null)
        {
            startButtonTextCache.text = isAspectSizeVisible ? GetString("Hide") : GetString("Show");
        }
    }
}