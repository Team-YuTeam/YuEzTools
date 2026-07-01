using System.Globalization;
using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using UnityEngine;
using YuEzTools.Attributes;
using YuEzTools.Modules;
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
    public static readonly string ModName = "YuEzTools"; // 模组名
    public static readonly string ModColor = "#A0E4FF"; // 模组颜色-Hex
    public static readonly Color32 ModColor32 = new(160, 228, 255,255); // 模组颜色-Color32
    public static readonly string MainMenuText = "We will be a star on the sky!\n<color=#ff0000>Happy 2026 New Year!</color>"; // 模组Ping部分标语
    public const string PluginGuid = "com.Yu.YuEzTools"; //模组Guid
    public const string PluginVersion = "2.0.0.0"; //模组版本号
    public const string CanUseInAmongUsVer = "2025.11.18"; // 适配的AU版本
    public const int PluginCreation = 1;

    public static string QQUrl = "https://qm.qq.com/q/uGuWqBkYUi";
    public static string DcUrl = "https://discord.gg/42tyx9FyD7";

    public static bool HasHacker = false;

    public static NormalGameOptionsV10 NormalOptions => GameOptionsManager.Instance.currentNormalGameOptions;
    public static HideNSeekGameOptionsV10 HideNSeekOptions => GameOptionsManager.Instance.currentHideNSeekGameOptions;

    public static ConfigEntry<string> menuKeybind;
    public static ConfigEntry<string> menuHtmlColor;
    public static MenuUI menuUI;
    public static FloatingButton floatingButton;
    public static CustomTips CustomTips;
    public static ConfigEntry<bool> PatchAccount;
    public static ConfigEntry<bool> PatchChat;
    public static ConfigEntry<bool> PatchCosmetics;
    public static ConfigEntry<bool> WinTextSize;
    public static ConfigEntry<bool> SwitchVanilla;
    public static ConfigEntry<bool> ShowPlayTimes;
    public static ConfigEntry<bool> AutoInstallServers;
    
    public static ConfigEntry<bool> DarkModeConfig;
    public static ConfigEntry<bool> ShowCommitConfig;
    public static ConfigEntry<bool> ShowModTextConfig;
    public static ConfigEntry<bool> ShowIsSafeConfig;
    public static ConfigEntry<bool> ShowIsDarkConfig;
    public static ConfigEntry<bool> ShowPingConfig;
    public static ConfigEntry<bool> ShowFPSConfig;
    public static ConfigEntry<bool> ShowServerConfig;
    public static ConfigEntry<bool> ShowIsAutoExitConfig;
    public static ConfigEntry<bool> ShowRoomTimeConfig;
    public static ConfigEntry<bool> ShowUTCConfig;
    public static ConfigEntry<bool> ShowLocalNowTimeConfig;
    public static ConfigEntry<bool> ShowGMConfig;
    public static ConfigEntry<bool> EnableAntiCheatConfig;
    public static ConfigEntry<bool> SafeModeConfig;
    public static ConfigEntry<bool> AutoExitConfig;
    public static ConfigEntry<bool> KickNotLoginConfig;
    public static ConfigEntry<bool> AutoReportHackerConfig;
    public static ConfigEntry<bool> DumpLogConfig;
    public static ConfigEntry<bool> OpenGameDicConfig;
    public static ConfigEntry<bool> CloseMusicOfOrConfig;
    public static ConfigEntry<bool> reShowRoleTConfig;
    public static ConfigEntry<bool> ShowInfoInLobbyConfig;
    public static ConfigEntry<bool> ExitGameConfig;
    public static ConfigEntry<bool> RealBanConfig;
    public static ConfigEntry<bool> HorseModeConfig;
    public static ConfigEntry<bool> LongModeConfig;
    public static ConfigEntry<bool> ChangeDownTimerToZeroConfig;
    public static ConfigEntry<bool> AbolishDownTimerConfig;
    public static ConfigEntry<bool> AutoStartGameConfig;
    public static ConfigEntry<bool> ServerAllHostOrNoHostConfig;
    public static ConfigEntry<bool> ChangeDownTimerTo114514Config;
    public static ConfigEntry<bool> FPSPlusConfig;
#if DEBUG
    public static ConfigEntry<bool> NotEndGameConfig;
