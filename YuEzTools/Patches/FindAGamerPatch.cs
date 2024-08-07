using HarmonyLib;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using InnerNet;
using Il2CppSystem.Linq;
using UnityEngine;

namespace YuEzTools;

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
    public static void Prefix(MatchMakerGameButton __instance, [HarmonyArgument(0)]  GameListing game)
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
            Platforms.StandaloneWin10 => "Windows",
            Platforms.StandaloneEpicPC => "Epic",
            Platforms.StandaloneSteamPC => "Steam",
            
            Platforms.Xbox => "Xbox",
            Platforms.Switch => "Switch",
            Platforms.Playstation => "PS",

            Platforms.StandaloneMac => "Mac",
            Platforms.IPhone => Translator.GetString("iPhone"),
            Platforms.Android => Translator.GetString("Android"),

            Platforms.Unknown or
            _ => Translator.GetString("Platforms.Unknown")
        };
        string str = Math.Abs(game.GameId).ToString();
            int id = Math.Min(Math.Max(int.Parse(str.Substring(str.Length - 2, 2)), 1) * nameList.Count / 100, nameList.Count);
        if (game.Platform == Platforms.Switch)
        {
            int halfLength = nameList[id].Length / 2;

            string firstHalf = nameList[id].Substring(0, halfLength);
            string secondHalf = nameList[id].Substring(halfLength);
            game.HostName = $"" +
                $"<size=80%>" +

                $"<color=#00B2FF>" +
                $"{firstHalf}" +
                $"</color>" +

                $"<color=#ff0000>" +
                $"{secondHalf}" +
                $"</color>" +
                $"</size>" +

                $"<size=40%>" +

                "<color=#ffff00>----</color>" +
                $"<color=#00B2FF>" +
                $"Nintendo" +
                $"</color>" +

                $"<color=#ff0000>" +
                $"Switch" +
                $"</color>" +

                $"</size>"
                ;
        }
        else
            game.HostName = $"" +
                $"<size=80%>" +
                $"<color={color}>" +
                $"{nameList[id]}" +
                $"</color>" +
                $"</size>"+
                $"<size=40%>" +
                "<color=#ffff00>----</color>" +
                $"<color={color}>" +
                $"{platforms}" +
                $"</color>" +
                $"</size>"
                ;
            game.HostName += $"<size=30%> ({Math.Max(0, 100 - game.Age / 100)}%)</size>";
        End:
        Logger.Info("1", "test");
    }
}
