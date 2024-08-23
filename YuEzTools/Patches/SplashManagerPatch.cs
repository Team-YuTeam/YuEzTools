using System.Collections;
using BepInEx;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using TMPro;
using UnityEngine;
using YuEzTools.Attributes;
using YuEzTools.UI;
using YuEzTools.Utils;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(SplashManager), nameof(SplashManager.Start))]
public static class SplashManagerPatch
{
    public static bool LanguageLoaded = false;
    static SpriteLoader logoSprite = SpriteLoader.FromResource("YuEzTools.Resources.YuET-Logo-tm.png", 200f);
    // static SpriteLoader logoGlowSprite = SpriteLoader.FromResource("YuEzTools.Resources.Yu-Logo-tm.png", 200f);
    static TextMeshPro loadText = null!;
    public static bool Prefix(SplashManager __instance)
    {
        __instance.logoAnimFinish.transform.FindChild("LogoRoot").FindChild("ISLogo").GetComponent<SpriteRenderer>().sprite = logoSprite.GetSprite();
        
        loadText = GameObject.Instantiate(__instance.errorPopup.InfoText,  __instance.logoAnimFinish.transform.FindChild("LogoRoot").FindChild("ISLogo"));
        loadText.transform.localPosition = new(3, __instance.logoAnimFinish.transform.FindChild("LogoRoot").FindChild("ISLogo").position.y -1.18f, 0);
        loadText.fontStyle = TMPro.FontStyles.Bold;
        loadText.text = "欢迎使用YuET!正在准备加载...\n<size=65%>Welcome YuET!Wait for Loading...</size>";
        loadText.color = Color.white.AlphaMultiplied(0.3f);
        loadText.SetActive(__instance.logoAnimFinish.enabled);
        // Main.LoadMain();
        __instance.StartCoroutine(CoLoadYuET(__instance).WrapToIl2Cpp());
        return true;
    }

    static IEnumerator CoLoadYuET(SplashManager __instance)
    {
        yield return new WaitForSeconds(2.5f);
        Logger.Info("Loading...","Load");
        loadText.text = "正在存放必要的文件\n<size=65%>The necessary documents are being stored</size>";
        ResourceUtils.WriteToFileFromResource(
            "BepInEx/core/YamlDotNet.dll",
            "YuEzTools.Resources.InDLL.Depends.YamlDotNet.dll");
        ResourceUtils.WriteToFileFromResource(
            "BepInEx/core/YamlDotNet.xml",
            "YuEzTools.Resources.InDLL.Depends.YamlDotNet.xml");
        yield return new WaitForSeconds(0.5f);
        
        loadText.text = "加载多语言\n<size=65%>Loading Language</size>";
        PluginModuleInitializerAttribute.InitializeAll();
        LanguageLoaded = true;
        yield return new WaitForSeconds(0.3f);
        
        loadText.text = "加载配置文件\n<size=65%>Loading Config</size>";
        Toggles.WinTextSize = Main.WinTextSize.Value;
        yield return new WaitForSeconds(0.25f);
        
        //Translator.Init();
        
        loadText.text = "检查AmongUs版本\n<size=65%>Check AmongUs Version</size>";
        if (Application.version == Main.CanUseInAmongUsVer)
            Logger.Info($"AmongUs Version: {Application.version}","AmongUsVersionCheck"); //牢底居然有智齿的版本？！
        else
            Logger.Info($"游戏本体版本过低或过高,AmongUs Version: {Application.version}","AmongUsVersionCheck"); //牢底你的版本也不行啊
        yield return new WaitForSeconds(0.2f);
        
        loadText.text = "启用/禁用控制台\n<size=65%>Enable Console or Disable</size>";
        yield return new WaitForSeconds(0.175f);
        if (Main.ModMode != 0) ConsoleManager.DetachConsole();
        else ConsoleManager.CreateConsole();
        
        loadText.text = "加载开发组名单\n<size=65%>Loading Devs List</size>";
        DevManager.Init();
        yield return new WaitForSeconds(0.15f);
        //模组加载好了标语
        loadText.text = "完成...\n<size=65%>Finished</size>";
        yield return new WaitForSeconds(0.2f);
        YuEzTools.Logger.Msg("========= YuET loaded! =========", "YuET Plugin Load");
        yield return null;
    }
    
}