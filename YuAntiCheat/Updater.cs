using HarmonyLib;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YuAntiCheat;
using YuAntiCheat.UI;

namespace YuAntiCheat.Updater;

[HarmonyPatch]
public class ModUpdater
{
    private static readonly string URL_Github = "https://api.github.com/repos/Night-GUA/YuAntiCheat";
    public static bool hasUpdate = false;
    public static bool forceUpdate = true;
    public static bool isChecked = false;
    public static Version latestVersion = null;
    public static string latestTitle = null;
    public static bool isBroken = false;
    public static string downloadUrl = null;
    public static string md5 = null;
    public static GenericPopup InfoPopup;

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start)), HarmonyPrefix]
    [HarmonyPriority(2)]
    public static void Start_Prefix(MainMenuManager __instance)
    {
        DeleteOldFiles();
        InfoPopup = UnityEngine.Object.Instantiate(Twitch.TwitchManager.Instance.TwitchPopup);
        InfoPopup.name = "InfoPopup";
        InfoPopup.TextAreaTMP.GetComponent<RectTransform>().sizeDelta = new(2.5f, 2f);
        if (!isChecked)
        {
            var done = false;
            if (CultureInfo.CurrentCulture.Name == "zh-CN")
            {
                done = CheckReleaseFromGithub(Main.BetaBuildURL.Value != "").GetAwaiter().GetResult();
            }
            else
            {
                done = CheckReleaseFromGithub(Main.BetaBuildURL.Value != "").GetAwaiter().GetResult();
            }
            Main.Logger.LogMessage("检查更新结果: " + done);
            Main.Logger.LogInfo("hasupdate: " + hasUpdate);
            Main.Logger.LogInfo("forceupdate: " + forceUpdate);
            Main.Logger.LogInfo("downloadUrl: " + downloadUrl);
            Main.Logger.LogInfo("latestVersionl: " + latestVersion);
        }
        // MainMenuManagerPatch.updateButton.SetActive(hasUpdate);
        // MainMenuManagerPatch.updateButton.transform.position = MainMenuManagerPatch.template.transform.position + new Vector3(0.25f, 0.75f);
        __instance.StartCoroutine(Effects.Lerp(0.01f, new Action<float>((p) =>
        {
            // MainMenuManagerPatch.updateButton.transform
            //     .GetChild(0).GetComponent<TMPro.TMP_Text>()
            //     .SetText($"一键更新\n{latestTitle}");
        })));
    }
    
    public static string Get(string url)
    {
        string result = "";
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
    
    public static async Task<bool> CheckReleaseFromGithub(bool beta = false)
    {
        Main.Logger.LogMessage("开始从Github检查更新");
        string url = beta ? Main.BetaBuildURL.Value : URL_Github + "/releases/latest";
        try
        {
            string result;
            using (HttpClient client = new())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "YuAC Updater");
                using var response = await client.GetAsync(new Uri(url), HttpCompletionOption.ResponseContentRead);
                if (!response.IsSuccessStatusCode || response.Content == null)
                {
                    Main.Logger.LogError($"状态码: {response.StatusCode}");
                    return false;
                }
                result = await response.Content.ReadAsStringAsync();
            }
            JObject data = JObject.Parse(result);
            if (beta)
            {
                latestTitle = data["name"].ToString();
                downloadUrl = data["url"].ToString();
                hasUpdate = latestTitle != ThisAssembly.Git.Commit;
            }
            else
            {
                latestVersion = new(data["tag_name"]?.ToString().TrimStart('v'));
                latestTitle = $"Ver. {latestVersion}";
                JArray assets = data["assets"].Cast<JArray>();
                for (int i = 0; i < assets.Count; i++)
                {
                    if (assets[i]["name"].ToString() == "YuAntiCheat.dll")
                        downloadUrl = assets[i]["browser_download_url"].ToString();
                }
                hasUpdate = latestVersion.CompareTo(Main.PluginVersion) > 0;
            }

            Main.Logger.LogInfo("hasupdate: " + hasUpdate);
            Main.Logger.LogInfo("forceupdate: " + forceUpdate);
            Main.Logger.LogInfo("downloadUrl: " + downloadUrl);
            Main.Logger.LogInfo("latestVersionl: " + latestVersion);
            Main.Logger.LogInfo("latestTitle: " + latestTitle);

            if (downloadUrl == null || downloadUrl == "")
            {
                Main.Logger.LogError("获取下载地址失败");
                return false;
            }
            isChecked = true;
            isBroken = false;
        }
        catch (Exception ex)
        {
            isBroken = true;
            Main.Logger.LogError($"发布检查失败\n{ex}");
            return false;
        }
        return true;
    }

    public static void StartUpdate(string url)
    {
        ShowPopup("请等待...", StringNames.Cancel, true, false);
        _ = DownloadDLL(url);
        return;
    }

    public static bool BackOldDLL()
    {
        try
        {
            foreach (var path in Directory.EnumerateFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "*.dll"))
            {
                Main.Logger.LogInfo($"{Path.GetFileName(path)} 已删除");
                File.Delete(path);
            }
            File.Move(Assembly.GetExecutingAssembly().Location + ".bak", Assembly.GetExecutingAssembly().Location);
        }
        catch
        {
            Main.Logger.LogError("回退老版本失败");
            return false;
        }
        return true;
    }
    public static void DeleteOldFiles()
    {
        try
        {
            foreach (var path in Directory.EnumerateFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "*.*"))
            {
                if (path.EndsWith(Path.GetFileName(Assembly.GetExecutingAssembly().Location))) continue;
                if (path.EndsWith("YuAntiCheat.dll")) continue;
                Main.Logger.LogInfo($"{Path.GetFileName(path)} 已删除");
                File.Delete(path);
            }
        }
        catch (Exception e)
        {
            Main.Logger.LogError($"清除更新残留失败\n{e}");
        }
        return;
    }
    private static readonly object downloadLock = new();
    public static async Task<bool> DownloadDLL(string url)
    {
        try
        {
            var savePath = "BepInEx/plugins/YuAntiCheat.dll.temp";
            File.Delete(savePath);

#pragma warning disable SYSLIB0014
            using WebClient client = new();
#pragma warning restore SYSLIB0014

            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadCallBack);
            client.DownloadFileAsync(new Uri(url), savePath);
            while (client.IsBusy) await Task.Delay(1);

            if (GetMD5HashFromFile(savePath) != md5)
            {
                File.Delete(savePath);
                ShowPopup("下载文件失败\n请稍后或重启游戏后稍后", StringNames.Okay, true, false);
                // MainMenuManagerPatch.updateButton.SetActive(true);
                // MainMenuManagerPatch.updateButton.transform.position = MainMenuManagerPatch.template.transform.position + new Vector3(0.25f, 0.75f);
            }
            else
            {
                var fileName = Assembly.GetExecutingAssembly().Location;
                File.Move(fileName, fileName + ".bak");
                File.Move("BepInEx/plugins/YuAntiCheat.dll.temp", fileName);
                ShowPopup("更新好啦~\n重启游戏试试吧", StringNames.ExitGame, true, true);
            }
        }
        catch (Exception ex)
        {
            Main.Logger.LogError($"更新失败\n{ex}");
            ShowPopup("更新失败\n请更换网络重试或手动更新", StringNames.ExitGame, true, true);
            return false;
        }
        return true;
    }
    private static void DownloadCallBack(object sender, DownloadProgressChangedEventArgs e)
    {
        try
        {
            ShowPopup($"更新中\n{e.BytesReceived}/{e.TotalBytesToReceive}({e.ProgressPercentage}%)", StringNames.Cancel);
        }
        catch (Exception ex)
        {
            Main.Logger.LogMessage(ex);
            throw;
        }
    }
    public static string GetMD5HashFromFile(string fileName)
    {
        try
        {
            FileStream file = new(fileName, FileMode.Open);
            MD5 md5 = MD5.Create();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();

            StringBuilder sb = new();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
        }
    }
    private static void DownloadCallBack(long total, long downloaded, double progress)
    {
        ShowPopup($"更新中...\n{downloaded}/{total}({progress}%)", StringNames.Cancel, true, false);
    }
    private static void ShowPopup(string message, StringNames buttonText, bool showButton = false, bool buttonIsExit = true)
    {
        if (InfoPopup != null)
        {
            InfoPopup.Show(message);
            var button = InfoPopup.transform.FindChild("ExitGame");
            if (button != null)
            {
                button.gameObject.SetActive(showButton);
                button.GetChild(0).GetComponent<TextTranslatorTMP>().TargetText = buttonText;
                button.GetComponent<PassiveButton>().OnClick = new();
                if (buttonIsExit) button.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() => Application.Quit()));
                else button.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() => InfoPopup.Close()));
            }
        }
    }
}