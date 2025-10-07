using HarmonyLib;
using System;
using System.Reflection;
using YuEzTools.Modules;
using YuEzTools.UI;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Start))]
public static class ToolsMenuBehaviourStartPatch
{
    // 存储所有生成的按钮实例
    private static ClientToolsItem[] toggleButtons;

    public static void Postfix(OptionsMenuBehaviour __instance)
    {
        if (__instance.DisableMouseMovement == null) return;

        // 获取Toggles类中所有公开的静态bool字段
        var toggleFields = typeof(Toggles).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(bool))
            .OrderBy(f => f.Name) // 按名称排序，保证按钮顺序固定
            .ToArray();

        toggleButtons = new ClientToolsItem[toggleFields.Length];

        // 遍历所有字段，批量生成按钮
        for (int i = 0; i < toggleFields.Length; i++)
        {
            var field = toggleFields[i];
            var fieldName = field.Name;

            // 生成按钮
            toggleButtons[i] = ClientToolsItem.Create(
                GetString(fieldName),  // 按钮显示文本
                (bool)field.GetValue(null), 
                __instance, 
                CreateToggleAction(field)  // 通用的切换动作
            );
        }
    }

    // 创建通用的切换动作：仅修改对应bool字段的值
    private static Action CreateToggleAction(FieldInfo field)
    {
        return () =>
        {
            // 获取当前值并取反
            bool currentValue = (bool)field.GetValue(null);
            bool newValue = !currentValue;
            
            // 设置新值到Toggles类的静态字段
            field.SetValue(null, newValue);
        };
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