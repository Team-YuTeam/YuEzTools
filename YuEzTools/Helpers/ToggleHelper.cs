using System;
using YuEzTools.UI;
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
            nameKey: "ServerAllHostOrNoHost",    // 按钮文本键（对应本地化）
            groupKey: "AntiCheat",          // 所属组（反作弊）
            submenuKey: "ModMode",                      
            getState: () => Toggles.ServerAllHostOrNoHost, // 绑定 Toggles 字段
            setState: val => Toggles.ServerAllHostOrNoHost = val
        ),

    };
}