using HarmonyLib;
using System;
using HarmonyLib;
using System.Collections.Generic;
using System.Text;
using TMPro;
using YuEzTools;
using UnityEngine;
using System.IO;
using System.Reflection;
using YuEzTools.Updater;
using TMPro;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using static YuEzTools.Translator;

namespace YuEzTools.UI;

[HarmonyPatch]
public class MainMenuManagerPatch
{
    public static MainMenuManager Instance { get; private set; }

    public static GameObject InviteButton;
    public static GameObject WebsiteButton;
    public static GameObject ProjectButton;
    public static GameObject DevsButton;
    // public static GameObject AfdianButton;
    // public static GameObject BilibiliButton;
    public static GameObject UpdateButton;
    public static GameObject SponsorButton;
    public static GameObject PlayButton;
    public static GameObject EightBallButton;

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
        
        // __instance.creditsButton.transform.localPosition += new Vector3(0, 0.7f, 0);
        // __instance.quitButton.transform.localPosition += new Vector3(0, 0.7f, 0);

    void CreateButton(GameObject button,string text, Action action,bool active,string name)
    {
        if (button == null) button = CreatButton(text, action);
        InviteButton.gameObject.SetActive(active);
        button.name = name;
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

        if (InviteButton == null) InviteButton = CreatButton(extraLinkName, () => { Application.OpenURL(extraLinkUrl); });
        InviteButton.gameObject.SetActive(extraLinkEnabled);
        InviteButton.name = "YuET Extra Link Button";
        

        if (WebsiteButton == null) WebsiteButton = CreatButton("WebsiteButton", () => Application.OpenURL("https://night-gua.github.io/"));
        WebsiteButton.gameObject.SetActive(true);
        WebsiteButton.name = "YuET Website Button";

        var ProjectLink = IsChineseLanguageUser
            ? "https://gitee.com/xigua_ya/YuEzTools/"
            : "https://github.com/Team-YuTeam/YuEzTools/";
        if (ProjectButton == null) ProjectButton = CreatButton("ProjectButton", () => Application.OpenURL(ProjectLink));
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

        var rando = IRandom.Instance;
        int result = rando.Next(0, 1);

        if (result == 0)
        {
            if (SponsorButton == null) SponsorButton = CreatButton("AfdianButton", () => Application.OpenURL("https://afdian.com/a/yuqianzhi"));
            SponsorButton.gameObject.SetActive(true);
            SponsorButton.name = "YuET Afdian Button";
        }
        else
        {
            if (SponsorButton == null) SponsorButton = CreatButton("BiliBiliButton", () => Application.OpenURL("https://space.bilibili.com/1638639993"));
            SponsorButton.gameObject.SetActive(true); 
            SponsorButton.name = "YuET BiliBili Button";
        }
        
        if (EightBallButton == null) EightBallButton = CreatButton("EightBallButton", () =>
        {
            CustomPopup.Show(GetString("EightBallTitle"), GetString("EightBallText")
                , new()
                {
                    (GetString(StringNames.Okay), () =>
                    {
                                    var rando = IRandom.Instance;
            int result = rando.Next(0, 16);
            string str = "";
                    switch (result)
                    {
                        case 0:
                            str = GetString("8BallYes");
                            break;
                        case 1:
                            str = GetString("8BallNo");
                            break;
                        case 2:
                            str = GetString("8BallMaybe");
                            break;
                        case 3:
                            str = GetString("8BallTryAgainLater");
                            break;
                        case 4:
                            str = GetString("8BallCertain");
                            break;
                        case 5:
                            str = GetString("8BallNotLikely");
                            break;
                        case 6:
                            str = GetString("8BallLikely");
                            break;
                        case 7:
                            str = GetString("8BallDontCount");
                            break;
                        case 8:
                            str = GetString("8BallStop");
                            break;
                        case 9:
                            str = GetString("8BallPossibly");
                            break;
                        case 10:
                            str = GetString("8BallProbably");
                            break;
                        case 11:
                            str = GetString("8BallProbablyNot");
                            break;
                        case 12:
                            str = GetString("8BallBetterNotTell");
                            break;
                        case 13:
                            str = GetString("8BallCantPredict");
                            break;
                        case 14:
                            str = GetString("8BallWithoutDoubt");
                            break;
                        case 15:
                            str = GetString("8BallWithDoubt");
                            break;
                    }
                    CustomPopup.Show(GetString("8BallTitle"), $"<size=8.5><color=#FF443D>{str}</color></size>"
                        , new()
                        {
                            (GetString(StringNames.Okay), null)
                        });
                    })
                });
        });
        EightBallButton.gameObject.SetActive(true);
        EightBallButton.name = "YuET Eight Ball Button";
        
        
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
        
        Dictionary<List<PassiveButton>, (Sprite, Color, Color, Color, Color)> customButtons = new()
        {
            {new List<PassiveButton>() {InviteButton.GetComponent<PassiveButton>(),WebsiteButton.GetComponent<PassiveButton>(),ProjectButton.GetComponent<PassiveButton>(),SponsorButton.GetComponent<PassiveButton>(),EightBallButton.GetComponent<PassiveButton>(),DevsButton.GetComponent<PassiveButton>()},
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