using HarmonyLib;
using System.Collections.Generic;
using System.Text;
using TMPro;
using YuEzTools;
using UnityEngine;
using System.IO;
using System.Reflection;
using YuEzTools.Updater;

namespace YuEzTools.UI;

[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start)), HarmonyPriority(Priority.First)]
internal class TitleLogoPatch
{
    public static GameObject ModStamp;
    public static GameObject YuET_Background;
    public static GameObject Ambience;
    public static GameObject Starfield;
    public static GameObject Sizer;
    public static GameObject AULogo;
    public static GameObject BottomButtonBounds;
    public static GameObject LeftPanel;
    public static GameObject RightPanel;
    public static GameObject CloseRightButton;
    public static GameObject Tint;

    public static Vector3 RightPanelOp;
    
    public static void showPopup(string text){
        var popup = GameObject.Instantiate(DiscordManager.Instance.discordPopup, Camera.main!.transform);
        
        var background = popup.transform.Find("Background").GetComponent<SpriteRenderer>();
        //var button = popup.transform.Find("ExitGame").GetComponent<SpriteRenderer>();
        var size = background.size;
        size.x *= 2.5f;
        size.y *= 2f;
        background.size = size;
        //button.GetComponent<AspectPosition>().anchorPoint = new(0.2f, 0.38f);

        popup.TextAreaTMP.fontSizeMin = 2;
        popup.Show(text);
    }
    
