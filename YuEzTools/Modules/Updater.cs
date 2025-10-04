using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using YuEzTools.Patches;

namespace YuEzTools.Modules;

[HarmonyPatch]
public class ModUpdater
{
    public static string DownloadFileTempPath = Assembly.GetExecutingAssembly().Location + ".temp";
    private static IReadOnlyList<string> URLs => new List<string>
    {
#if DEBUG
        $"file:///{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "info.json")}",
#else
        "https://raw.githubusercontent.com/Team-YuTeam/YuEzTools/main/YuEzTools/info.json",
        // "https://gitee.com/xigua_ya/YuEzTools/raw/main/YuEzTools/info.json",
        //"https://gitlab.com/yu9522124/YuEzTools/-/raw/main/YuEzTools/info.json?ref_type=heads",
        "https://raw.kkgithub.com/Team-YuTeam/YuEzTools/main/YuEzTools/info.json",
        //"https://raw.gitcode.com/YuQZ/YuEzTools/raw/main/YuEzTools/info.json",
#endif
    };
    private static IReadOnlyList<string> GetInfoFileUrlList()
    {
        var list = URLs.ToList();
        if (Translator.IsChineseUser) list.Reverse();
        return list;
    }

    public static bool firstStart = true;

    public static bool hasUpdate = false;
    public static bool forceUpdate = false;
    public static bool isBroken = false;
    public static bool isChecked = false;
    public static bool DebugUnused = false;
    public static string versionInfoRaw = "";

    public static Version latestVersion = null;
    public static string showVer = "";
    public static Version DebugVer = null;
    public static bool CanUpdate = false;
    public static Version minimumVersion = null;
    public static int creation = 0;
    public static string md5 = "";
    public static int visit = 0;

