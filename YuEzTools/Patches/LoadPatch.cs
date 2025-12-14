using System;
using System.Collections;
using TMPro;
using UnityEngine;
using YuEzTools.Helpers;
using System.IO;
using BepInEx.Unity.IL2CPP.Utils;
using Object = UnityEngine.Object;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(SplashManager))]
public class LoadPatch
{
    private static GameObject YuETLoading = new GameObject();
    private static TextMeshPro loadText = null!;
    private static TextMeshPro processText = null!;
    
    [HarmonyPatch(nameof(SplashManager.Start)), HarmonyPrefix]
    public static bool Start(SplashManager __instance)
    {
        __instance.StartCoroutine(InitializeRefData(__instance));
        return false;
    }
    
    private static IEnumerator InitializeRefData(SplashManager __instance)
    {
        ChangeLogoAndAwakeLoadingLogo(__instance);
        CreateTextComponents(__instance);
        yield break;
    }

    private static void ChangeLogoAndAwakeLoadingLogo(SplashManager __instance)
    {
        var logoAnimator = GameObject.Find("LogoAnimator");
        var logoroot = logoAnimator.transform.Find("LogoRoot");
        var ISLogo = logoroot.transform.Find("ISLogo");
        var islogo_sr = ISLogo.GetComponent<SpriteRenderer>();
        var blackoverlay = logoAnimator.transform.Find("BlackOverlay");
        islogo_sr.sprite = LoadSprite("YuEzTools.Resources.YuET-Logo-tm.png", 450f);
        Object.Destroy(blackoverlay.gameObject);
        __instance.loadingObject.SetActive(true);
    }
    
        
    // some code of this class is copied from FS
    private static void CreateTextComponents(SplashManager instance)
    {
        YuETLoading.name = "YuETLoading";
        
        var temp = instance.errorPopup.InfoText;
        loadText = ObjectHelper.InstantiateTextComponent(temp, new Vector3(0f, -1.08f, -10f),YuETLoading.transform);
        processText = ObjectHelper.InstantiateTextComponent(temp, new Vector3(2.56f, -2.57f, 1f),YuETLoading.transform);
        
        loadText.name = "LoadText";
        loadText.text = "LoadText is already.";
        loadText.alignment = TextAlignmentOptions.Top;
        
        processText.name = "ProcessText";
        processText.text = "ProcessText is already.";
        processText.alignment = TextAlignmentOptions.TopRight;
        var ptc = processText.color;
        ptc.a = 0.5f;
        processText.color = ptc;
    }
}