    private static void Postfix(MainMenuManager __instance)
    {
        GameObject.Find("BackgroundTexture")?.SetActive(!MainMenuManagerPatch.ShowedBak);
        
        if (!(ModStamp = GameObject.Find("ModStamp"))) return;
        ModStamp.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        
        YuET_Background = new GameObject("YuET Background");
        YuET_Background.transform.position = new Vector3(-0.0182f,0f, 520f);
        var bgRenderer = YuET_Background.AddComponent<SpriteRenderer>();
        bgRenderer.sprite = LoadSprite("YuEzTools.Resources.YuET-BG.jpg", 179f);//Bg
        
        if (!(Ambience = GameObject.Find("Ambience"))) return;
        if (!(Starfield = Ambience.transform.FindChild("starfield").gameObject)) return;
        StarGen starGen = Starfield.GetComponent<StarGen>();
        starGen.SetDirection(new Vector2(0, -2));
        Starfield.transform.SetParent(YuET_Background.transform);
        GameObject.Destroy(Ambience);
        Ambience.SetActive(false);
        
        if (!(LeftPanel = GameObject.Find("LeftPanel"))) return;
        LeftPanel.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        static void ResetParent(GameObject obj) => obj.transform.SetParent(LeftPanel.transform.parent);
        LeftPanel.ForEachChild((Il2CppSystem.Action<GameObject>)ResetParent);
        LeftPanel.SetActive(false);
        
        Color shade = new(0f, 0f, 0f, 0f);
        var standardActiveSprite = __instance.newsButton.activeSprites.GetComponent<SpriteRenderer>().sprite;
        var minorActiveSprite = __instance.quitButton.activeSprites.GetComponent<SpriteRenderer>().sprite;

        Dictionary<List<PassiveButton>, (Sprite, Color, Color, Color, Color)> mainButtons = new()
        {
            {new List<PassiveButton>() {__instance.playButton, __instance.inventoryButton, __instance.shopButton},
                (standardActiveSprite, new(0.57f, 0.99f, 1f, 0.8f), shade, Color.white, Color.white) },
            {new List<PassiveButton>() {__instance.newsButton, __instance.myAccountButton, __instance.settingsButton},
                (minorActiveSprite, new(0.09f, 0.917f, 0.67f, 0.8f), shade, Color.white, Color.white) },
            {new List<PassiveButton>() {__instance.creditsButton, __instance.quitButton},
                (minorActiveSprite, new(0.098f, 0.917f, 0.427f, 0.8f), shade, Color.white, Color.white) },
        };
        __instance.playButton.buttonText.color = Color.white;
        __instance.inventoryButton.buttonText.color = Color.white;
        __instance.shopButton.buttonText.color = Color.white;
        __instance.newsButton.buttonText.color = Color.white;
        __instance.myAccountButton.buttonText.color = Color.white;
        __instance.settingsButton.buttonText.color = Color.white;
        
        __instance.playButton.transform.localPosition += new Vector3(0, 0.7f, 0);
        __instance.inventoryButton.transform.localPosition += new Vector3(0, 0.7f, 0);
        __instance.shopButton.transform.localPosition += new Vector3(0, 0.7f, 0);
        __instance.myAccountButton.transform.localPosition += new Vector3(0, 0.7f, 0);
        __instance.newsButton.transform.localPosition += new Vector3(0, 0.7f, 0);
        __instance.settingsButton.transform.localPosition += new Vector3(0, 0.7f, 0);
        
        __instance.freePlayButton.gameObject.SetActive(false);
        __instance.howToPlayButton.gameObject.SetActive(false);
        __instance.creditsScreen.SetActive(false);
        __instance.creditsButton.gameObject.SetActive(false);
        

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

        foreach (var kvp in mainButtons)
            kvp.Key.Do(button => FormatButtonColor(button, kvp.Value.Item1, kvp.Value.Item2, kvp.Value.Item3, kvp.Value.Item4, kvp.Value.Item5));

        GameObject.Find("Divider")?.SetActive(false);
        
        if (!(RightPanel = GameObject.Find("RightPanel"))) return;
        var rpap = RightPanel.GetComponent<AspectPosition>();
        if (rpap) Object.Destroy(rpap);
        RightPanelOp = RightPanel.transform.localPosition;
        RightPanel.transform.localPosition = RightPanelOp + new Vector3(10f, 0f, 0f);
        RightPanel.GetComponent<SpriteRenderer>().color = new(1f, 0.78f, 0.9f, 1f);
        
        if (!(Sizer = GameObject.Find("Sizer"))) return;
        if (!(AULogo = GameObject.Find("LOGO-AU"))) return;
        Sizer.transform.localPosition += new Vector3(0f, 0.82f, 0f);
        AULogo.transform.localScale = new Vector3(0.66f, 0.67f, 1f);
        AULogo.transform.position += new Vector3(0f, 0.1f, 0f);
        var logoRenderer = AULogo.GetComponent<SpriteRenderer>();
        logoRenderer.sprite = LoadSprite("YuEzTools.Resources.YuET-Logo-tm.png",100f);//Yu的Logo

        if (!(BottomButtonBounds = GameObject.Find("BottomButtonBounds"))) return;
        BottomButtonBounds.transform.localPosition -= new Vector3(0f, 0.1f, 0f);

        
        CloseRightButton = new GameObject("CloseRightPanelButton");
        CloseRightButton.transform.SetParent(RightPanel.transform);
        CloseRightButton.transform.localPosition = new Vector3(-4.78f, 1.3f, 1f);
        CloseRightButton.transform.localScale = new(1f, 1f, 1f);
        CloseRightButton.AddComponent<BoxCollider2D>().size = new(0.6f, 1.5f);
        var closeRightSpriteRenderer = CloseRightButton.AddComponent<SpriteRenderer>();
        //closeRightSpriteRenderer.sprite = LoadSprite("YuEzTools.Resources.Images.RightPanelCloseButton.png", 100f);
        closeRightSpriteRenderer.color = new(1f, 0.78f, 0.9f, 1f);
        var closeRightPassiveButton = CloseRightButton.AddComponent<PassiveButton>();
        closeRightPassiveButton.OnClick = new();
        //closeRightPassiveButton.OnClick.AddListener((System.Action)MainMenuManagerPatch.HideRightPanel);
        closeRightPassiveButton.OnMouseOut = new();
        closeRightPassiveButton.OnMouseOut.AddListener((System.Action)(() => closeRightSpriteRenderer.color = new(1f, 0.78f, 0.9f, 1f)));
        closeRightPassiveButton.OnMouseOver = new();
        closeRightPassiveButton.OnMouseOver.AddListener((System.Action)(() => closeRightSpriteRenderer.color = new(1f, 0.68f, 0.99f, 1f)));

        Tint = __instance.screenTint.gameObject;
        var ttap = Tint.GetComponent<AspectPosition>();
        if (ttap) Object.Destroy(ttap);
        Tint.transform.SetParent(RightPanel.transform);
        Tint.transform.localPosition = new Vector3(-0.0824f, 0.0513f, Tint.transform.localPosition.z);
        Tint.transform.localScale = new Vector3(1f, 1f, 1f);
        
        var creditsScreen = __instance.creditsScreen;
        if (creditsScreen)
        {
            var csto = creditsScreen.GetComponent<TransitionOpen>();
            if (csto) Object.Destroy(csto);
            var closeButton = creditsScreen.transform.FindChild("CloseButton");
            closeButton?.gameObject.SetActive(false);
        }
        
    }
    
