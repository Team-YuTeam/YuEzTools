using System;
using YuEzTools.Modules;
using YuEzTools.Utils;

namespace YuEzTools.Helpers;

// 统一按钮配置类：存储每个按钮的所有信息，只需配置一次
public class ToggleHelper
{
    // 1. 本地化键（用于 GetString 获取显示文本）
    public string NameKey { get; set; }
    // 2. 所属分组（对应 MenuUI 的 Group，如“反作弊”“界面”）
    public string GroupKey { get; set; }
    // 3. 所属子菜单（对应 MenuUI 的 Submenu，可选，无则为 null）
    public string SubmenuKey { get; set; }
    // 4. 绑定 Toggles 字段：获取当前状态
    public Func<bool> GetState { get; set; }
    // 5. 绑定 Toggles 字段：设置新状态
    public Action<bool> SetState { get; set; }
    // 6. 额外点击逻辑（可选，如退出游戏、反作弊触发）
    public Action AdditionalAction { get; set; }

    // 构造函数：配置按钮时调用
    public ToggleHelper(
        string nameKey, 
        string groupKey, 
        string submenuKey, 
        Func<bool> getState, 
        Action<bool> setState, 
        Action additionalAction = null)
    {
        NameKey = nameKey;
        GroupKey = groupKey;
        SubmenuKey = submenuKey;
        GetState = getState;
        SetState = setState;
        AdditionalAction = additionalAction;
    }
}

