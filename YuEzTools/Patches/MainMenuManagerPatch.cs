using System;
using TMPro;
using UnityEngine;
using YuEzTools.Helpers;
using YuEzTools.Modules;
using YuEzTools.UI;
using Object = UnityEngine.Object;

namespace YuEzTools.Patches;

[HarmonyPatch]
public class MainMenuManagerPatch
{
    public static MainMenuManager Instance { get; private set; }

    public static GameObject InviteButton;
    public static GameObject WebsiteButton;
    public static GameObject ProjectButton;
    public static GameObject DevsButton;
    public static GameObject AfdianButton;
    public static GameObject BilibiliButton;
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

        var s = "";
        if (DevsButton == null) DevsButton = CreatButton("DevsButton", () =>
        {
            s = "";
            foreach (var dev in DevManager.DevUserList)
            {
                if (dev.Jobs != "NotJob")
                    s += $"<color={dev.Color}>{dev.Name}</color> => <size=60%>{GetString(dev.Jobs)}</size>\n";
            }
            CustomPopup.Show(GetString("DevsTitle"), s
                , new()
                {
                    (GetString(StringNames.Okay), null)
                });
        });
        DevsButton.gameObject.SetActive(true);
        DevsButton.name = "YuET Devs Button";

        if (AfdianButton == null) AfdianButton = CreatButton("AfdianButton", () => OpenURL.OpenUrl("https://afdian.com/a/yuqianzhi"));
        AfdianButton.gameObject.SetActive(true);
        AfdianButton.name = "YuET Afdian Button";

        if (BilibiliButton == null) BilibiliButton = CreatButton("BiliBiliButton", () => OpenURL.OpenUrl("https://space.bilibili.com/1638639993"));
        BilibiliButton.gameObject.SetActive(true);
        BilibiliButton.name = "YuET BiliBili Button";

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
            {new List<PassiveButton>() {InviteButton.GetComponent<PassiveButton>(),WebsiteButton.GetComponent<PassiveButton>(),ProjectButton.GetComponent<PassiveButton>(),AfdianButton.GetComponent<PassiveButton>(),BilibiliButton.GetComponent<PassiveButton>(),DevsButton.GetComponent<PassiveButton>()},
                (minorActiveSprite, new(0.65f, 1f, 0.247f, 0.8f), shade, Color.white, Color.white) },
        };

        foreach (var kvp in customButtons)
            kvp.Key.Do(button => FormatButtonColor(button, kvp.Value.Item1, kvp.Value.Item2, kvp.Value.Item3, kvp.Value.Item4, kvp.Value.Item5));

        // GameObject.Destroy(__instance.creditsButton.gameObject);
        // GameObject.Destroy(__instance.quitButton.gameObject);
        // var BottomButtonBounds = GameObject.Find("BottomButtonBounds");
        // BottomButtonBounds.transform.localPosition += new Vector3(0, 0.8f, 0);
    }
}