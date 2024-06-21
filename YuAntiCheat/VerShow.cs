using HarmonyLib;
using YuAntiCheat.Updater;

namespace YuAntiCheat;

[HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
public static class VersionShower_Start
{
    public static void Postfix(VersionShower __instance)
    {
        if(ModUpdater.hasUpdate)
            __instance.text.text = TranslationController.Instance.currentLanguage.languageID == SupportedLangs.SChinese || TranslationController.Instance.currentLanguage.languageID == SupportedLangs.TChinese ? $"<color={Main.ModColor}>{Main.ModName}</color> (<color=#DC143C>YuAC有v{ModUpdater.latestVersion}更新!</color>)" : $"<color={Main.ModColor}>{Main.ModName}</color> (<color=#DC143C>YuAC has v{ModUpdater.latestVersion} version update!</color>)";
        else
            __instance.text.text = TranslationController.Instance.currentLanguage.languageID == SupportedLangs.SChinese || TranslationController.Instance.currentLanguage.languageID == SupportedLangs.TChinese ? $"<color={Main.ModColor}>{Main.ModName}</color> (<color=#00FF00>YuAC无更新</color>)" : $"<color={Main.ModColor}>{Main.ModName}</color> (<color=#00FF00>Your YuAC version is up to date/color>)";
    }
}