// 按钮配置管理器：集中管理所有按钮配置（用户只在这里写一次！）
public static class ToggleHelperManager
{
    // 全局统一的按钮配置列表：所有按钮都在这里添加
    public static List<ToggleHelper> AllButtons = new List<ToggleHelper>
    {
        // -------------------------- 反作弊组 --------------------------
        new ToggleHelper(
            nameKey: "EnableAntiCheat",    // 按钮文本键（对应本地化）
            groupKey: "AntiCheat",          // 所属组（反作弊）
            submenuKey: null,                      // 无icesubmenu，直接在组下
            getState: () => Toggles.EnableAntiCheat, // 绑定 Toggles 字段
            setState: val => Toggles.EnableAntiCheat = val
        ),
        new ToggleHelper(
            nameKey: "AutoExit",    // 按钮文本键（对应本地化）
            groupKey: "AntiCheat",          // 所属组（反作弊）
            submenuKey: null,                      // 无icesubmenu，直接在组下
            getState: () => Toggles.AutoExit, // 绑定 Toggles 字段
            setState: val => Toggles.AutoExit = val
        ),
        new ToggleHelper(
            nameKey: "KickNotLogin",    // 按钮文本键（对应本地化）
            groupKey: "AntiCheat",          // 所属组（反作弊）
            submenuKey: null,                      // 无icesubmenu，直接在组下
            getState: () => Toggles.KickNotLogin, // 绑定 Toggles 字段
            setState: val => Toggles.KickNotLogin = val
        ),
        new ToggleHelper(
            nameKey: "AutoReportHacker",    // 按钮文本键（对应本地化）
            groupKey: "AntiCheat",          // 所属组（反作弊）
            submenuKey: null,                      // 无icesubmenu，直接在组下
            getState: () => Toggles.AutoReportHacker, // 绑定 Toggles 字段
            setState: val => Toggles.AutoReportHacker = val
        ),
        new ToggleHelper(
            nameKey: "ServerAllHostOrNoHost",    // 按钮文本键（对应本地化）
            groupKey: "AntiCheat",          // 所属组（反作弊）
            submenuKey: "ModMode",                      
            getState: () => Toggles.ServerAllHostOrNoHost, // 绑定 Toggles 字段
            setState: val => Toggles.ServerAllHostOrNoHost = val
        ),
        new ToggleHelper(
            nameKey: "SafeMode",    // 按钮文本键（对应本地化）
            groupKey: "AntiCheat",          // 所属组（反作弊）
            submenuKey: "ModMode",                      
            getState: () => Toggles.SafeMode, // 绑定 Toggles 字段
            setState: val => Toggles.SafeMode = val
        ),
        // -------------------------- 界面 --------------------------
        new ToggleHelper(
            nameKey: "DarkUI",    // 按钮文本键（对应本地化）
            groupKey: "Interface",          // 所属组
            submenuKey: null,                      
            getState: () => Toggles.DarkMode, // 绑定 Toggles 字段
            setState: val => Toggles.DarkMode = val
        ),
        new ToggleHelper(
            nameKey: "ShowInfoInLobby",    // 按钮文本键（对应本地化）
            groupKey: "Interface",          // 所属组
            submenuKey: null,                      
            getState: () => Toggles.ShowInfoInLobby, // 绑定 Toggles 字段
            setState: val => Toggles.ShowInfoInLobby = val
        ),
        new ToggleHelper(
            nameKey: "ShowCommit",    // 按钮文本键（对应本地化）
            groupKey: "Interface",          // 所属组
            submenuKey: "PingPart",                      
            getState: () => Toggles.ShowCommit, // 绑定 Toggles 字段
            setState: val => Toggles.ShowCommit = val
        ),
        new ToggleHelper(
            nameKey: "ShowModText",    // 按钮文本键（对应本地化）
            groupKey: "Interface",          // 所属组
            submenuKey: "PingPart",                      
            getState: () => Toggles.ShowModText, // 绑定 Toggles 字段
            setState: val => Toggles.ShowModText = val
        ),
        new ToggleHelper(
            nameKey: "ShowIsSafe",    // 按钮文本键（对应本地化）
            groupKey: "Interface",          // 所属组
            submenuKey: "PingPart",                      
            getState: () => Toggles.ShowIsSafe, // 绑定 Toggles 字段
            setState: val => Toggles.ShowIsSafe = val
        ),
        new ToggleHelper(
            nameKey: "ShowPing",    // 按钮文本键（对应本地化）
            groupKey: "Interface",          // 所属组
            submenuKey: "PingPart",                      
            getState: () => Toggles.ShowPing, // 绑定 Toggles 字段
            setState: val => Toggles.ShowPing = val
        ),
        new ToggleHelper(
            nameKey: "ShowFPS",    // 按钮文本键（对应本地化）
            groupKey: "Interface",          // 所属组
            submenuKey: "PingPart",                      
            getState: () => Toggles.ShowFPS, // 绑定 Toggles 字段
            setState: val => Toggles.ShowFPS = val
        ),
        new ToggleHelper(
            nameKey: "ShowServer",    // 按钮文本键（对应本地化）
            groupKey: "Interface",          // 所属组
            submenuKey: "PingPart",                      
            getState: () => Toggles.ShowServer, // 绑定 Toggles 字段
            setState: val => Toggles.ShowServer = val
        ),
        new ToggleHelper(
            nameKey: "ShowRoomTime",    // 按钮文本键（对应本地化）
            groupKey: "Interface",          // 所属组
            submenuKey: "PingPart",                      
            getState: () => Toggles.ShowRoomTime, // 绑定 Toggles 字段
            setState: val => Toggles.ShowRoomTime = val
        ),
        new ToggleHelper(
            nameKey: "ShowIsAutoExit",    // 按钮文本键（对应本地化）
            groupKey: "Interface",          // 所属组
            submenuKey: "PingPart",                      
            getState: () => Toggles.ShowIsAutoExit, // 绑定 Toggles 字段
            setState: val => Toggles.ShowIsAutoExit = val
        ),
        new ToggleHelper(
            nameKey: "ShowGM",    // 按钮文本键（对应本地化）
            groupKey: "Interface",          // 所属组
            submenuKey: "PingPart",                      
            getState: () => Toggles.ShowGM, // 绑定 Toggles 字段
            setState: val => Toggles.ShowGM = val
        ),
        new ToggleHelper(
            nameKey: "WinTextSize",    // 按钮文本键（对应本地化）
            groupKey: "Interface",          // 所属组
            submenuKey: "EndPart",                      
            getState: () => Toggles.WinTextSize, // 绑定 Toggles 字段
            setState: val => Toggles.WinTextSize = val
        ),
        /*
         new ToggleHelper(
            nameKey: "ShowLocalNowTime",    // 按钮文本键（对应本地化）
            groupKey: "Interface",          // 所属组
            submenuKey: "PingPart",                      
            getState: () => Toggles.ShowLocalNowTime, // 绑定 Toggles 字段
            setState: val => Toggles.ShowLocalNowTime = val
        ),
                 new ToggleHelper(
            nameKey: "ShowUTC",    // 按钮文本键（对应本地化）
            groupKey: "Interface",          // 所属组
            submenuKey: "PingPart",                      
            getState: () => Toggles.ShowUTC, // 绑定 Toggles 字段
            setState: val => Toggles.ShowUTC = val
        ),
        */
        // -------------------------- 快捷按钮 --------------------------
        new ToggleHelper(
            nameKey: "DumpLog",    // 按钮文本键（对应本地化）
            groupKey: "ShortcutButton",          // 所属组
            submenuKey: null,                      
            getState: () => Toggles.DumpLog, // 绑定 Toggles 字段
            setState: val => Toggles.DumpLog = val
        ),
        new ToggleHelper(
            nameKey: "OpenGameDic",    // 按钮文本键（对应本地化）
            groupKey: "ShortcutButton",          // 所属组
            submenuKey: null,                      
            getState: () => Toggles.OpenGameDic, // 绑定 Toggles 字段
            setState: val => Toggles.OpenGameDic = val
        ),
        new ToggleHelper(
            nameKey: "CloseMusicOfOr",    // 按钮文本键（对应本地化）
            groupKey: "ShortcutButton",          // 所属组
            submenuKey: null,                      
            getState: () => Toggles.CloseMusicOfOr, // 绑定 Toggles 字段
            setState: val => Toggles.CloseMusicOfOr = val
        ),
        new ToggleHelper(
            nameKey: "reShowRoleT",    // 按钮文本键（对应本地化）
            groupKey: "ShortcutButton",          // 所属组
            submenuKey: null,                      
            getState: () => Toggles.reShowRoleT, // 绑定 Toggles 字段
            setState: val => Toggles.reShowRoleT = val
        ),
        new ToggleHelper(
            nameKey: "ExitGame",    // 按钮文本键（对应本地化）
            groupKey: "ShortcutButton",          // 所属组
            submenuKey: "Left",                      
            getState: () => Toggles.ExitGame, // 绑定 Toggles 字段
            setState: val => Toggles.ExitGame = val
        ),
        new ToggleHelper(
            nameKey: "RealBan",    // 按钮文本键（对应本地化）
            groupKey: "ShortcutButton",          // 所属组
            submenuKey: "Left",                      
            getState: () => Toggles.RealBan, // 绑定 Toggles 字段
            setState: val => Toggles.RealBan = val
        ),
        new ToggleHelper(
            nameKey: "HorseMode",    // 按钮文本键（对应本地化）
            groupKey: "ShortcutButton",          // 所属组
            submenuKey: "PeopleMode",                      
            getState: () => Toggles.HorseMode, // 绑定 Toggles 字段
            setState: val => Toggles.HorseMode = val
        ),
        new ToggleHelper(
            nameKey: "LongMode",    // 按钮文本键（对应本地化）
            groupKey: "ShortcutButton",          // 所属组
            submenuKey: "PeopleMode",                      
            getState: () => Toggles.LongMode, // 绑定 Toggles 字段
            setState: val => Toggles.LongMode = val
        ),
        new ToggleHelper(
            nameKey: "ChangeDownTimerToZero",    // 按钮文本键（对应本地化）
            groupKey: "ShortcutButton",          // 所属组
            submenuKey: "OnlyHost",                      
            getState: () => Toggles.ChangeDownTimerToZero, // 绑定 Toggles 字段
            setState: val => Toggles.ChangeDownTimerToZero = val
        ),
        new ToggleHelper(
            nameKey: "ChangeDownTimerTo114514",    // 按钮文本键（对应本地化）
            groupKey: "ShortcutButton",          // 所属组
            submenuKey: "OnlyHost",                      
            getState: () => Toggles.ChangeDownTimerTo114514, // 绑定 Toggles 字段
            setState: val => Toggles.ChangeDownTimerTo114514 = val
        ),
        new ToggleHelper(
            nameKey: "AutoStartGame",    // 按钮文本键（对应本地化）
            groupKey: "ShortcutButton",          // 所属组
            submenuKey: "OnlyHost",                      
            getState: () => Toggles.AutoStartGame, // 绑定 Toggles 字段
            setState: val => Toggles.AutoStartGame = val
        ),
        new ToggleHelper(
            nameKey: "AbolishDownTimer",    // 按钮文本键（对应本地化）
            groupKey: "ShortcutButton",          // 所属组
            submenuKey: "OnlyHost",                      
            getState: () => Toggles.AbolishDownTimer, // 绑定 Toggles 字段
            setState: val => Toggles.AbolishDownTimer = val
        ),
        // -------------------------- 其他 --------------------------
        new ToggleHelper(
            nameKey: "FPSPlus",    // 按钮文本键（对应本地化）
            groupKey: "Other",          // 所属组
            submenuKey: null,                      
            getState: () => Toggles.FPSPlus, // 绑定 Toggles 字段
            setState: val => Toggles.FPSPlus = val
        ),
    };
}