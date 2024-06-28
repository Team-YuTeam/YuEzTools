using HarmonyLib;
using YuAntiCheat.Updater;

namespace YuAntiCheat;

[HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
public static class VersionShower_Start
{
    public static void Postfix(VersionShower __instance)
    {
        #if CANARY
        __instance.text.text = TranslationController.Instance.currentLanguage.languageID == SupportedLangs.SChinese || TranslationController.Instance.currentLanguage.languageID == SupportedLangs.TChinese ? $"<color={Main.ModColor}>{Main.ModName}</color> (<color=#DC143C>您正在使用 v{Main.PluginVersion} Canary测试版！</color>)" : $"<color={Main.ModColor}>{Main.ModName}</color> (<color=#DC143C>You are using  v{Main.PluginVersion} Canary Version</color>)";
        #endif
#if DEBUG
        __instance.text.text = TranslationController.Instance.currentLanguage.languageID == SupportedLangs.SChinese || TranslationController.Instance.currentLanguage.languageID == SupportedLangs.TChinese ? $"<color={Main.ModColor}>{Main.ModName}</color> (<color=#DC143C>您正在使用 v{Main.PluginVersion} Debug开发者版！</color>)" : $"<color={Main.ModColor}>{Main.ModName}</color> (<color=#DC143C>You are using  v{Main.PluginVersion} Debug Version</color>)";
#endif
        #if RELEASE
        if(ModUpdater.hasUpdate)
            __instance.text.text = TranslationController.Instance.currentLanguage.languageID == SupportedLangs.SChinese || TranslationController.Instance.currentLanguage.languageID == SupportedLangs.TChinese ? $"<color={Main.ModColor}>{Main.ModName}</color> (<color=#DC143C>YuAC有v{ModUpdater.latestVersion}更新!</color>)" : $"<color={Main.ModColor}>{Main.ModName}</color> (<color=#DC143C>YuAC has v{ModUpdater.latestVersion} version update!</color>)";
        else
            __instance.text.text = TranslationController.Instance.currentLanguage.languageID == SupportedLangs.SChinese || TranslationController.Instance.currentLanguage.languageID == SupportedLangs.TChinese ? $"<color={Main.ModColor}>{Main.ModName}</color> (<color=#00FF00>YuAC无更新</color>)" : $"<color={Main.ModColor}>{Main.ModName}</color> (<color=#00FF00>Your YuAC version is up to date/color>)";
#endif
    }
}