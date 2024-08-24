using AmongUs.Data;
using AmongUs.Data.Player;
using Assets.InnerNet;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;

namespace YuEzTools;

// 参考：https://github.com/Yumenopai/TownOfHost_Y
public class ModNews
{
    public int Number;
    public uint Lang;
    public int BeforeNumber;
    public string Title;
    public string SubTitle;
    public string ShortTitle;
    public string Text;
    public string Date;

    public Announcement ToAnnouncement()
    {
        var result = new Announcement
        {
            Number = Number,
            Language = Lang,
            Title = Title,
            SubTitle = SubTitle,
            ShortTitle = ShortTitle,
            Text = Text,
            Date = Date,
            Id = "ModNews"
        };
        return result;
    }
}

[HarmonyPatch]
public class ModNewsHistory
{
    public static List<ModNews> AllModNews = new();
    public static ModNews GetContentFromRes(string path)
    {
        ModNews mn = new();
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
        stream.Position = 0;
        using StreamReader reader = new(stream, Encoding.UTF8);
        string text = "";
        uint langId = (uint)DataManager.Settings.Language.CurrentLanguage;
        //uint langId = (uint)SupportedLangs.SChinese;
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            if (line.StartsWith("#Number:")) mn.Number = int.Parse(line.Replace("#Number:", string.Empty));
            else if (line.StartsWith("#LangId:")) langId = uint.Parse(line.Replace("#LangId:", string.Empty));
            else if (line.StartsWith("#Title:")) mn.Title = line.Replace("#Title:", string.Empty);
            else if (line.StartsWith("#SubTitle:")) mn.SubTitle = line.Replace("#SubTitle:", string.Empty);
            else if (line.StartsWith("#ShortTitle:")) mn.ShortTitle = line.Replace("#ShortTitle:", string.Empty);
            else if (line.StartsWith("#Date:")) mn.Date = line.Replace("#Date:", string.Empty);
            else if (line.StartsWith("#---")) continue;
            else
            {
                if (line.StartsWith("## ")) line = line.Replace("## ", "<b>") + "</b>";
                else if (line.StartsWith("- ")) line = line.Replace("- ", "・");
                text += $"\n{line}";
            }
        }
        mn.Lang = langId;
        mn.Text = text;
        Logger.Info($"Number:{mn.Number}", "ModNews");
        Logger.Info($"Title:{mn.Title}", "ModNews");
        Logger.Info($"SubTitle:{mn.SubTitle}", "ModNews");
        Logger.Info($"ShortTitle:{mn.ShortTitle}", "ModNews");
        Logger.Info($"Date:{mn.Date}", "ModNews");
        return mn;
    }

    [HarmonyPatch(typeof(PlayerAnnouncementData), nameof(PlayerAnnouncementData.SetAnnouncements)), HarmonyPrefix]
    public static bool SetModAnnouncements(PlayerAnnouncementData __instance, [HarmonyArgument(0)] ref Il2CppReferenceArray<Announcement> aRange)
    {
        
        if (AllModNews.Count < 1)
        {
            var lang = DataManager.Settings.Language.CurrentLanguage.ToString();
            if (!Assembly.GetExecutingAssembly().GetManifestResourceNames().Any(x => x.StartsWith($"YuEzTools.Resources.ModNews.{lang}.")))
                lang = SupportedLangs.English.ToString();

            var fileNames = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(x => x.StartsWith($"YuEzTools.Resources.ModNews.{lang}."));
            foreach (var file in fileNames)
                AllModNews.Add(GetContentFromRes(file));

            AllModNews.Sort((a1, a2) => { return DateTime.Compare(DateTime.Parse(a2.Date), DateTime.Parse(a1.Date)); });
        }

        List<Announcement> FinalAllNews = new();
        AllModNews.Do(n => FinalAllNews.Add(n.ToAnnouncement()));
        foreach (var news in aRange)
        {
            if (!AllModNews.Any(x => x.Number == news.Number))
                FinalAllNews.Add(news);
        }
        FinalAllNews.Sort((a1, a2) => { return DateTime.Compare(DateTime.Parse(a2.Date), DateTime.Parse(a1.Date)); });

        aRange = new(FinalAllNews.Count);
        for (int i = 0; i < FinalAllNews.Count; i++)
            aRange[i] = FinalAllNews[i];

        return true;
    }
    static SpriteLoader ModLabel = SpriteLoader.FromResource("YuEzTools.Resources.Yu-Logo-tm.png", 1000f);
    // static SpriteLoader logoGlowSprite = SpriteLoader.FromResource("YuEzTools.Resources.YuET-BG-A.png", 3000f);
    static SpriteLoader logoGlowSprite = SpriteLoader.FromResource("YuEzTools.Resources.YuET-Logo-tm.png", 3000f);


    public static bool first = true;
    [HarmonyPatch(typeof(AnnouncementPanel), nameof(AnnouncementPanel.SetUp)), HarmonyPostfix]
    public static void SetUpPanel(AnnouncementPanel __instance, [HarmonyArgument(0)] Announcement announcement)
    {
        // __instance.Background.gameObject.transform.FindChild("WhiteColor").GetComponent<SpriteRenderer>().color = new Color(61, 255, 215, 185);
        if (announcement.Number < 100000) return;
        var obj = new GameObject("ModLabel");
        obj.layer = LayerExpansion.GetUILayer();
        obj.transform.SetParent(__instance.transform);
        obj.transform.localPosition = new Vector3(-0.81f, 0.16f, 0.5f);
        obj.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        var renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = ModLabel.GetSprite();
        renderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        
        var Announcement = GameObject.Find("Announcement");
        var Sizer = Announcement.transform.FindChild("Sizer");
        var Background_old = Sizer.FindChild("Background");
        var WhiteColor_Old = Background_old.FindChild("WhiteColor");
        // var AnnouncementList = Sizer.FindChild("AnnouncementList");
        // var List_Scroller = AnnouncementList.FindChild("Scroller");
        // var Scroller_Inner = List_Scroller.FindChild("Inner");
        // var AnnouncementPanel = Scroller_Inner.FindChild("AnnouncementPanel");
        // var AnnouncementPanel_Title = AnnouncementPanel.FindChild("Title");
        // var title = AnnouncementPanel_Title.GetComponent<TextMeshPro>();
        // title.color = Color.white;
        if (!first)
        {
            return;
        }
        
        var newBackground = GameObject.Instantiate(WhiteColor_Old, Sizer);
        // Background_old.SetActive(false);
        // newBackground.GetComponent<SpriteRenderer>().sprite = logoGlowSprite.GetSprite();
        // WhiteColor_Old.SetActive(false);
        // newBackground.transform.localPosition = new Vector3(0, 0, 0);
        // newBackground.transform.localScale = new Vector3(16.6294f, 16.7267f, 3.0309f);
        // newBackground.GetComponent<SpriteRenderer>().color = Color.white.AlphaMultiplied(0.35f);
        newBackground.GetComponent<SpriteRenderer>().sprite = logoGlowSprite.GetSprite();
        newBackground.transform.localPosition = new Vector3(0.8315f, -0.2503f, -1);
        newBackground.transform.localScale = new Vector3(7, 7, 1f);
        first = false;
    }
    // [HarmonyPatch(typeof(AnnouncementPanel), nameof(AnnouncementPanel.)), HarmonyPostfix]

    // [HarmonyPatch(typeof(AnnouncementPanel), nameof(AnnouncementPanel.close)), HarmonyPostfix]

}
