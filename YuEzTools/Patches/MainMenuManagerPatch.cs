using System;
using TMPro;
using UnityEngine;
using YuEzTools.Helpers;
using YuEzTools.Modules;
using YuEzTools.UI;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Assets.InnerNet;
using System.Linq;
using Assets.CoreScripts;
using System.Text;
using InnerNet;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;
using Il2CppSystem.Security.Cryptography;
using static UnityEngine.UI.Button;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using Il2CppSystem.CodeDom.Compiler;
using AmongUs.Data;
using Object = UnityEngine.Object;

namespace YuEzTools.Patches;

[HarmonyPatch]
public class MainMenuManagerPatch
{
    public static MainMenuManager Instance { get; private set; }

    public static GameObject InviteButton;
    public static GameObject WebsiteButton;
    public static GameObject ProjectButton;
    // public static GameObject DevsButton;
    public static GameObject AfdianButton;
    public static GameObject BilibiliButton;
    public static GameObject BugButton;
    public static GameObject UpdateButton;
    public static GameObject PlayButton;
    
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.OpenGameModeMenu))]
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.OpenAccountMenu))]
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.OpenCredits))]
    [HarmonyPrefix, HarmonyPriority(Priority.Last)]
    public static void ShowRightPanel() => ShowingPanel = true;

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Open))]
    [HarmonyPatch(typeof(AnnouncementPopUp), nameof(AnnouncementPopUp.Show))]
    [HarmonyPrefix, HarmonyPriority(Priority.Last)]
    public static void HideRightPanel()
    {
        ShowingPanel = false;
        AccountManager.Instance?.transform?.FindChild("AccountTab/AccountWindow")?.gameObject?.SetActive(false);
    }

    public static void ShowRightPanelImmediately()
    {
        ShowingPanel = true;
        TitleLogoPatch.RightPanel.transform.localPosition = TitleLogoPatch.RightPanelOp;
        Instance.OpenGameModeMenu();
    }

    private static bool isOnline = false;
    public static bool ShowedBak = false;
    private static bool ShowingPanel = false;

    [HarmonyPatch(typeof(SignInStatusComponent), nameof(SignInStatusComponent.SetOnline)), HarmonyPostfix]
    public static void SetOnline_Postfix() { _ = new LateTask(() => { isOnline = true; }, 0.1f, "Set Online Status"); }

    // [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start)), HarmonyPostfix]
    // public static void MainMenuManager_Start()
    // {
    //     GameObject.Find("MainUI").transform.FindChild("ScreemMask").gameObject.SetActive(false);
    //     // MainMenuManager.screenMask.gameObject.SetActive(false);
    // }
    
    
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate)), HarmonyPostfix]
    public static void MainMenuManager_LateUpdate()
    {
        CustomPopup.Update();

        if (GameObject.Find("MainUI") == null) ShowingPanel = false;

        if (TitleLogoPatch.RightPanel != null)
        {
            // var pos1 = TitleLogoPatch.RightPanel.transform.localPosition;
            // Vector3 lerp1 = Vector3.Lerp(pos1, TitleLogoPatch.RightPanelOp + new Vector3((ShowingPanel ? 0f : 10f), 0f, 0f), Time.deltaTime * (ShowingPanel ? 3f : 2f));
            // if (ShowingPanel
            //     ? TitleLogoPatch.RightPanel.transform.localPosition.x > TitleLogoPatch.RightPanelOp.x + 0.03f
            //     : TitleLogoPatch.RightPanel.transform.localPosition.x < TitleLogoPatch.RightPanelOp.x + 9f
            //     ) TitleLogoPatch.RightPanel.transform.localPosition = lerp1;
            
            // thanks fs
            var pos1 = TitleLogoPatch.RightPanel.transform.localPosition;
            var pos3 = new Vector3(
                TitleLogoPatch.RightPanelOp.x * GetResolutionOffset(),
                TitleLogoPatch.RightPanelOp.y, TitleLogoPatch.RightPanelOp.z);
            var lerp1 = Vector3.Lerp(pos1, ShowingPanel ? pos3 : TitleLogoPatch.RightPanelOp + new Vector3(20f, 0f, 0f),
                Time.deltaTime * (ShowingPanel ? 3f : 2f));
            if (ShowingPanel
                    ? TitleLogoPatch.RightPanel.transform.localPosition.x > pos3.x + 0.03f
                    : TitleLogoPatch.RightPanel.transform.localPosition.x < TitleLogoPatch.RightPanelOp.x + 29f
               ) TitleLogoPatch.RightPanel.transform.localPosition = lerp1;
        }

        if (ShowedBak || !isOnline) return;
        var bak = GameObject.Find("BackgroundTexture");
        if (bak == null || !bak.active) return;
        var pos2 = bak.transform.position;
        Vector3 lerp2 = Vector3.Lerp(pos2, new Vector3(pos2.x, 7.1f, pos2.z), Time.deltaTime * 1.4f);
        bak.transform.position = lerp2;
        if (pos2.y > 7f) ShowedBak = true;
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start)), HarmonyPostfix]
    public static void Start_Postfix(MainMenuManager __instance)
    {
        Instance = __instance;

        EnterCodePatch.ifFirst = true;

        SimpleButton.SetBase(__instance.quitButton);

        int row = 1; int col = 0;
        GameObject CreatButton(string text, Action action)
        {
            col++; if (col > 2) { col = 1; row++; }
            var template = col == 1 ? __instance.creditsButton.gameObject : __instance.quitButton.gameObject;
            var button = Object.Instantiate(template, template.transform.parent);
            button.transform.transform.FindChild("FontPlacer").GetChild(0).gameObject.DestroyTranslator();
            var buttonText = button.transform.FindChild("FontPlacer").GetChild(0).GetComponent<TextMeshPro>();
            buttonText.text = GetString(text);
            PassiveButton passiveButton = button.GetComponent<PassiveButton>();
            passiveButton.OnClick = new();
            passiveButton.OnClick.AddListener(action);
            AspectPosition aspectPosition = button.GetComponent<AspectPosition>();
            aspectPosition.anchorPoint = new Vector2(col == 1 ? 0.415f : 0.583f, 0.5f - 0.08f * row);
            // from fs
#if Android
            var yPosition = col == 1 ? 0.5f - 0.08f * row : 0.5f - 0.08f * (row - 1);
            if(col != 1) passiveButton.gameObject.SetActive(true);
#else
            var yPosition = 0.5f - 0.08f * row;
#endif
            aspectPosition.anchorPoint = new Vector2(
                col == 1 ? 0.415f : 0.583f,
                yPosition
            );
            return button;
        }

        var minorActiveSprite = __instance.quitButton.activeSprites.GetComponent<SpriteRenderer>().sprite;
        Color shade = new(0f, 0f, 0f, 0f);

        void FormatButtonColor(PassiveButton button, Sprite borderType, Color inActiveColor, Color activeColor, Color inActiveTextColor, Color activeTextColor)
        {
            button.activeSprites.transform.FindChild("Shine")?.gameObject?.SetActive(false);
            button.inactiveSprites.transform.FindChild("Shine")?.gameObject?.SetActive(false);
            var activeRenderer = button.activeSprites.GetComponent<SpriteRenderer>();
            var inActiveRenderer = button.inactiveSprites.GetComponent<SpriteRenderer>();
            activeRenderer.sprite = minorActiveSprite;
            inActiveRenderer.sprite = minorActiveSprite;
            activeRenderer.color = activeColor.a == 0f ? new Color(inActiveColor.r, inActiveColor.g, inActiveColor.b, 1f) : activeColor;
            inActiveRenderer.color = inActiveColor;
            button.activeTextColor = activeTextColor;
            button.inactiveTextColor = inActiveTextColor;
        }

        var extraLinkName = "";
        var extraLinkUrl = "";
        var extraLinkEnabled = false;
        extraLinkName = "InviteButton";
        extraLinkUrl = TranslationController.Instance.currentLanguage.languageID == SupportedLangs.SChinese || TranslationController.Instance.currentLanguage.languageID == SupportedLangs.TChinese ? Main.QQUrl : Main.DcUrl;
        extraLinkEnabled = true;

        if (InviteButton == null) InviteButton = CreatButton(extraLinkName, () => { OpenURL.OpenUrl(extraLinkUrl); });
        InviteButton.gameObject.SetActive(extraLinkEnabled);
        InviteButton.name = "YuET Extra Link Button";

        if (WebsiteButton == null) WebsiteButton = CreatButton("WebsiteButton", () => OpenURL.OpenUrl("https://night-gua.github.io/"));
        WebsiteButton.gameObject.SetActive(true);
        WebsiteButton.name = "YuET Website Button";

        var ProjectLink = IsChineseLanguageUser
             // ? "https://gitee.com/xigua_ya/YuEzTools/"
             ? "https://kkgithub.com/Team-YuTeam/YuEzTools/"
            : "https://github.com/Team-YuTeam/YuEzTools/";
        if (ProjectButton == null) ProjectButton = CreatButton("ProjectButton", () => OpenURL.OpenUrl(ProjectLink));
        ProjectButton.gameObject.SetActive(true);
        ProjectButton.name = "YuET Project Button";

        // var s = "";
        // if (DevsButton == null) DevsButton = CreatButton("DevsButton", () =>
        // {
        //     s = "";
        //     foreach (var dev in DevManager.DevUserList)
        //     {
        //         if (dev.Jobs != "NotJob")
        //             s += $"<color={dev.Color}>{dev.Name}</color> => <size=60%>{GetString(dev.Jobs)}</size>\n";
        //     }
        //     CustomPopup.Show(GetString("DevsTitle"), s
        //         , new()
        //         {
        //             (GetString(StringNames.Okay), null)
        //         });
        // });
        // DevsButton.gameObject.SetActive(true);
        // DevsButton.name = "YuET Devs Button";

        if (AfdianButton == null) AfdianButton = CreatButton("AfdianButton", () => OpenURL.OpenUrl("https://afdian.com/a/yuqianzhi"));
        AfdianButton.gameObject.SetActive(true);
        AfdianButton.name = "YuET Afdian Button";

        if (BilibiliButton == null) BilibiliButton = CreatButton("BiliBiliButton", () => OpenURL.OpenUrl("https://space.bilibili.com/1638639993"));
        BilibiliButton.gameObject.SetActive(true);
        BilibiliButton.name = "YuET BiliBili Button";

        if (BugButton == null) BugButton = CreatButton("BugButton", ShowBugReportUI);
        BugButton.gameObject.SetActive(true);
        BugButton.name = "YuET Bug Button";

        PlayButton = __instance.playButton.gameObject;
        if (UpdateButton == null)
        {

            UpdateButton = Object.Instantiate(PlayButton, PlayButton.transform.parent);
            UpdateButton.name = "YuET Update Button";
            UpdateButton.transform.localPosition = PlayButton.transform.localPosition;
            var passiveButton = UpdateButton.GetComponent<PassiveButton>();
            passiveButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(0.49f, 0.34f, 0.62f, 0.8f);
            passiveButton.activeSprites.GetComponent<SpriteRenderer>().color = new Color(0.49f, 0.34f, 0.62f, 1f);
            passiveButton.OnClick = new();
            passiveButton.OnClick.AddListener((Action)(() =>
            {
                PlayButton.SetActive(true);
                UpdateButton.SetActive(false);
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    if (ModUpdater.CanUpdate)
                    {
                        ModUpdater.StartUpdate();
                    }
                    else
                    {
                        CustomPopup.Show(GetString("UpdateBySelfTitle"), GetString("UpdateBySelfText"), new()
                        {
                            (GetString(StringNames.Okay), null)
                        });
                    }
                }
            }));
            UpdateButton.transform.transform.FindChild("FontPlacer").GetChild(0).gameObject.DestroyTranslator();
        }

        Dictionary<List<PassiveButton>, (Sprite, Color, Color, Color, Color)> customButtons = new()
        {
            {new List<PassiveButton>() {InviteButton.GetComponent<PassiveButton>(),WebsiteButton.GetComponent<PassiveButton>(),ProjectButton.GetComponent<PassiveButton>(),AfdianButton.GetComponent<PassiveButton>(),BilibiliButton.GetComponent<PassiveButton>(),BugButton.GetComponent<PassiveButton>()},//,DevsButton.GetComponent<PassiveButton>()},
                (minorActiveSprite, new(0.65f, 1f, 0.247f, 0.8f), shade, Color.white, Color.white) },
        };

        foreach (var kvp in customButtons)
            kvp.Key.Do(button => FormatButtonColor(button, kvp.Value.Item1, kvp.Value.Item2, kvp.Value.Item3, kvp.Value.Item4, kvp.Value.Item5));

        // GameObject.Destroy(__instance.creditsButton.gameObject);
        // GameObject.Destroy(__instance.quitButton.gameObject);
        // var BottomButtonBounds = GameObject.Find("BottomButtonBounds");
        // BottomButtonBounds.transform.localPosition += new Vector3(0, 0.8f, 0);
    }

    private static int _bugButtonClickCount = 0;

    private static void ShowBugReportUI()
    {
        _bugButtonClickCount++;

        var oldBugScreen = AccountManager.Instance.transform.Find("BUGSCREEN");
        if (oldBugScreen != null) Object.Destroy(oldBugScreen.gameObject);
        var oldScreen = AccountManager.Instance.transform.Find("SCREEN");
        if (oldScreen != null) Object.Destroy(oldScreen.gameObject);

        var template = AccountManager.Instance.transform.Find("PremissionRequestWindow");
        if (template == null) return;

        if (_bugButtonClickCount == 1)
        {
            GameObject sliderTemplate = Object.Instantiate(template.gameObject, AccountManager.Instance.transform);
            sliderTemplate.name = "BUGSCREEN";
            sliderTemplate.SetActive(true);

            sliderTemplate.transform.Find("TitleText_TMP").GetComponent<TextMeshPro>().text = GetString("BugReport.Title");
            Object.Destroy(sliderTemplate.transform.Find("TitleText_TMP").GetComponent<TextTranslatorTMP>());

            sliderTemplate.transform.Find("InfoText_TMP").GetComponent<TextMeshPro>().text = GetString("BugReport.Info");
            Object.Destroy(sliderTemplate.transform.Find("InfoText_TMP").GetComponent<TextTranslatorTMP>());

            sliderTemplate.transform.Find("GuardianEmailTitle_TMP").GetComponent<TextMeshPro>().text = GetString("BugReport.TimeLabel");
            Object.Destroy(sliderTemplate.transform.Find("GuardianEmailTitle_TMP").GetComponent<TextTranslatorTMP>());
            sliderTemplate.transform.Find("GuardianEmailTitle_TMP").localPosition = new Vector3(-2.3f, 1.3f, 0f);

            sliderTemplate.transform.Find("GuardianEmailConfirm").localPosition = new Vector3(0f, 0.67f, 0f);
            Object.Destroy(sliderTemplate.transform.Find("GuardianEmailConfirm").GetComponent<EmailTextBehaviour>());

            sliderTemplate.transform.Find("GuardianEmailConfirmTitle_TMP").GetComponent<TextMeshPro>().text = GetString("BugReport.DescLabel");
            Object.Destroy(sliderTemplate.transform.Find("GuardianEmailConfirmTitle_TMP").GetComponent<TextTranslatorTMP>());
            sliderTemplate.transform.Find("GuardianEmailConfirmTitle_TMP").localPosition = new Vector3(-2.3f, 0f, 0f);

            var emailInput = sliderTemplate.transform.Find("GuardianEmail");
            emailInput.GetChild(0).GetComponent<SpriteRenderer>().size = new Vector2(6.8f, 1.35f);
            Object.Destroy(emailInput.GetComponent<EmailTextBehaviour>());
            emailInput.localPosition = new Vector3(0f, -0.98f, 0f);
            emailInput.GetComponent<BoxCollider2D>().size = new Vector2(6.8f, 1.35f);
            emailInput.GetChild(1).localPosition = new Vector3(-3.3f, 0.45f, 0f);

            sliderTemplate.transform.GetChild(9).gameObject.SetActive(false);

            var submitBtn = sliderTemplate.transform.Find("SubmitButton").GetComponent<PassiveButton>();
            submitBtn.OnClick = new ButtonClickedEvent();
            submitBtn.OnClick.AddListener((Action)(() =>
            {
                var bugText = emailInput.GetChild(1).GetComponent<TextMeshPro>();
                var timeText = sliderTemplate.transform.Find("GuardianEmailConfirm").GetChild(1).GetComponent<TextMeshPro>();
                var timeBg = emailInput.GetChild(0).GetComponent<SpriteRenderer>();
                var bugBg = sliderTemplate.transform.Find("GuardianEmailConfirm").GetChild(0).GetComponent<SpriteRenderer>();

                bool timeEmpty = string.IsNullOrWhiteSpace(timeText.text);
                bool bugEmpty = string.IsNullOrWhiteSpace(bugText.text);

                if (timeEmpty || bugEmpty)
                {
                    if (timeEmpty) timeBg.color = Color.red;
                    if (bugEmpty) bugBg.color = Color.red;
                    return;
                }

                string username = DataManager.Player.Customization.Name;
                string friendcode = DestroyableSingleton<EOSManager>.Instance.FriendCode;
                string issueTitle = timeText.text;
                string issueLong = bugText.text;
                DumpLogCache();
                string logPath = Path.Combine(Environment.CurrentDirectory, "YuET_Data", "LogCache", "LogOutput.log");

                SubmitBugReport(username, friendcode, issueTitle, issueLong, logPath, sliderTemplate, template);
            }));

            var closeButton = Object.Instantiate(submitBtn, submitBtn.transform.parent);
            closeButton.gameObject.name = "CloseButton";
            closeButton.transform.Find("Text_TMP").GetComponent<TextMeshPro>().text = "";
            Object.Destroy(closeButton.transform.Find("Text_TMP").GetComponent<TextTranslatorTMP>());
            closeButton.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = LoadSprite("YuEzTools.Resources.Close.png", 100f);
            closeButton.transform.localPosition = new Vector3(-3.7f, 2.4f, 0);
            closeButton.transform.localScale = new Vector3(0.3f, 1.2f, 0);
            closeButton.OnClick = new ButtonClickedEvent();
            closeButton.OnClick.AddListener((Action)(() =>
            {
                Object.Destroy(sliderTemplate.gameObject);
                _bugButtonClickCount = 0;
            }));
        }
        else
        {
            ShowBugReportSuccessUI(template);
        }
    }

    private const string VikaApiToken = "uskFhEry1SQ13uKl7LPY7RX";
    private const string VikaDatasheetId = "dst15rMhG2qeb5jGDX";
    private const string VikaUploadUrl = $"https://api.vika.cn/fusion/v1/datasheets/{VikaDatasheetId}/attachments";
    private const string VikaRecordsUrl = $"https://api.vika.cn/fusion/v1/datasheets/{VikaDatasheetId}/records";

    private static void SubmitBugReport(string username, string friendcode, string issueTitle, string issueLong, string logPath, GameObject sliderTemplate, Transform template)
    {
        try
        {
            var fileData = UploadLogFile(logPath);
            if (fileData == null)
            {
                Error("Failed to upload log file", "BugReport");
                return;
            }

            var recordId = CreateVikaRecord(username, friendcode, fileData, issueTitle, issueLong);
            if (!string.IsNullOrEmpty(recordId))
            {
                Info($"Bug report submitted successfully! RecordId: {recordId}, Title: {issueTitle}", "BugReport");
            }
            else
            {
                Error("Failed to create Vika record", "BugReport");
            }
        }
        catch (Exception ex)
        {
            Error($"Bug report error: {ex.Message}", "BugReport");
        }
        finally
        {
            try
            {
                if (File.Exists(logPath))
                {
                    File.Delete(logPath);
                }
            }
            catch { }
        }

        Object.Destroy(sliderTemplate);
        ShowBugReportSuccessUI(template);
    }

    private static VikaFileData UploadLogFile(string logPath)
    {
        if (!File.Exists(logPath))
        {
            Warn($"Log file not found: {logPath}", "BugReport");
            return null;
        }

        var boundary = "----WebKitFormBoundary" + DateTime.Now.Ticks.ToString("x");
        var formData = new List<byte>();

        string fileName = "LogOutput.log";
        byte[] fileBytes = File.ReadAllBytes(logPath);

        string header = $"--{boundary}\r\nContent-Disposition: form-data; name=\"file\"; filename=\"{fileName}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
        formData.AddRange(Encoding.UTF8.GetBytes(header));
        formData.AddRange(fileBytes);
        formData.AddRange(Encoding.UTF8.GetBytes($"\r\n--{boundary}--\r\n"));

        var request = new UnityWebRequest(VikaUploadUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(formData.ToArray());
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {VikaApiToken}");
        request.SetRequestHeader("Content-Type", $"multipart/form-data; boundary={boundary}");

        var operation = request.SendWebRequest();
        while (!operation.isDone) { }

        string response = request.downloadHandler.text;
        
        if (request.result != UnityWebRequest.Result.Success)
        {
            Error($"Upload failed: {request.error}", "BugReport");
            Error($"Response: {response}", "BugReport");
            request.Dispose();
            return null;
        }

        request.Dispose();

        try
        {
            var jsonNode = JObject.Parse(response);
            if ((bool)jsonNode["success"])
            {
                var data = jsonNode["data"];
                return new VikaFileData
                {
                    id = data["id"] != null ? (string)data["id"] : "",
                    name = (string)data["name"],
                    size = (int)data["size"],
                    mimeType = (string)data["mimeType"],
                    token = (string)data["token"],
                    width = data["width"] != null ? (int)data["width"] : 0,
                    height = data["height"] != null ? (int)data["height"] : 0,
                    url = (string)data["url"]
                };
            }
            else
            {
                Error($"Upload API returned failure: {response}", "BugReport");
            }
        }
        catch (Exception ex)
        {
            Error($"Parse upload response failed: {ex.Message}", "BugReport");
            Error($"Response: {response}", "BugReport");
        }

        return null;
    }

    private static string CreateVikaRecord(string username, string friendcode, VikaFileData fileData, string issueTitle, string issueLong)
    {
        string json = $@"{{
    ""records"": [
        {{
            ""fields"": {{
                ""Name"": ""{EscapeJsonString(username)}"",
                ""FriendCode"": ""{EscapeJsonString(friendcode)}"",
                ""LogFile"": [
                    {{
                        ""token"": ""{fileData.token}"",
                        ""name"": ""{fileData.name}"",
                        ""size"": {fileData.size},
                        ""mimeType"": ""{fileData.mimeType}"",
                        ""url"": ""{fileData.url}""
                    }}
                ],
                ""IssueTitle"": ""{EscapeJsonString(issueTitle)}"",
                ""IssueLong"": ""{EscapeJsonString(issueLong)}""
            }}
        }}
    ],
    ""fieldKey"": ""name""
}}";

        var request = new UnityWebRequest(VikaRecordsUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {VikaApiToken}");
        request.SetRequestHeader("Content-Type", "application/json");

        var operation = request.SendWebRequest();
        while (!operation.isDone) { }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Error($"Create record failed: {request.error}", "BugReport");
            request.Dispose();
            return null;
        }

        string response = request.downloadHandler.text;
        request.Dispose();

        try
        {
            var jsonNode = JObject.Parse(response);
            if ((bool)jsonNode["success"])
            {
                return (string)jsonNode["data"]["records"][0]["recordId"];
            }
        }
        catch (Exception ex)
        {
            Error($"Parse record response failed: {ex.Message}", "BugReport");
        }

        return null;
    }

    private static string EscapeJsonString(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
    }

    private class VikaFileData
    {
        public string id;
        public string name;
        public int size;
        public string mimeType;
        public string token;
        public int width;
        public int height;
        public string url;

    }
    private static void DumpLogCache(){
        // 1. 定义日志文件的保存路径：
        string f = $"{Environment.CurrentDirectory}/YuET_Data/LogCache/";
        string filename = $"{f}LogOutput.log";
        
        // 4. 如果桌面的YuET-logs文件夹不存在，就创建它
        if (!Directory.Exists(f)) Directory.CreateDirectory(f);
        
        // 5. 定位到BepInEx的默认日志文件（LogOutput.log是BepInEx插件框架的核心日志文件）
        FileInfo file = new(@$"{Environment.CurrentDirectory}/BepInEx/LogOutput.log");
        // 6. 将原始日志文件复制到桌面的目标路径
        file.CopyTo(@filename, true);
    }

    private static void ShowBugReportSuccessUI(Transform template)
    {
        GameObject successTemplate = Object.Instantiate(template.gameObject, AccountManager.Instance.transform);
        successTemplate.name = "SCREEN";
        successTemplate.SetActive(true);

        successTemplate.transform.Find("TitleText_TMP").GetComponent<TextMeshPro>().text = GetString("BugReport.SuccessTitle");
        successTemplate.transform.Find("InfoText_TMP").GetComponent<TextMeshPro>().text = GetString("BugReport.SuccessInfo");
        successTemplate.transform.Find("InfoText_TMP").localPosition = new Vector3(0f, -1.1f, 0f);
        successTemplate.transform.Find("InfoText_TMP").localScale = new Vector3(2.5f, 2.5f, 1f);
        Object.Destroy(successTemplate.transform.Find("InfoText_TMP").GetComponent<TextTranslatorTMP>());
        Object.Destroy(successTemplate.transform.Find("TitleText_TMP").GetComponent<TextTranslatorTMP>());

        for (int i = 4; i <= 7; i++)
            successTemplate.transform.GetChild(i).gameObject.SetActive(false);
        successTemplate.transform.GetChild(9).gameObject.SetActive(false);

        var submitButton = successTemplate.transform.Find("SubmitButton").GetComponent<PassiveButton>();
        submitButton.transform.Find("Text_TMP").GetComponent<TextMeshPro>().text = GetString(StringNames.Okay);
        Object.Destroy(submitButton.transform.Find("Text_TMP").GetComponent<TextTranslatorTMP>());
        submitButton.OnClick = new ButtonClickedEvent();
        submitButton.OnClick.AddListener((Action)(() =>
        {
            Object.Destroy(successTemplate.gameObject);
        }));
    }
}