using HarmonyLib;
using YuEzTools.Helpers;
using YuEzTools.Modules;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Start))]
public static class ToolsMenuBehaviourStartPatch
{
    public static void Postfix(OptionsMenuBehaviour __instance)
    {
        if (__instance.DisableMouseMovement == null) return;
        
        foreach (var toggleHelper in ToggleHelperManager.AllButtons)
        {
            ClientToolsItem.Create(
                name: GetString("MenuUI." + toggleHelper.NameKey),
                toggleHelper: toggleHelper,
                optionsMenuBehaviour: __instance
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
