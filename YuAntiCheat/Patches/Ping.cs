using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using Il2CppSystem;
using Rewired.UI.ControlMapper;
using TMPro;
using YuAntiCheat;
using YuAntiCheat.Get;
using YuAntiCheat.Patches;
using YuAntiCheat.Utils;
using static YuAntiCheat.Translator;

namespace YuAntiCheat;

[HarmonyPriority(Priority.Low)]
[HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
public static class PingTracker_Update
{
    private static float deltaTime;
    public static float fps;
    
    [HarmonyPostfix]
    public static void Postfix(PingTracker __instance)
    {
        var offset_x = 2.5f; //从右边缘偏移
        var offset_y = 6f; //从右边缘偏移
        if (HudManager.InstanceExists && HudManager._instance.Chat.chatButton.gameObject.active) offset_x -= 0.8f; //如果有聊天按钮，则有额外的偏移量
        //if (FriendsListManager.InstanceExists && FriendsListManager._instance.FriendsListButton.Button.active) offset_x -= 0.8f; //当有好友列表按钮时，还会有额外的偏移量
        __instance.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(offset_x, offset_y, 0f);

        __instance.text.text = __instance.ToString();
        __instance.text.alignment = TextAlignmentOptions.TopRight;
        __instance.text.text =
            $"<color={Main.ModColor}>{Main.ModName}</color><color=#00FFFF> v{Main.PluginVersion}</color>";
        if(Toggles.ShowCommit) __instance.text.text += $"<color=#00FFFF>({ThisAssembly.Git.Commit})</color>";
        if(Toggles.ShowModText) __instance.text.text += $"\n{Main.MainMenuText}";
        
        if(Toggles.FPSPlus && Application.targetFrameRate != 240) Application.targetFrameRate = 240;
        else if(!Toggles.FPSPlus && Application.targetFrameRate != 60) Application.targetFrameRate = 60;
        
        if(Toggles.ShowIsSafe)
        {
            if (Toggles.SafeMode)
                __instance.text.text +=
                    "\n<color=#DC143C>[Safe]</color>";
            else
                __instance.text.text +=
                    "\n<color=#1E90FF>[UnSafe]</color>";
        }
        if(Toggles.ShowSafeText)
        {
            if (Toggles.SafeMode)
                __instance.text.text += $"\n{Translator.GetString("SafeModeText")}";
            else
                __instance.text.text += $"\n{Translator.GetString("UnSafeModeText")}";
        }
        if(Toggles.ShowIsDark)
        {
            if (Toggles.DarkMode)
                __instance.text.text += "\n<color=#00BFFF>[Dark]</color>";
            else
                __instance.text.text += "\n<color=#00FA9A>[Light]</color>";
        }
        
        if (Toggles.ShowIsAutoExit) __instance.text.text += Toggles.AutoExit ? "\n<color=#DC143C>[AutoExit]</color>" : "\n<color=#1E90FF>[UnAutoExit]</color>";
#if DEBUG
__instance.text.text += "\n<color=#FFC0CB>[DEBUG]</color>";
#endif
#if CANARY
        __instance.text.text += "\n<color=#6A5ACD>[CANARY]</color>";
#endif
#if RELEASE
        __instance.text.text += "\n<color=#00FFFF>[RELEASE]</color>";
#endif
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        fps = Mathf.Ceil(1.0f / deltaTime);
        if(Toggles.ShowPing) __instance.text.text += Utils.Utils.getColoredPingText(AmongUsClient.Instance.Ping); // 书写Ping
        if(Toggles.ShowFPS) __instance.text.text += Utils.Utils.getColoredFPSText(fps); // 书写FPS
        
        DateTime dt = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now, TimeZoneInfo.Local);
        DateTime dt1 = TimeZoneInfo.ConvertTimeFromUtc(dt, TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time"));//参数对应国家或者时区   ***对于有夏令时冬令时的区域，程序会自动调整***
        if(Toggles.ShowLocalNowTime)__instance.text.text += GetString("Ping.NowTime") + DateTime.Now.ToString();
        if (Toggles.ShowUTC) __instance.text.text += "\r\n" + "UTC :" + dt.ToString();

        
        if (Toggles.ShowRoomTime && GetPlayer.IsLobby && AmongUsClient.Instance.AmHost && GetPlayer.IsOnlineGame)__instance.text.text += "\n<color=#FFD700>"+GameStartManagerPatch.countDown+"</color>";
        __instance.text.text += "\n<color=#FFFF00>By</color> <color=#FF0000>Yu</color>";
    }
}