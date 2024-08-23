using HarmonyLib;
using TMPro;
using UnityEngine;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(SplashManager), nameof(SplashManager.Start))]
public class SplashManagerPatch
{
    static SpriteLoader logoSprite = SpriteLoader.FromResource("YuEzTools.Resources.YuET-Logo-tm.png", 200f);
    // static SpriteLoader logoGlowSprite = SpriteLoader.FromResource("YuEzTools.Resources.Yu-Logo-tm.png", 200f);
    static TextMeshPro loadText = null!;
    public static bool Prefix(SplashManager __instance)
    {
        __instance.logoAnimFinish.transform.FindChild("LogoRoot").FindChild("ISLogo").GetComponent<SpriteRenderer>().sprite = logoSprite.GetSprite();
        
        
        loadText = GameObject.Instantiate(__instance.errorPopup.InfoText,  __instance.logoAnimFinish.transform.FindChild("LogoRoot").FindChild("ISLogo"));
        loadText.transform.localPosition = new(0, __instance.logoAnimFinish.transform.FindChild("LogoRoot").FindChild("ISLogo").position.y -1.18f, 0);
        loadText.fontStyle = TMPro.FontStyles.Bold;
        loadText.text = "欢迎使用YuET!\n<size=65%>Welcome YuET!</size>";
        loadText.color = Color.white.AlphaMultiplied(0.3f);
        loadText.SetActive(__instance.logoAnimFinish.enabled);
        return true;
    }
}