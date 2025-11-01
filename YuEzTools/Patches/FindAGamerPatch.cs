using Il2CppSystem;
using InnerNet;
using UnityEngine;

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

[HarmonyPatch(typeof(MatchMakerGameButton), nameof(MatchMakerGameButton.SetGame))]
public static class MatchMakerGameButtonSetGamePatch
{
    public static void Prefix(MatchMakerGameButton __instance, [HarmonyArgument(0)] GameListing game)
    {
        var nameList = TranslationController.Instance.currentLanguage.languageID is SupportedLangs.SChinese or SupportedLangs.TChinese ? Main.TName_Snacks_CN : Main.TName_Snacks_EN;

        if (game.Language.ToString().Length > 9) goto End;
        var color = game.Platform.GetPlatformColor();
        var platforms = game.Platform.GetPlatformText();
        Info(game.IPString + ":" + game.Port, "FAG");
        string str = Math.Abs(game.GameId).ToString();

        int id = Math.Min(Math.Max(int.Parse(str.Substring(str.Length - 2, 2)), 1) * nameList.Count / 100, nameList.Count);
        game.HostName = $"" +
            $"<size=80%>" +
            $"<color={color}>" +
            $"{nameList[id]}" +
            $"</size>" +
            $"<size=60%>" +
            $"({platforms})" +
            $"</color>" +
            $"</size>";
        game.HostName += $"<size=40%> ({GetString("ToCloseThisRoom")}{Math.Max(0, 100 - game.Age / 100)}%)</size>";
    End:
        Info("--------This room end.--------", "FindAGamerPatch");
    }
}