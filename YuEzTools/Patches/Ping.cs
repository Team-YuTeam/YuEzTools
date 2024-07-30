using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System.Text;
using Il2CppSystem;
using Rewired.UI.ControlMapper;
using TMPro;
using YuEzTools;
using YuEzTools.Utils;
using YuEzTools.Get;
using YuEzTools.Patches;
using static YuEzTools.Translator;

namespace YuEzTools;

[HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
internal class PingTrackerUpdatePatch
{
    private static float deltaTime;
    public static string ServerName = "";
    private static TextMeshPro pingTrackerCredential = null;
    private static AspectPosition pingTrackerCredentialAspectPos = null;
    public static float fps;

    private static void Postfix(PingTracker __instance)
    {
        __instance.text.text = "";
        if (pingTrackerCredential == null)
        {
            var uselessPingTracker = UnityEngine.Object.Instantiate(__instance, __instance.transform.parent);
            pingTrackerCredential = uselessPingTracker.GetComponent<TextMeshPro>();
            UnityEngine.Object.Destroy(uselessPingTracker);
            pingTrackerCredential.alignment = TextAlignmentOptions.TopRight;
            pingTrackerCredential.color = new(1f, 1f, 1f, 0.7f);
            pingTrackerCredential.rectTransform.pivot = new(1f, 1f); // 中心を右上角に設定
            pingTrackerCredentialAspectPos = pingTrackerCredential.GetComponent<AspectPosition>();
            pingTrackerCredentialAspectPos.Alignment = AspectPosition.EdgeAlignments.RightTop;
        }

        if (pingTrackerCredentialAspectPos)
        {
            pingTrackerCredentialAspectPos.DistanceFromEdge =
                DestroyableSingleton<HudManager>.InstanceExists &&
                DestroyableSingleton<HudManager>.Instance.Chat.chatButton.gameObject.active
                    ? new(2.5f, 0f, -800f)
                    : new(1.8f, 0f, -800f);
        }

        __instance.text.alignment = TextAlignmentOptions.TopRight;

        StringBuilder sb = new();
        sb.Append($"<color={Main.ModColor}>{Main.ModName}</color><color=#00FFFF> v{Main.PluginVersion}</color>");
        if (Toggles.ShowCommit) sb.Append($"<color=#00FFFF>({ThisAssembly.Git.Commit})</color>");
        if (Toggles.ShowModText) sb.Append($"\r\n").Append($"{Main.MainMenuText}");

        if (Toggles.FPSPlus && Application.targetFrameRate != 240) Application.targetFrameRate = 240;
        else if (!Toggles.FPSPlus && Application.targetFrameRate != 60) Application.targetFrameRate = 60;

        if (Toggles.ShowIsSafe)
        {
            if (Toggles.SafeMode)
                sb.Append($"\r\n").Append($"<color=#DC143C>[Safe]</color>");
            else
                sb.Append($"\r\n").Append($"<color=#1E90FF>[UnSafe]</color>");
        }

        if (Toggles.ShowSafeText)
        {
            if (Toggles.SafeMode)
                sb.Append($"\r\n").Append($"{Translator.GetString("SafeModeText")}");
            else
                sb.Append($"\r\n").Append($"{Translator.GetString("UnSafeModeText")}");
        }

        if (Toggles.ShowIsDark)
        {
            if (Toggles.DarkMode)
                sb.Append($"\r\n").Append("<color=#00BFFF>[Dark]</color>");
            else
                sb.Append($"\r\n").Append("<color=#00FA9A>[Light]</color>");
        }

        if (Toggles.ShowIsAutoExit)
            sb.Append($"\r\n").Append(Toggles.AutoExit
                ? "<color=#DC143C>[AutoExit]</color>"
                : "<color=#1E90FF>[UnAutoExit]</color>");
#if DEBUG
        sb.Append($"\r\n").Append("<color=#FFC0CB>[DEBUG]</color>");
#endif
#if CANARY
        sb.Append($"\r\n").Append("<color=#6A5ACD>[CANARY]</color>");
#endif
#if RELEASE
        sb.Append($"\r\n").Append("<color=#00FFFF>[RELEASE]</color>");
#endif
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        fps = Mathf.Ceil(1.0f / deltaTime);
        var ping = AmongUsClient.Instance.Ping;
        if (Toggles.ShowPing) sb.Append($"\r\n").Append(Utils.Utils.getColoredPingText(AmongUsClient.Instance.Ping) + "<size=60%>Ping</size></color>"); // 书写Ping
        if (Toggles.ShowFPS) sb.Append(!Toggles.ShowPing ? $"\r\n" : " ").Append(Utils.Utils.getColoredFPSText(fps) + "<size=60%>FPS</size></color>"); // 书写FPS
        if(Toggles.ShowServer) sb.Append((!Toggles.ShowFPS && !Toggles.ShowPing) ? $"\r\n" : " ").Append("  " + (GetPlayer.IsOnlineGame ? ServerName : GetString("Local")));
        // sb.Append($"\r\n")
        //     .Append(
        //         $"{Utils.Utils.getColoredPingText(ping)} <size=60%>Ping</size></color>  {Utils.Utils.getColoredFPSText(fps)} <size=60%>FPS</size></color>{"  " + (GetPlayer.IsOnlineGame ? ServerName : GetString("Local"))}");

        DateTime dt = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now, TimeZoneInfo.Local);
        DateTime dt1 =
            TimeZoneInfo.ConvertTimeFromUtc(dt,
                TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time")); //参数对应国家或者时区   ***对于有夏令时冬令时的区域，程序会自动调整***
        if (Toggles.ShowLocalNowTime) sb.Append($"\r\n").Append(DateTime.Now.ToString());
        if (Toggles.ShowUTC) sb.Append($"\r\n").Append("UTC: " + dt.ToString());


        if (Toggles.ShowRoomTime && GetPlayer.IsLobby && AmongUsClient.Instance.AmHost && GetPlayer.IsOnlineGame)
            sb.Append($"\r\n").Append($"<color=#FFD700>" + GameStartManagerPatch.countDown + "</color>");
        sb.Append($"\r\n").Append($"<color=#FFFF00>By</color> <color=#FF0000>Yu</color>");
        
        pingTrackerCredential.gameObject.SetActive(true);
        pingTrackerCredential.text = sb.ToString();
        if (GameSettingMenu.Instance?.gameObject?.active ?? false)
            pingTrackerCredential.text = "";
    }
}