using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Globalization;
using System.Reflection;
using UnityEngine;
using static CloudGenerator;
using UnityEngine.Playables;
using Il2CppSystem.IO;
using YuAntiCheat.Get;
using YuAntiCheat.Attributes;
using YuAntiCheat.UI;
using YuAntiCheat.Utils;

[assembly: AssemblyFileVersion(YuAntiCheat.Main.PluginVersion)]
[assembly: AssemblyInformationalVersion(YuAntiCheat.Main.PluginVersion)]
[assembly: AssemblyVersion(YuAntiCheat.Main.PluginVersion)]
namespace YuAntiCheat;

[BepInPlugin(PluginGuid, "YuAntiCheat", PluginVersion)]
[BepInProcess("Among Us.exe")]
public class Main : BasePlugin
{

    public static readonly string ModName = "YuAntiCheat"; // 咱们的模组名字
    public static readonly string ModColor = "#fffcbe"; // 咱们的模组颜色
    public static readonly string MainMenuText = "外挂根本就不可能存在好嘛~"; // 咱们模组的首页标语
    public const string PluginGuid = "com.Yu.YuAntiCheat"; //咱们模组的Guid
    public const string PluginVersion = "1.1.13"; //咱们模组的版本号
    public const string CanUseInAmongUsVer = "2024.6.18"; //智齿的AU版本
    public const int PluginCreation = 1;

    public static string QQUrl = "https://qm.qq.com/q/aW5s4sDsHu";
    public static string DcUrl = "https://discord.gg/9Jy7gzPq";
    
    public static ConfigEntry<string> menuKeybind;
    public static ConfigEntry<string> menuHtmlColor;
    public static MenuUI menuUI;
    
    public static System.Version version = System.Version.Parse(PluginVersion);
        
    public static int ModMode { get; private set; } =
#if DEBUG
0;
#elif CANARY
        1;
    #else
    2;
#endif
    
    public Harmony Harmony { get; } = new Harmony(PluginGuid);

    public static BepInEx.Logging.ManualLogSource Logger;
    
    public static IEnumerable<PlayerControl> AllPlayerControls => PlayerControl.AllPlayerControls.ToArray().Where(p => p != null);
    
    public static Main Instance; //设置Main实例
    
    public static bool IsChineseUser => Translator.GetUserLangByRegion() == SupportedLangs.SChinese;
    
    public static bool VisibleTasksCount = false;
    
    //public static bool safemode = true;//设置安全模式
    //public static bool ShowMode = true;//设置揭示模式
    
    //public static Dictionary<int, PlayerState> PlayerStates = new Dictionary<int, PlayerState>();
    
    public static ConfigEntry<string> BetaBuildURL { get; private set; }
    public override void Load()//加载 启动！
    {
        Instance = this; //Main实例

        menuKeybind = Config.Bind("YuAC.GUI",
            "Keybind",
            "Delete",
            "The keyboard key used to toggle the GUI on and off. List of supported keycodes: https://docs.unity3d.com/Packages/com.unity.tiny@0.16/api/Unity.Tiny.Input.KeyCode.html");
        menuHtmlColor = Config.Bind("YuAC.GUI",
            "Color",
            "",
            "A custom color for your YuAC GUI. Supports html color codes");

        menuUI = AddComponent<MenuUI>();

        ResourceUtils.WriteToFileFromResource(
            "BepInEx/core/YamlDotNet.dll",
            "YuAntiCheat.Resources.InDLL.Depends.YamlDotNet.dll");
        ResourceUtils.WriteToFileFromResource(
            "BepInEx/core/YamlDotNet.xml",
            "YuAntiCheat.Resources.InDLL.Depends.YamlDotNet.xml");
        
        //Translator.Init();
        
        BetaBuildURL = Config.Bind("Other", "BetaBuildURL", "");
        
        Logger = BepInEx.Logging.Logger.CreateLogSource("YuAntiCheat"); //输出前缀 设置！
        YuAntiCheat.Logger.Enable();

        PluginModuleInitializerAttribute.InitializeAll();
        
        if (Application.version == CanUseInAmongUsVer)
            Logger.LogInfo($"AmongUs Version: {Application.version}"); //牢底居然有智齿的版本？！
        else
            Logger.LogInfo($"游戏本体版本过低或过高,AmongUs Version: {Application.version}"); //牢底你的版本也不行啊
        RegistryManager.Init(); // 这是优先级最高的模块初始化方法，不能使用模块初始化属性
        //各组件初始化
        Harmony.PatchAll();
        if(ModMode != 0) ConsoleManager.DetachConsole();
        //模组加载好了标语
        YuAntiCheat.Logger.Msg("========= YuAC loaded! =========", "YuAC Plugin Load");
    }
}