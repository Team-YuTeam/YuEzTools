using HarmonyLib;
using System;
using System.Reflection;
using YuEzTools.Helpers;
using YuEzTools.Modules;
using YuEzTools.UI;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Start))]
public static class ToolsMenuBehaviourStartPatch
{
    public static void Postfix(OptionsMenuBehaviour __instance)
    {
        if (__instance.DisableMouseMovement == null) return;
        
        GenerateClientToolsButtons(__instance);
    }
    
    
    private static void GenerateClientToolsButtons(OptionsMenuBehaviour optionsMenu)
    {
        foreach (var config in ToggleHelperManager.AllButtons)
        {
            // 调用 ClientToolsItem.Create 生成按钮，绑定配置的逻辑
            ClientToolsItem.Create(
                name: GetString("MenuUI."+config.NameKey), // 按钮文本（本地化）
                config: config.GetState(),       // 初始状态（从 Toggles 读取）
                optionsMenuBehaviour: optionsMenu,
                additionalOnClickAction: () =>   // 点击逻辑（同步 Toggles + 额外操作）
                {
                    var newState = !config.GetState(); // 翻转状态
                    config.SetState(newState);         // 更新 Toggles 字段
                    config.AdditionalAction?.Invoke(); // 触发额外逻辑
                }
            );
        }
    }
}


[HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Close))]
public static class ToolsMenuBehaviourClosePatch
{
    public static void Postfix()
    {
        ClientToolsItem.CustomBackground?.gameObject.SetActive(false);
    }
}