#endif

    // public static List<string> TName_Snacks_CN = new() { "冰激凌", "奶茶", "巧克力", "蛋糕", "甜甜圈", "可乐", "柠檬水", "冰糖葫芦", "果冻", "糖果", "牛奶", "抹茶", "烧仙草", "菠萝包", "布丁", "椰子冻", "曲奇", "红豆土司", "三彩团子", "艾草团子", "泡芙", "可丽饼", "桃酥", "麻薯", "鸡蛋仔", "马卡龙", "雪梅娘", "炒酸奶", "蛋挞", "松饼", "西米露", "奶冻", "奶酥", "可颂", "奶糖" ,"咔皮呆","Yu","Night瓜","慕斯Mousse","毒液","Slok7565","喜","乐崽"};
    // public static List<string> TName_Snacks_EN = new() { "Ice cream", "Milk tea", "Chocolate", "Cake", "Donut", "Coke", "Lemonade", "Candied haws", "Jelly", "Candy", "Milk", "Matcha", "Burning Grass Jelly", "Pineapple Bun", "Pudding", "Coconut Jelly", "Cookies", "Red Bean Toast", "Three Color Dumplings", "Wormwood Dumplings", "Puffs", "Can be Crepe", "Peach Crisp", "Mochi", "Egg Waffle", "Macaron", "Snow Plum Niang", "Fried Yogurt", "Egg Tart", "Muffin", "Sago Dew", "panna cotta", "soufflé", "croissant", "toffee" ,"KARPED1EM","Yu","Night-GUA","Mousse","Farewell","Slok7565","Xi","LezaiYa"};

    public static System.Version version = System.Version.Parse(PluginVersion);

    public static ModMode ModMode { get; private set; } =
#if DEBUG
        ModMode.Debug;
#elif CANARY
        ModMode.Canary;
#else
        ModMode.Release;
