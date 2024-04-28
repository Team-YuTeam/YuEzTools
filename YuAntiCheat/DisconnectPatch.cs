using HarmonyLib;
using YuAntiCheat;

namespace YuAntiCheat;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnDisconnected))]
internal class OnDisconnectedPatch
{
    public static void Postfix(AmongUsClient __instance)
    {
        Main.VisibleTasksCount = false;
    }
}

[HarmonyPatch(typeof(DisconnectPopup), nameof(DisconnectPopup.DoShow))]
internal class ShowDisconnectPopupPatch
{
    public static DisconnectReasons Reason;
    public static string StringReason;
    public static void Postfix(DisconnectPopup __instance)
    {
        new LateTask(() =>
        {
            if (__instance != null)
            {
                switch (Reason)
                {
                    case DisconnectReasons.Hacking:
                        __instance.SetText("神经树懒反作弊（YuAC的和AUAC冲突啦~）\r\nYou have been kicked out by the anti-cheat");
                        break;
                    case DisconnectReasons.Banned:
                        __instance.SetText("略~被房间封禁了\r\nYou are this room's Banner");
                        break;
                    case DisconnectReasons.Kicked:
                        __instance.SetText("略~被踢咯~\r\nYou are this room's Kicker");
                        break;
                    case DisconnectReasons.GameNotFound:
                        __instance.SetText("什么记忆啊!这是错误的房间号!\r\nNot found this room!This room code is wrong!");
                        break;
                    case DisconnectReasons.GameStarted:
                        __instance.SetText("房间开咯~ta们不等你咯！\r\nThis room is started,pls wait");
                        break;
                    case DisconnectReasons.GameFull:
                        __instance.SetText("房间满咯~这个家里没有你的位置啦！\r\nThe room is full,pls try again later");
                        break;
                    case DisconnectReasons.IncorrectVersion:
                        __instance.SetText("啧...小丑~人家的AU版本都不知道~AU版本错啦!\r\nYour AmongUs ver is different from this room");
                        break;
                    case DisconnectReasons.Error:
                        //if (StringReason.Contains("Couldn't find self")) __instance.SetText(GetString("DCNotify.DCFromServer"));
                        if (StringReason.Contains("Failed to send message")) __instance.SetText("啊哦,和服务器走散了/(ㄒoㄒ)/~~断链啦!\r\nYou disconnected from the server.");
                        break;
                    case DisconnectReasons.Custom:
                        if (StringReason.Contains("Reliable packet")) __instance.SetText("啊哦,和服务器走散了/(ㄒoㄒ)/~~断链啦!\r\nYou disconnected from the server.");
                        else if (StringReason.Contains("remote has not responded to")) __instance.SetText("啊哦,和服务器走散了/(ㄒoㄒ)/~~断链啦!\r\nYou disconnected from the server.");
                        break;
                }
                Main.Logger.LogError("被踢出/封禁 理由：" + Reason.ToString());
            }
        }, 0.01f, "Override Disconnect Text");
    }
}