    public static string announcement_zh = "";
    public static string announcement_en = "";
    public static string downloadUrl_github = "";
    public static string downloadUrl_kkgithub = "";
    // public static string downloadUrl_gitee = "";
    private static int retried = 0;
    private static bool firstLaunch = true;
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start)), HarmonyPostfix, HarmonyPriority(Priority.LowerThanNormal)]
    public static void StartPostfix()
    {
        CustomPopup.Init();

        // if (!isChecked && firstStart)
        CheckForUpdate();
        SetUpdateButtonStatus();
        AddVisit();

        firstStart = false;
    }
    private static readonly string URL_2018k = "http://api.2018k.cn";
    public static string UrlSetId(string url) => url + "?id=CAEFF4652FB44BAAA4F6E300404F528F";
    public static string UrlSetInfo(string url) => url + "/getExample";
    public static void AddVisit()
    {
        if(!firstStart) return;
        if(!Translator.IsChineseLanguageUser) return;
        Logger.Msg("开始从2018k检查visit", "AddVisit");
        string url = UrlSetId(UrlSetInfo(URL_2018k)) + "&data=visit";
        try
        {
            string[] data = Get(url).Split("|");
            visit = int.TryParse(data[0], out int x) ? x : 0;

            Logger.Info("Visit: " + data[0], "2018k");
        }
        catch (Exception ex)
        {
            Logger.Error($"增加Visit时发生错误，已忽略\n{ex}", "AddVisit", false);
            return;
        }
    }
    public static bool CheckNowFileMD5()
    {
        if (md5 == "DEBUGVERSION")
        {
            Logger.Info("MD5 Debug version, didn't check","CheckNowFileMD5");
            return false;
        }
        else if (GetMD5HashFromFile(Assembly.GetExecutingAssembly().Location) != md5)
        {
            Logger.Info("MD5 FAIL","CheckNowFileMD5");
            return true;
        }
        Logger.Info("MD5 TRUE","CheckNowFileMD5");
        return false;
    }
    public static void SetUpdateButtonStatus()
    {
        MainMenuManagerPatch.UpdateButton.gameObject.SetActive(isChecked && hasUpdate);
        MainMenuManagerPatch.PlayButton.gameObject.SetActive(!MainMenuManagerPatch.UpdateButton.activeSelf);
        var buttonText = MainMenuManagerPatch.UpdateButton.transform.FindChild("FontPlacer").GetChild(0).GetComponent<TextMeshPro>();
        Logger.Info(showVer, "ver");
        buttonText.text = $"{(CanUpdate ? Translator.GetString("updateButton"): Translator.GetString("updateNotice"))}\nv{showVer}";
        Logger.Info(showVer, "ver");
    }
    public static void Retry()
    {
        retried++;
        CustomPopup.Show(Translator.GetString("updateCheckPopupTitle"), Translator.GetString("PleaseWait"), null);
        _ = new LateTask(CheckForUpdate, 0.3f, "Retry Check Update");
    }
    public static void CheckForUpdate()
    {
        isChecked = false;
        DeleteOldFiles();

        foreach (var url in GetInfoFileUrlList())
        {
            Logger.Msg("Using "+url,"Updater");
            if (GetVersionInfo(url).GetAwaiter().GetResult())
            {
                isChecked = true;
                break;
            }
        }

        Logger.Msg("Check For Update: " + isChecked, "CheckRelease");
        isBroken = !isChecked;
        if (isChecked)
        {
            Logger.Info("Has Update: " + hasUpdate, "CheckRelease");
            Logger.Info("Latest Version: " + latestVersion.ToString(), "CheckRelease");
            Logger.Info("Minimum Version: " + minimumVersion.ToString(), "CheckRelease");
            Logger.Info("Creation: " + creation.ToString(), "CheckRelease");
            Logger.Info("Force Update: " + forceUpdate, "CheckRelease");
            Logger.Info("Github Url: " + downloadUrl_github, "CheckRelease");
            // Logger.Info("Gitee Url: " + downloadUrl_gitee, "CheckRelease");
            Logger.Info("kkGithub Url: " + downloadUrl_kkgithub, "CheckRelease");
            Logger.Info("Announcement (English): " + announcement_en, "CheckRelease");
            Logger.Info("Announcement (SChinese): " + announcement_zh, "CheckRelease");
            Logger.Info("File MD5: " + md5, "CheckRelease");
            Logger.Info(GetMD5HashFromFile(Assembly.GetExecutingAssembly().Location), "CheckRelease");

            if (!hasUpdate && CheckNowFileMD5() && Main.ModMode == 2)
            {
                Logger.Info("MD5 FAIL","CheckRelease");
                CustomPopup.Show(Translator.GetString(StringNames.AnnouncementLabel), Translator.GetString("Updater.NowFileMD5Fail"), new()
                {
                    (Translator.GetString("updateSource.Github"), () => StartUpdate(downloadUrl_github)),
                    // (Translator.GetString("updateSource.Gitee"), () => StartUpdate(downloadUrl_gitee)),
                    (Translator.GetString("updateSource.kkGithub"), () => StartUpdate(downloadUrl_kkgithub)),
                    (Translator.GetString(StringNames.ExitGame), Application.Quit)
                });
            }
            else if (firstLaunch || isBroken)
            {
                firstLaunch = false;
                var annos = Translator.IsChineseUser ? announcement_zh : announcement_en;
                if (isBroken) CustomPopup.Show(Translator.GetString(StringNames.AnnouncementLabel), annos, new() { (Translator.GetString(StringNames.ExitGame), Application.Quit) });
                else CustomPopup.Show(Translator.GetString(StringNames.AnnouncementLabel), annos, new()
                {
                    (Translator.GetString("updateSource.Afdian"), () => Application.OpenURL("https://afdian.com/a/yuqianzhi")),
                    (Translator.GetString("updateSource.BiliBili"), () => Application.OpenURL("https://www.bilibili.com/opus/898712994671755300")),
                    (Translator.GetString(Translator.IsChineseLanguageUser ? "updateSource.QQ" : "updateSource.github"), () => Application.OpenURL(Translator.IsChineseLanguageUser ? Main.QQUrl : "https://github.com/Team-YuTeam/YuEzTools/issues/new/choose")),
                    (Translator.GetString("updateSource.NextTime"), null)
                });
            }
        }
        else
        {
            if (retried >= 2) CustomPopup.Show(Translator.GetString("updateCheckPopupTitle"), Translator.GetString("updateCheckFailedExit"), new() { (Translator.GetString(StringNames.Okay), null) });
            else CustomPopup.Show(Translator.GetString("updateCheckPopupTitle"), Translator.GetString("updateCheckFailedRetry"), new() { (Translator.GetString("Retry"), Retry) });
        }

        SetUpdateButtonStatus();
    }
    public static void BeforeCheck()
    {
        isChecked = false;
        DeleteOldFiles();

        foreach (var url in GetInfoFileUrlList())
        {
            if (GetVersionInfo(url).GetAwaiter().GetResult())
            {
                isChecked = true;
                break;
            }
        }

        Logger.Msg("Check For Update: " + isChecked, "CheckRelease");
        isBroken = !isChecked;
        if (isChecked)
        {
            Logger.Info("Has Update: " + hasUpdate, "CheckRelease");
            Logger.Info("Latest Version: " + latestVersion.ToString(), "CheckRelease");
            Logger.Info("Minimum Version: " + minimumVersion.ToString(), "CheckRelease");
            Logger.Info("Creation: " + creation.ToString(), "CheckRelease");
            Logger.Info("Force Update: " + forceUpdate, "CheckRelease");
            Logger.Info("File MD5: " + md5, "CheckRelease");
            Logger.Info("Github Url: " + downloadUrl_github, "CheckRelease");
            // Logger.Info("Gitee Url: " + downloadUrl_gitee, "CheckRelease");
            Logger.Info("kkGithub Url: " + downloadUrl_kkgithub, "CheckRelease");
            Logger.Info("Announcement (English): " + announcement_en, "CheckRelease");
            Logger.Info("Announcement (SChinese): " + announcement_zh, "CheckRelease");
        }
    }
    public static string Get(string url)
    {
        string result = string.Empty;
        HttpClient req = new HttpClient();
        var res = req.GetAsync(url).Result;
        Stream stream = res.Content.ReadAsStreamAsync().Result;
        try
        {
            //获取内容
            using StreamReader reader = new(stream);
            result = reader.ReadToEnd();
        }
        finally
        {
            stream.Close();
        }
        return result;
    }
    public static async Task<bool> GetVersionInfo(string url)
    {
        Logger.Msg(url, "CheckRelease");
        try
        {
            string result;
            if (url.StartsWith("file:///"))
            {
                result = File.ReadAllText(url[8..]);
            }
            else
            {
                using HttpClient client = new();
                client.DefaultRequestHeaders.Add("User-Agent", "YuET Updater");
                using var response = await client.GetAsync(new Uri(url), HttpCompletionOption.ResponseContentRead);
                if (!response.IsSuccessStatusCode || response.Content == null)
                {
                    Logger.Error($"Failed: {response.StatusCode}", "CheckRelease");
                    return false;
                }
                result = await response.Content.ReadAsStringAsync();
                result = result.Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();
            }

            JObject data = JObject.Parse(result);

            DebugVer = new(data["DebugVer"]?.ToString());

            CanUpdate = bool.Parse(new(data["CanUpdate"]?.ToString()));

            md5 = data["md5"]?.ToString();
            latestVersion = new(data["version"]?.ToString());

            showVer = $"{latestVersion}";

            var minVer = data["minVer"]?.ToString();
            minimumVersion = minVer.ToLower() == "latest" ? latestVersion : new(minVer);
            creation = int.Parse(data["creation"]?.ToString());
            isBroken = data["allowStart"]?.ToString().ToLower() != "true";

            JObject announcement = data["announcement"].Cast<JObject>();
            announcement_en = announcement["English"]?.ToString();
            announcement_zh = announcement["SChinese"]?.ToString();

            JObject downloadUrl = data["url"].Cast<JObject>();
            downloadUrl_github = downloadUrl["github"]?.ToString();
            // downloadUrl_gitee = downloadUrl["gitee"]?.ToString().Replace("{{showVer}}", $"v{showVer}");
            downloadUrl_kkgithub = downloadUrl["kkgithub"]?.ToString();

            hasUpdate = Main.version < latestVersion;
            forceUpdate = Main.version < minimumVersion || creation > Main.PluginCreation;
#if DEBUG
            DebugUnused = Main.version < DebugVer;
            hasUpdate = forceUpdate = DebugUnused;
#endif
            return true;
        }
        catch
        {
            return false;
        }
    }
    public static void StartUpdate(string url = "waitToSelect")
    {
        if (url == "waitToSelect")
        {
            CustomPopup.Show(Translator.GetString("updatePopupTitle"), Translator.GetString("updateChoseSource"), new()
            {
                (Translator.GetString("updateSource.Github"), () => StartUpdate(downloadUrl_github)),
                // (Translator.GetString("updateSource.Gitee"), () => StartUpdate(downloadUrl_gitee)),
                (Translator.GetString("updateSource.kkGithub"), () => StartUpdate(downloadUrl_kkgithub)),
                (Translator.GetString(StringNames.Cancel), SetUpdateButtonStatus)
            });
            return;
        }

        Regex r = new Regex(@"^(http|https|ftp)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)?((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.[a-zA-Z]{2,4})(\:[0-9]+)?(/[^/][a-zA-Z0-9\.\,\?\'\\/\+&%\$#\=~_\-@]*)*$");
        if (!r.IsMatch(url))
        {
            CustomPopup.ShowLater(Translator.GetString("updatePopupTitleFialed"), string.Format(Translator.GetString("updatePingFialed"), "404 Not Found"), new() { (Translator.GetString(StringNames.Okay), SetUpdateButtonStatus) });
            return;
        }

        CustomPopup.Show(Translator.GetString("updatePopupTitle"), Translator.GetString("updatePleaseWait"), null);

        var task = DownloadDLL(url);
        task.ContinueWith(t =>
        {
            var (done, reason) = t.Result;
            string title = done ? Translator.GetString("updatePopupTitleDone") : Translator.GetString("updatePopupTitleFialed");
            string desc = done ? Translator.GetString("updateRestart") : reason;
            CustomPopup.ShowLater(title, desc, new() { (Translator.GetString(done ? StringNames.ExitGame : StringNames.Okay), done ? Application.Quit : null) });
            SetUpdateButtonStatus();
        });
    }
    public static void DeleteOldFiles()
    {
        try
        {
            foreach (var path in Directory.EnumerateFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "*.*"))
            {
                if (path.EndsWith(Path.GetFileName(Assembly.GetExecutingAssembly().Location))) continue;
                if (path.EndsWith("YuEzTools.dll") || path.EndsWith("Downloader.dll")) continue;
                Logger.Info($"{Path.GetFileName(path)} Deleted", "DeleteOldFiles");
                File.Delete(path);
            }
        }
        catch (Exception e)
        {
            Logger.Error($"清除更新残留失败\n{e}", "DeleteOldFiles");
        }
        return;
    }
    public static async Task<(bool, string)> DownloadDLL(string url)
    {
        Retry:
        File.Delete(DownloadFileTempPath);
        File.Create(DownloadFileTempPath).Close();

        Logger.Msg("Start Downlaod From: " + url, "DownloadDLL");
        Logger.Msg("Save To: " + DownloadFileTempPath, "DownloadDLL");
        try
        {
            using var client = new HttpClientDownloadWithProgress(url, DownloadFileTempPath);
            client.ProgressChanged += OnDownloadProgressChanged;
            await client.StartDownload();
            Logger.Info(GetMD5HashFromFile(DownloadFileTempPath), "LocalMD5");
            Logger.Info(md5, "CloudMD5");
            Thread.Sleep(100);
            if (GetMD5HashFromFile(DownloadFileTempPath) != md5)
            {
                File.Delete(DownloadFileTempPath);
                return (false, Translator.GetString("updateFileMd5Incorrect"));
            }
            else
            {
                var fileName = Assembly.GetExecutingAssembly().Location;
                File.Move(fileName, fileName + ".bak");
                File.Move("BepInEx/plugins/YuEzTools.dll.temp", fileName);
                return (true, null);
            }
        }
        catch (Exception ex)
        {
            File.Delete(DownloadFileTempPath);
            Logger.Error($"更新失败\n{ex.Message}", "DownloadDLL", false);
            return (false, Translator.GetString("downloadFailed"));
        }
    }
    private static void OnDownloadProgressChanged(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage)
    {
        string msg = $"{Translator.GetString("updateInProgress")}\n{totalFileSize / 1000}KB / {totalBytesDownloaded / 1000}KB  -  {(int)progressPercentage}%";
        Logger.Info(msg, "DownloadDLL");
        CustomPopup.UpdateTextLater(msg);
    }
    public static string GetMD5HashFromFile(string fileName)
    {
        try
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(fileName);
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
        catch (Exception ex)
        {
            Logger.Exception(ex, "GetMD5HashFromFile");
            return "";
        }
    }
}