#endif

    public static bool isChatCommand = false;

    // public static List<PlayerControl> JoinedPlayer = new();

    public Harmony Harmony { get; } = new Harmony(PluginGuid);

    public static BepInEx.Logging.ManualLogSource Logger;

    public static IEnumerable<PlayerControl> AllPlayerControls => PlayerControl.AllPlayerControls.ToArray().Where(p => p != null);

    public static List<PlayerControl> ClonePlayerControlsOnStart => [.. AllPlayerControls];

    public static Main Instance; //设置Main实例

    public static bool isFirstSendEnd = false;

    public static bool IsChineseUser => GetUserLangByRegion() == SupportedLangs.SChinese;
    public static bool isChineseBySystem()
    {
        try
        {
            var name = CultureInfo.CurrentUICulture.Name;
            if (name.StartsWith("zh")) return true;
            return false;
        }
        catch
        {
            return false;
        }
    }

    public static bool VisibleTasksCount = false;

    public static bool isAmongUsVersionOK = true;

    //public static bool safemode = true;//设置安全模式
    //public static bool ShowMode = true;//设置揭示模式

    public static List<(string, byte, string)> MessagesToSend = [];
    public static List<PlayerControl> HackerList = [];

    public static IEnumerable<PlayerControl> AllAlivePlayerControls =>
        //(PlayerControl.AllPlayerControls == null || PlayerControl.AllPlayerControls.Count == 0) && LoadEnd
        //? AllAlivePlayerControls :
        PlayerControl.AllPlayerControls.ToArray().Where(p => p != null);

    //public static Dictionary<int, PlayerState> PlayerStates = new Dictionary<int, PlayerState>();
    
    public static ConfigEntry<string> BetaBuildURL { get; private set; }
    public override void Load()//加载 启动！
    {
        Instance = this; //Main实例

        #if Windows
        ResourceUtils.WriteToFileFromResource(
            "BepInEx/core/YamlDotNet.dll",
            "YuEzTools.Resources.InDLL.Depends.YamlDotNet.dll");
        ResourceUtils.WriteToFileFromResource(
            "BepInEx/core/YamlDotNet.xml",
            "YuEzTools.Resources.InDLL.Depends.YamlDotNet.xml");
#elif Android   
        ResourceUtils.WriteToFileFromResource(
            "/data/data/dev.allofus.starlight/files/BepInEx/core/YamlDotNet.dll",
            "YuEzTools.Resources.InDLL.Depends.YamlDotNet.dll");
        ResourceUtils.WriteToFileFromResource(
            "/data/data/dev.allofus.starlight/files/BepInEx/core/YamlDotNet.xml",
            "YuEzTools.Resources.InDLL.Depends.YamlDotNet.xml");
#endif

        AutoInstallServers = Config.Bind("Server", "AutoInstallServers", true, "Auto install custom servers on startup");

        PluginModuleInitializerAttribute.InitializeAll();

        Logger = BepInEx.Logging.Logger.CreateLogSource("YuEzTools"); //输出前缀 设置！
        Enable();

        menuKeybind = Config.Bind("YuET.GUI",
            "Keybind",
            "Delete",
            "The keyboard key used to toggle the GUI on and off. List of supported keycodes: https://docs.unity3d.com/Packages/com.unity.tiny@0.16/api/Unity.Tiny.Input.KeyCode.html");
        menuHtmlColor = Config.Bind("YuET.GUI",
            "Color",
            "",
            "A custom color for your YuET GUI. Supports html color codes");
        PatchAccount = Config.Bind("Patches", "AccountPatches", true, "Enable account-related patches");
        PatchChat = Config.Bind("Patches", "ChatPatches", true, "Enable chat-related patches");
        PatchCosmetics = Config.Bind("Patches", "CosmeticPatches", true, "Enable cosmetic-related patches");
        WinTextSize = Config.Bind("WinText", "WinTextSize", false, "The Winner big(true) or the reason big(false)");
        SwitchVanilla = Config.Bind("Client Options", "SwitchVanilla", false);
        ShowPlayTimes = Config.Bind("Interface", "ShowPlayTimes", true, "Show play times on main menu");
        
        DarkModeConfig = Config.Bind("Interface", "DarkMode", true);
        ShowCommitConfig = Config.Bind("Interface", "ShowCommit", false);
        ShowModTextConfig = Config.Bind("Interface", "ShowModText", true);
        ShowIsSafeConfig = Config.Bind("Interface", "ShowIsSafe", true);
        ShowIsDarkConfig = Config.Bind("Interface", "ShowIsDark", true);
        ShowPingConfig = Config.Bind("Interface", "ShowPing", true);
        ShowFPSConfig = Config.Bind("Interface", "ShowFPS", true);
        ShowServerConfig = Config.Bind("Interface", "ShowServer", true);
        ShowIsAutoExitConfig = Config.Bind("Interface", "ShowIsAutoExit", true);
        ShowRoomTimeConfig = Config.Bind("Interface", "ShowRoomTime", true);
        ShowUTCConfig = Config.Bind("Interface", "ShowUTC", false);
        ShowLocalNowTimeConfig = Config.Bind("Interface", "ShowLocalNowTime", false);
        ShowGMConfig = Config.Bind("Interface", "ShowGM", true);
        ShowInfoInLobbyConfig = Config.Bind("Interface", "ShowInfoInLobby", false);
        
        EnableAntiCheatConfig = Config.Bind("AntiCheat", "EnableAntiCheat", true);
        SafeModeConfig = Config.Bind("AntiCheat", "SafeMode", true);
        AutoExitConfig = Config.Bind("AntiCheat", "AutoExit", true);
        KickNotLoginConfig = Config.Bind("AntiCheat", "KickNotLogin", true);
        AutoReportHackerConfig = Config.Bind("AntiCheat", "AutoReportHacker", false);
        ServerAllHostOrNoHostConfig = Config.Bind("AntiCheat", "ServerAllHostOrNoHost", false);
        
        DumpLogConfig = Config.Bind("ShortcutButton", "DumpLog", false);
        OpenGameDicConfig = Config.Bind("ShortcutButton", "OpenGameDic", false);
        CloseMusicOfOrConfig = Config.Bind("ShortcutButton", "CloseMusicOfOr", false);
        reShowRoleTConfig = Config.Bind("ShortcutButton", "reShowRoleT", false);
        ExitGameConfig = Config.Bind("ShortcutButton", "ExitGame", false);
        RealBanConfig = Config.Bind("ShortcutButton", "RealBan", false);
        HorseModeConfig = Config.Bind("ShortcutButton", "HorseMode", false);
        LongModeConfig = Config.Bind("ShortcutButton", "LongMode", false);
        ChangeDownTimerToZeroConfig = Config.Bind("ShortcutButton", "ChangeDownTimerToZero", false);
        AbolishDownTimerConfig = Config.Bind("ShortcutButton", "AbolishDownTimer", false);
        AutoStartGameConfig = Config.Bind("ShortcutButton", "AutoStartGame", false);
        ChangeDownTimerTo114514Config = Config.Bind("ShortcutButton", "ChangeDownTimerTo114514", false);
        
        FPSPlusConfig = Config.Bind("Other", "FPSPlus", false);
#if DEBUG
        NotEndGameConfig = Config.Bind("Other", "NotEndGame", false);
#endif

        menuUI = AddComponent<MenuUI>();
        CustomTips = AddComponent<CustomTips>();
        floatingButton = AddComponent<FloatingButton>();

        //Translator.Init();

        BetaBuildURL = Config.Bind("Other", "BetaBuildURL", "");

        if (Application.version == CanUseInAmongUsVer)
        {
            Logger.LogInfo($"AmongUs Version: {Application.version}, is ok"); // AmongUs 版本支持
            isAmongUsVersionOK = true;
        }
        else
        {
            Logger.LogInfo($"游戏本体版本过低或过高,AmongUs Version: {Application.version}"); // AmongUs 版本不正确
            isAmongUsVersionOK = false;
        }
        #if Windows
        RegistryManager.Init(); // 这是优先级最高的模块初始化方法，不能使用模块初始化属性
#endif
        //各组件初始化
        Harmony.PatchAll();
        if (ModMode != 0) ConsoleManager.DetachConsole();
        else ConsoleManager.CreateConsole();

        DevManager.Init();

        //模组加载好了标语
        Msg("========= YuET loaded! =========", "YuET Plugin Load");
    }
}

public enum RoleTeam
{
    Crewmate,
    Impostor,
    Error
}

public enum RoomMode
{
    Normal,
    Plus25,
    Error
}

public enum TipsCode
{
    Info,
    Warn,
    Error,
    Custom,
    ModLogo,
    AntiCheat
}

public enum ModMode
{
    Debug = 0,
    Canary = 1,
    Release = 2,
    Error = -1
}

// 需配合 QQHelper.cs 的DeleteReason使用
public static class DisconnectReasonEx
{
    public const int CloseGame = 13;
}