using UnityEngine;
using YuEzTools.UI;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(LanguageSetter), nameof(LanguageSetter.SetLanguage))]
public class LanguageSetterPatch
{
    private static SupportedLangs pre_selected;
    private static void Prefix(LanguageSetter __instance, LanguageButton selected)
    {
        pre_selected = TranslationController.Instance.currentLanguage.languageID;
    }
    private static void Postfix(LanguageSetter __instance, LanguageButton selected)
    {
        if (selected.Language.languageID == pre_selected) return;
        CustomPopup.Show(GetString("Tips"), string.Format(GetString("LanguageChange.text"),GetString(pre_selected.ToString()),selected.Language.Name,GetString("LanguageChange.Donot",pre_selected))
                                            + "\n\n" +  string.Format(GetString("LanguageChange.Ortext",pre_selected),GetString(pre_selected.ToString()),selected.Language.Name)
            , new()
            {
                (GetString("LanguageChange.Refresh"), () => SceneChanger.ChangeScene("MainMenu") ),
                (GetString("LanguageChange.Donot",pre_selected), () => SetLanguage(__instance,pre_selected)),
                (GetString(StringNames.ExitGame), Application.Quit),
                (GetString(StringNames.Cancel), null)
            });
    }

    private static void SetLanguage(LanguageSetter __instance,SupportedLangs selected)
    {
        if (selected != null)
        {
            DestroyableSingleton<TranslationController>.Instance.SetLanguage(selected);
            for (int i = 0; i < __instance.AllButtons.Length; i++)
            {
                __instance.AllButtons[i].Title.color = Color.white;
            }
            __instance.parentLangButton.text = GetString(selected.ToString());
        }
    }
}