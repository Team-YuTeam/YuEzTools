using YuEzTools.Modules;
using YuEzTools.Utils;

namespace YuEzTools.Patches;

//��Դ��https://github.com/tukasa0001/TownOfHost/pull/1265
[HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Start))]
public static class ToolsMenuBehaviourStartPatch
{
    private static ClientToolsItem SwitchVanilla;

    public static void Postfix(OptionsMenuBehaviour __instance)
    {
        if (__instance.DisableMouseMovement == null) return;
        Main.SwitchVanilla.Value = false;

        if (SwitchVanilla == null || SwitchVanilla.ToggleButton == null)
        {
            SwitchVanilla = ClientToolsItem.Create(GetString("SwitchVanilla1"), Main.SwitchVanilla, __instance, SwitchVanillaButtonToggle);
            static void SwitchVanillaButtonToggle()
            {
                if (GetPlayer.IsPlayer)
                {
                    AmongUsClient.Instance.ExitGame(DisconnectReasons.ExitGame);
                }
                Harmony.UnpatchAll();
                Main.Instance.Unload();
            }
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