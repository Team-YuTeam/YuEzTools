using Il2CppSystem;
using InnerNet;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(FindAGameManager), nameof(FindAGameManager.Update))]
public static class FindAGameManagerUpdatePatch
{
    private static int buffer = 80;
    private static GameObject RefreshButton;
    private static GameObject InputDisplayGlyph;
    public static void Postfix(FindAGameManager __instance)
    {
        if ((RefreshButton = GameObject.Find("RefreshButton")) != null)
            RefreshButton.transform.localPosition = new Vector3(100f, 100f, 100f);
        if ((InputDisplayGlyph = GameObject.Find("InputDisplayGlyph")) != null)
            InputDisplayGlyph.transform.localPosition = new Vector3(100f, 100f, 100f);

        buffer--; if (buffer > 0) return; buffer = 80;
        // __instance.RefreshList();
    }
}//*/


[HarmonyPatch(typeof(GameContainer), nameof(GameContainer.SetupGameInfo))]
public static class SetupGameInfoPatch
{
    public static GameObject HostName;
    [HarmonyPostfix]
    public static void Postfix(GameContainer __instance)
    {
        var game = __instance.gameListing;
        // var nameList = TranslationController.Instance.currentLanguage.languageID is SupportedLangs.SChinese or SupportedLangs.TChinese ? Main.TName_Snacks_CN : Main.TName_Snacks_EN;
     
        // Some from FS
        var mapLogo = __instance.mapLogo.transform;
        var old = mapLogo.parent.FindChild("HostName")?.gameObject;
        if (old)
            Object.Destroy(old);
        var Container = mapLogo.parent.FindChild("Container").gameObject;
        var Container_TMP = Container.transform.FindChild("Text_TMP").GetComponent<TextMeshPro>();
        Container_TMP.text += $"<color=#FF0000>({game.NumImpostors})</color>";
        HostName = new GameObject("HostName")
        {
            transform =
            {
                parent = __instance.mapLogo.transform.parent,
                localPosition = new Vector3(-0.6f, mapLogo.localPosition.y - 0.175f, mapLogo.localPosition.z),
                localScale = new Vector3(0.14f, 0.14f, 1f),
            },
        };

        if (game.Language.ToString().Length > 9) goto End;
        var color = game.Platform.GetPlatformColor();
        var platforms = game.Platform.GetPlatformText();
        Info(game.IPString + ":" + game.Port, "SetupGameInfoPatch");
        string str = Math.Abs(game.GameId).ToString();

        // int id = Math.Min(Math.Max(int.Parse(str.Substring(str.Length - 2, 2)), 1) * nameList.Count / 100, nameList.Count);
        var HNTMP = HostName.AddComponent<TextMeshPro>();
        HNTMP.text = $"" +
                     $"<size=45%>" +
                     $"<color={color}>" +
                     $"{game.TrueHostName}" +
                     $"</size>" +
                     $"<size=25%>" +
                     $"({platforms})" +
                     $"</color>" +
                        $"</size>";
        HNTMP.text += $"\n<size=18%><color={Main.ModColor}>{GameCode.IntToGameName(game.GameId)}</color>";
        // HNTMP.alignment = TextAlignmentOptions.MidlineLeft;
        End:
        Info("--------This room end.--------", "SetupGameInfoPatch");
    }
}