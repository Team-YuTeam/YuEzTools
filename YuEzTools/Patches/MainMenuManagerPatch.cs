using HarmonyLib;
using System;
using TMPro;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using YuEzTools.Updater;
using static YuEzTools.Translator;

namespace YuEzTools.UI;

[HarmonyPatch]
public class MainMenuManagerPatch
{
    public static MainMenuManager Instance { get; private set; }

    public static GameObject InviteButton;
    public static GameObject GithubButton;
    public static GameObject WebsiteButton;
    public static GameObject GiteeButton;
    public static GameObject DevsButton;
    // public static GameObject GitlabButton;
    // public static GameObject GitcodeButton;
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
    
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate)), HarmonyPostfix]
    public static void MainMenuManager_LateUpdate()
    {
        CustomPopup.Update();

        if (GameObject.Find("MainUI") == null) ShowingPanel = false;

        if (TitleLogoPatch.RightPanel != null)
        {
            var pos1 = TitleLogoPatch.RightPanel.transform.localPosition;
            Vector3 lerp1 = Vector3.Lerp(pos1, TitleLogoPatch.RightPanelOp + new Vector3((ShowingPanel ? 0f : 10f), 0f, 0f), Time.deltaTime * (ShowingPanel ? 3f : 2f));
            if (ShowingPanel
                ? TitleLogoPatch.RightPanel.transform.localPosition.x > TitleLogoPatch.RightPanelOp.x + 0.03f
                : TitleLogoPatch.RightPanel.transform.localPosition.x < TitleLogoPatch.RightPanelOp.x + 9f
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
            return button;
        }

    void CreateButton(GameObject button,string text, Action action,bool active,string name)
    {
        if (button == null) button = CreatButton(text, action);
        InviteButton.gameObject.SetActive(active);
        button.name = name;
    }


        var extraLinkName = "";
        var extraLinkUrl = "";
        var extraLinkEnabled = false;
        extraLinkName = "InviteButton";
        extraLinkUrl = TranslationController.Instance.currentLanguage.languageID == SupportedLangs.SChinese || TranslationController.Instance.currentLanguage.languageID == SupportedLangs.TChinese ? Main.QQUrl : Main.DcUrl;
        extraLinkEnabled = true;

        if (InviteButton == null) InviteButton = CreatButton(extraLinkName, () => { Application.OpenURL(extraLinkUrl); });
        InviteButton.gameObject.SetActive(extraLinkEnabled);
        InviteButton.name = "YuET Extra Link Button";

        if (WebsiteButton == null) WebsiteButton = CreatButton("WebsiteButton", () => Application.OpenURL("https://night-gua.github.io/"));
        WebsiteButton.gameObject.SetActive(true);
        WebsiteButton.name = "YuET Website Button";

        if(!Translator.IsChineseLanguageUser)
        {
            if (GithubButton == null)
                GithubButton = CreatButton("GithubButton",
                    () => Application.OpenURL("https://github.com/Team-YuTeam/YuEzTools/"));
            GithubButton.gameObject.SetActive(true);
            GithubButton.name = "YuET Github Button";
        }
        else{
            if (GiteeButton == null) GiteeButton = CreatButton("GiteeButton", () => Application.OpenURL("https://gitee.com/xigua_ya/YuEzTools/"));
            GiteeButton.gameObject.SetActive(true);
            GiteeButton.name = "YuET Gitee Button";
        }

        
        if (AfdianButton == null) AfdianButton = CreatButton("AfdianButton", () => Application.OpenURL("https://afdian.com/a/yuqianzhi"));
        AfdianButton.gameObject.SetActive(true);
        AfdianButton.name = "YuET Afdian Button";
        
        if (BilibiliButton == null) BilibiliButton = CreatButton("BiliBiliButton", () => Application.OpenURL("https://space.bilibili.com/1638639993"));
        BilibiliButton.gameObject.SetActive(true); 
        BilibiliButton.name = "YuET BiliBili Button";
        
        if (DevsButton == null) DevsButton = CreatButton("DevsButton", () =>
        {
            CustomPopup.Show(GetString("DevsTitle"),
                $"<color=#fffcbe>Yu</color> → <size=60%>{GetString("MainDev")}</size>\n" +
                $"<color=#F95C2A>Mousse</color> → <size=60%>{GetString("Dev")}</size>\n"
                // 有待添加
                , new()
            {
                (Translator.GetString(StringNames.Okay), null)
            });
        });
        DevsButton.gameObject.SetActive(true); 
        DevsButton.name = "YuET Devs Button";
        
        PlayButton = __instance.playButton.gameObject;
        if (UpdateButton == null)
        {
            
            UpdateButton = Object.Instantiate(PlayButton, PlayButton.transform.parent);
            UpdateButton.name = "YuET Update Button";
            UpdateButton.transform.localPosition = PlayButton.transform.localPosition - new Vector3(0f, 0f, 3f);
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
                        CustomPopup.Show(Translator.GetString("UpdateBySelfTitle"), Translator.GetString("UpdateBySelfText"), new()
                        {
                            (Translator.GetString(StringNames.Okay), null)
                        });
                    }
                }
            }));
            UpdateButton.transform.transform.FindChild("FontPlacer").GetChild(0).gameObject.DestroyTranslator();
        }
    }
}