    public static Dictionary<string, Sprite> CachedSprites = new();
    public static Sprite LoadSprite(string path, float pixelsPerUnit = 1f)
    {
        try
        {
            if (CachedSprites.TryGetValue(path + pixelsPerUnit, out var sprite)) return sprite;
            Texture2D texture = LoadTextureFromResources(path);
            sprite = Sprite.Create(texture, new(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
            return CachedSprites[path + pixelsPerUnit] = sprite;
        }
        catch
        {
            
        }
        return null;
    }
    
    public static Texture2D LoadTextureFromResources(string path)
    {
        try
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            var texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            using MemoryStream ms = new();
            stream.CopyTo(ms);
            ImageConversion.LoadImage(texture, ms.ToArray(), false);
            return texture;
        }
        catch
        {
        }
        return null;
    }
}
[HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))] 
public static class VersionShower_Start
{
    public static void Postfix(VersionShower __instance)
    {
        __instance.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        if (Main.ModMode == 1)
        {
            //__instance.text.text = TranslationController.Instance.currentLanguage.languageID == SupportedLangs.SChinese || TranslationController.Instance.currentLanguage.languageID == SupportedLangs.TChinese ? $"<color={Main.ModColor}>{Main.ModName}</color> (<color=#DC143C>您正在使用 v{Main.PluginVersion} Canary测试版！</color>)" : $"<color={Main.ModColor}>{Main.ModName}</color> (<color=#DC143C>You are using  v{Main.PluginVersion} Canary Version</color>)";
            if(Translator.IsChineseUser) __instance.text.text = string.Format(Translator.GetString("UsingVersion"),Main.ModColor,Main.ModName,"Canary") + string.Format(Translator.GetString("VerShow.Visit"),Main.ModColor,Main.ModName,ModUpdater.visit);
            else __instance.text.text = string.Format(Translator.GetString("VerShow.HasNotUpdate"),Main.ModColor,Main.ModName);
        }
        else if (Main.ModMode == 0)
        {
            //__instance.text.text = TranslationController.Instance.currentLanguage.languageID == SupportedLangs.SChinese || TranslationController.Instance.currentLanguage.languageID == SupportedLangs.TChinese ? $"<color={Main.ModColor}>{Main.ModName}</color> (<color=#DC143C>您正在使用 v{Main.PluginVersion} Debug开发者版！</color>)" : $"<color={Main.ModColor}>{Main.ModName}</color> (<color=#DC143C>You are using  v{Main.PluginVersion} Debug Version</color>)";
            if(Translator.IsChineseUser) __instance.text.text = string.Format(Translator.GetString("UsingVersion"),Main.ModColor,Main.ModName,"Debug") + string.Format(Translator.GetString("VerShow.Visit"),Main.ModColor,Main.ModName,ModUpdater.visit);
            else __instance.text.text = string.Format(Translator.GetString("VerShow.HasNotUpdate"),Main.ModColor,Main.ModName);
            //__instance.text.text += "\n" + string.Format(Translator.GetString("VerShow.Visit"),Main.ModColor,Main.ModName,ModUpdater.visit);
        }
        else
        {
            if (ModUpdater.hasUpdate)
            {
                __instance.text.text = string.Format(Translator.GetString("VerShow.HasUpdate"),Main.ModColor,Main.ModName,ModUpdater.latestVersion);
                //else __instance.text.text += "\n" + string.Format(Translator.GetString("VerShow.HasNotUpdate"),Main.ModColor,Main.ModName);
                //__instance.text.text += "\n" + string.Format(Translator.GetString("VerShow.Visit"),Main.ModColor,Main.ModName,ModUpdater.visit);
            }
            else
            {
                if(Translator.IsChineseUser) __instance.text.text = string.Format(Translator.GetString("VerShow.Visit"),Main.ModColor,Main.ModName,ModUpdater.visit);
                else __instance.text.text = string.Format(Translator.GetString("VerShow.HasNotUpdate"),Main.ModColor,Main.ModName);
                //__instance.text.text += "\n" + string.Format(Translator.GetString("VerShow.Visit"),Main.ModColor,Main.ModName,ModUpdater.visit);
            }
        } 
    }
}
