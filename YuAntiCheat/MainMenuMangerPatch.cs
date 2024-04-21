using AmongUs.Data;
using AmongUs.Data.Player;
using Assets.InnerNet;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static UnityEngine.UI.Button;
using Object = UnityEngine.Object;
using YuAntiCheat;
using YuAntiCheat.UI;
//using YuAntiCheat.Updater;

namespace YuAntiCheat.UI;

//[HarmonyPatch]
// public class MainMenuManagerPatch
// {
//     public static GameObject template;
//     public static GameObject updateButton;
//
//     [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate)), HarmonyPostfix]
//     public static void Postfix(MainMenuManager __instance)
//     {
//         TitleLogoPatch.PlayLocalButton?.transform?.SetLocalY(100f);
//         TitleLogoPatch.PlayOnlineButton?.transform?.SetLocalY(100f);
//         TitleLogoPatch.HowToPlayButton?.transform?.SetLocalY(100f);
//         TitleLogoPatch.FreePlayButton?.transform?.SetLocalY(100f);
//     }
//     [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start)), HarmonyPrefix]
//     public static void Start_Prefix(MainMenuManager __instance)
//     {
//         if (template == null) template = GameObject.Find("/MainUI/ExitGameButton");
//         if (template == null) return;
//         //Updateボタンを生成
//         if (updateButton == null) updateButton = Object.Instantiate(template, template.transform.parent);
//         updateButton.name = "UpdateButton";
//         updateButton.transform.position = template.transform.position + new Vector3(0.25f, 0.75f);
//         updateButton.transform.GetChild(0).GetComponent<RectTransform>().localScale *= 1.5f;
//
//         var updateText = updateButton.transform.GetChild(0).GetComponent<TMPro.TMP_Text>();
//         Color updateColor = new Color32(247, 56, 23, byte.MaxValue);
//         PassiveButton updatePassiveButton = updateButton.GetComponent<PassiveButton>();
//         SpriteRenderer updateButtonSprite = updateButton.GetComponent<SpriteRenderer>();
//         updatePassiveButton.OnClick = new();
//         updatePassiveButton.OnClick.AddListener((Action)(() =>
//         {
//             updateButton.SetActive(false);
//             ModUpdater.StartUpdate(ModUpdater.downloadUrl);
//         }));
//         updatePassiveButton.OnMouseOut.AddListener((Action)(() => updateButtonSprite.color = updateText.color = updateColor));
//         updateButtonSprite.color = updateText.color = updateColor;
//         updateButtonSprite.size *= 1.5f;
//         updateButton.SetActive(false);
//
// #if RELEASE
//             //フリープレイの無効化
//             var freeplayButton = GameObject.Find("/MainUI/FreePlayButton");
//             if (freeplayButton != null)
//             {
//                 freeplayButton.GetComponent<PassiveButton>().OnClick = new();
//                 freeplayButton.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() => Application.OpenURL("https://night-gua.github.io")));
//                 __instance.StartCoroutine(Effects.Lerp(0.01f, new Action<float>((p) => freeplayButton.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().SetText("网站"))));
//             }
// #endif
//         /*var bottomTemplate = GameObject.Find("InventoryButton");
//         if (bottomTemplate == null) return;*/
//     }
// }

// 参考：TownOfHost_Y和TownOfNext
public class ModNews
{
    public int Number;
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

    [HarmonyPatch(typeof(AnnouncementPopUp), nameof(AnnouncementPopUp.Init)), HarmonyPostfix]
    public static void Initialize(ref Il2CppSystem.Collections.IEnumerator __result)
    {
        static IEnumerator GetEnumerator()
        {
            while (AnnouncementPopUp.UpdateState == AnnouncementPopUp.AnnounceState.Fetching) yield return null;
            if (AnnouncementPopUp.UpdateState > AnnouncementPopUp.AnnounceState.Fetching && DataManager.Player.Announcements.AllAnnouncements.Count > 0) yield break;

            AnnouncementPopUp.UpdateState = AnnouncementPopUp.AnnounceState.Fetching;
            AllModNews.Clear();

            var lang = DataManager.Settings.Language.CurrentLanguage.ToString();
            
            var fileNames = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(x => x.StartsWith($"YuAntiCheat.Resources.ModNews."));
            foreach (var file in fileNames)
                AllModNews.Add(GetContentFromRes(file));

            AnnouncementPopUp.UpdateState = AnnouncementPopUp.AnnounceState.NotStarted;
        }

        __result = Effects.Sequence(GetEnumerator().WrapToIl2Cpp(), __result);
    }

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
        mn.Text = text;
        Main.Logger.LogInfo($"Number:{mn.Number}");
        Main.Logger.LogInfo($"Title:{mn.Title}");
        Main.Logger.LogInfo($"SubTitle:{mn.SubTitle}");
        Main.Logger.LogInfo($"ShortTitle:{mn.ShortTitle}");
        Main.Logger.LogInfo($"Date:{mn.Date}");
        return mn;
    }

    [HarmonyPatch(typeof(PlayerAnnouncementData), nameof(PlayerAnnouncementData.SetAnnouncements)), HarmonyPrefix]
    public static bool SetModAnnouncements(PlayerAnnouncementData __instance, [HarmonyArgument(0)] Il2CppReferenceArray<Announcement> aRange)
    {
        List<Announcement> list = new();
        foreach (var a in aRange) list.Add(a);
        foreach (var m in AllModNews) list.Add(m.ToAnnouncement());
        list.Sort((a1, a2) => { return DateTime.Compare(DateTime.Parse(a2.Date), DateTime.Parse(a1.Date)); });

        __instance.allAnnouncements = new Il2CppSystem.Collections.Generic.List<Announcement>();
        foreach (var a in list) __instance.allAnnouncements.Add(a);


        __instance.HandleChange();
        __instance.OnAddAnnouncement?.Invoke();

        return false;
    }
}
