using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using Sentry.Unity.NativeUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AmongUs.HTTP;
using Il2CppSystem.Globalization;
using TMPro;
using UnityEngine;
using YuEzTools;
using YuEzTools.Modules;
using YuEzTools.Patches;
using YuEzTools.UI;

namespace YuEzTools.Modules;

public class CloudBanlistLoader : MonoBehaviour
{
    public static bool isLoading = false;
    public static bool isBrocked;
    // private static int nowNumber = 0;

    private static IReadOnlyList<string> URLs => new List<string>
    {
        "https://raw.githubusercontent.com/Team-YuTeam/YuEzTools/main/YuEzTools/Resources/BlackList.txt",
        "https://gitee.com/xigua_ya/YuEzTools/raw/main/YuEzTools/Resources/BlackList.txt",
        //"https://gitlab.com/yu9522124/YuEzTools/-/raw/main/YuEzTools/info.json?ref_type=heads",
        // "https://raw.kkgithub.com/Team-YuTeam/YuEzTools/main/YuEzTools/Resources/BlackList.txt",
        //"https://raw.gitcode.com/YuQZ/YuEzTools/raw/main/YuEzTools/info.json",
    };
    

    public static void DownloadBanlist(string url = "NowSet")
    {
        isLoading = true;
        if (url == "NowSet")
        {
            Logger.Info("Set Url","DownloadBanlist");
            url = !Translator.IsChineseUser ? URLs[0] : URLs[1];
        }
        
        Regex r = new Regex(@"^(http|https|ftp)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)?((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.[a-zA-Z]{2,4})(\:[0-9]+)?(/[^/][a-zA-Z0-9\.\,\?\'\\/\+&%\$#\=~_\-@]*)*$");
        Logger.Info("Set Regex","DownloadBanlist");
        if (!r.IsMatch(url))
        {
            Logger.Info("!IsMatch", "IsMatch");
            CustomPopup.ShowLater(Translator.GetString("DownloadBanlistFailedTitle"),
                string.Format(Translator.GetString("DownloadBanlistFailed"), "404 Not Found"),
                new() { (Translator.GetString(StringNames.Okay), null) });
            Logger.Error("ERROR Regex错误", "DownloadBanlist");
            isLoading = false;
            isBrocked = true;
            return;
        }

        Logger.Info("Set Download Task","DownloadBanlist");
        var task = DownloadBanlistD(url);
        Logger.Info("Start download","DownloadBanlist");
        task.ContinueWith(t =>
        {
            Logger.Info("ContinueWith", "DownloadBanlist");
            
            var (done, reason) = t.Result;
            if (!done)
            {
                Logger.Info($"下载失败！\nReason: {reason} \nNow Url is {url}", "DownloadBanlist");
                // nowNumber++;
                // DownloadBanlist();
                isLoading = false;
                return;
            }
            Logger.Info("下载完成", "DownloadBanlist");
            isLoading = false;
            return;
        });
        
    }

    public static long? TotalFileSize = 0,TotalBytesDownloaded = 0;
    public static double? ProgressPercentage = 0;
    private static void OnDownloadProgressChanged(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage)
    {
        TotalFileSize = totalFileSize;
        TotalBytesDownloaded = totalBytesDownloaded;
        ProgressPercentage = progressPercentage;
        string msg = $"Downloading...\n{totalFileSize / 1000}KB / {totalBytesDownloaded / 1000}KB  -  {(int)progressPercentage}%";
        Logger.Info(msg, "DownloadDLL");
        // CustomPopup.UpdateTextLater(msg);
    }
    public static string DownloadFileTempPath = "";

    public static async Task<(bool, string)> DownloadBanlistD(string url)
    {
        Logger.Info("Task DownloadBanlistD","DownloadBanlistD");

        DownloadFileTempPath = Main.userProfile + "Banlist.txt";
        Logger.Info("Set DownloadFileTempPath","DownloadBanlistD");
        File.Delete(DownloadFileTempPath);
        Logger.Info("Delete DownloadFileTempPath","DownloadBanlistD");
        // Directory.CreateDirectory(Main.userProfile);
        File.Create(DownloadFileTempPath).Close();
        // File.Create(DownloadFileTempPath);
        Logger.Info("Create Banlist.txt file","DownloadBanlistD");

        Logger.Msg("Start Download From: " + url, "DownloadBanlistD");
        Logger.Msg("Save To: " + DownloadFileTempPath, "DownloadBanlistD");
        try
        {
            using var client = new HttpClientDownloadWithProgress(url, DownloadFileTempPath);
            client.ProgressChanged += OnDownloadProgressChanged;
            await client.StartDownload();
            Thread.Sleep(100);
            // File.Move(Main.userProfile + "Banlist.txt", Main.userProfile + "Banlist.txt" + ".bak");
            // File.Move(Main.userProfile + "Banlist.txt" + ".temp", Main.userProfile + "Banlist.txt");
            return (true, null);
        }
        catch (Exception ex)
        {
            File.Delete(DownloadFileTempPath);
            Logger.Error($"下载banlist失败\n{ex.Message}", "DownloadBanlistD", false);
            return (false, Translator.GetString("DownloadBanlistFailedTitle"));
        }
    }
}
