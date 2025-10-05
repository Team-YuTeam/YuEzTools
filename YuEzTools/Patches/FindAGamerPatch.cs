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
        __instance.RefreshList();
    }
}//*/

[HarmonyPatch(typeof(MatchMakerGameButton), nameof(MatchMakerGameButton.SetGame))]
public static class MatchMakerGameButtonSetGamePatch
{
    public static void Prefix(MatchMakerGameButton __instance, [HarmonyArgument(0)] GameListing game)
    {
        var nameList = TranslationController.Instance.currentLanguage.languageID is SupportedLangs.SChinese or SupportedLangs.TChinese ? Main.TName_Snacks_CN : Main.TName_Snacks_EN;

        if (game.Language.ToString().Length > 9) goto End;
        var color = game.Platform switch
        {
            Platforms.StandaloneItch => "#FF4300",
            Platforms.StandaloneWin10 => "#FF7E32",
            Platforms.StandaloneEpicPC => "#FFD432",
            Platforms.StandaloneSteamPC => "#B8FF32",

            Platforms.Xbox => "#60FF32",
            Platforms.Switch => "#32FF69",
            Platforms.Playstation => "#32FFC6",

            Platforms.StandaloneMac => "#32E9FF",
            Platforms.IPhone => "#32AEFF",
            Platforms.Android => "#325AFF",

            Platforms.Unknown or
            _ => "#ffffff"
        };
        var platforms = game.Platform switch
        {
            Platforms.StandaloneItch => "Itch",
            Platforms.StandaloneWin10 => GetString("Microsoft"),
            Platforms.StandaloneEpicPC => "Epic",
            Platforms.StandaloneSteamPC => "Steam",

            Platforms.Xbox => "Xbox",
            Platforms.Switch => "Switch",
            Platforms.Playstation => "PS",

            Platforms.StandaloneMac => "Mac",
            Platforms.IPhone => GetString("iPhone"),
            Platforms.Android => GetString("Android"),

            Platforms.Unknown or
            _ => GetString("Platforms.Unknown")
        };
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