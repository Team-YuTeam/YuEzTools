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
using YuEzTools.Get;
using YuEzTools.Attributes;
using YuEzTools.UI;
using YuEzTools.Utils;

[assembly: AssemblyFileVersion(YuEzTools.Main.PluginVersion)]
[assembly: AssemblyInformationalVersion(YuEzTools.Main.PluginVersion)]
[assembly: AssemblyVersion(YuEzTools.Main.PluginVersion)]
namespace YuEzTools;

[BepInPlugin(PluginGuid, "YuEzTools", PluginVersion)]
[BepInProcess("Among Us.exe")]
public class Main : BasePlugin
{

    public static readonly string ModName = "YuEzTools"; // 咱们的模组名字
    public static readonly string ModColor = "#fffcbe"; // 咱们的模组颜色
    public static readonly string MainMenuText = "Who fix?No one!!"; // 咱们模组的首页标语
    public const string PluginGuid = "com.Yu.YuEzTools"; //咱们模组的Guid
    public const string PluginVersion = "1.3.0.5"; //咱们模组的版本号
    public const string CanUseInAmongUsVer = "2024.6.18"; //智齿的AU版本
    public const int PluginCreation = 1;

    public static string QQUrl = "https://qm.qq.com/q/uGuWqBkYUi";
    public static string DcUrl = "https://discord.gg/9Jy7gzPq";

    public static bool HasHacker = false;
    
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
    
    public static bool isChatCommand = false;
    
    public Harmony Harmony { get; } = new Harmony(PluginGuid);

    public static BepInEx.Logging.ManualLogSource Logger;
    
    public static IEnumerable<PlayerControl> AllPlayerControls => PlayerControl.AllPlayerControls.ToArray().Where(p => p != null);
    
    public static List<PlayerControl> ClonePlayerControlsOnStart => AllPlayerControls.ToList();
    
    public static Main Instance; //设置Main实例

    public static bool isFirstSendEnd = false;
    
    public static bool IsChineseUser => Translator.GetUserLangByRegion() == SupportedLangs.SChinese;
    
    public static bool VisibleTasksCount = false;
    
    //public static bool safemode = true;//设置安全模式
    //public static bool ShowMode = true;//设置揭示模式
    
    public static List<(string, byte, string)> MessagesToSend = new();

    public static IEnumerable<PlayerControl> AllAlivePlayerControls =>
        //(PlayerControl.AllPlayerControls == null || PlayerControl.AllPlayerControls.Count == 0) && LoadEnd
        //? AllAlivePlayerControls :
        PlayerControl.AllPlayerControls.ToArray().Where(p => p != null);
    
    //public static Dictionary<int, PlayerState> PlayerStates = new Dictionary<int, PlayerState>();
    
    public static ConfigEntry<string> BetaBuildURL { get; private set; }
    public override void Load()//加载 启动！
    {
        Instance = this; //Main实例
        
            ResourceUtils.WriteToFileFromResource(
                "BepInEx/core/YamlDotNet.dll",
                "YuEzTools.Resources.InDLL.Depends.YamlDotNet.dll");
            ResourceUtils.WriteToFileFromResource(
                "BepInEx/core/YamlDotNet.xml",
                "YuEzTools.Resources.InDLL.Depends.YamlDotNet.xml");
        
        PluginModuleInitializerAttribute.InitializeAll();
        
        Logger = BepInEx.Logging.Logger.CreateLogSource("YuEzTools"); //输出前缀 设置！
        YuEzTools.Logger.Enable();
        
        menuKeybind = Config.Bind("YuET.GUI",
            "Keybind",
            "Delete",
            "The keyboard key used to toggle the GUI on and off. List of supported keycodes: https://docs.unity3d.com/Packages/com.unity.tiny@0.16/api/Unity.Tiny.Input.KeyCode.html");
        menuHtmlColor = Config.Bind("YuET.GUI",
            "Color",
            "",
            "A custom color for your YuET GUI. Supports html color codes");

        menuUI = AddComponent<MenuUI>();
        
        //Translator.Init();
        
        BetaBuildURL = Config.Bind("Other", "BetaBuildURL", "");
        
        if (Application.version == CanUseInAmongUsVer)
            Logger.LogInfo($"AmongUs Version: {Application.version}"); //牢底居然有智齿的版本？！
        else
            Logger.LogInfo($"游戏本体版本过低或过高,AmongUs Version: {Application.version}"); //牢底你的版本也不行啊
        RegistryManager.Init(); // 这是优先级最高的模块初始化方法，不能使用模块初始化属性
        //各组件初始化
        Harmony.PatchAll();
        if(ModMode != 0) ConsoleManager.DetachConsole();
        //模组加载好了标语
        YuEzTools.Logger.Msg("========= YuET loaded! =========", "YuET Plugin Load");
    }
}

public enum RoleTeam
{
    Crewmate,
    Impostor,
    Error
}
