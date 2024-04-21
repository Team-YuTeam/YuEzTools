using AmongUs.GameOptions;
using Hazel;
using System;
using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using InnerNet;
using UnityEngine;
using YuAntiCheat.Get;
using YuAntiCheat;

namespace YuAntiCheat;

public class SendChat
{
    public static void Prefix(PlayerControl __instance)
    {
        if (Main.safemode && !AmongUsClient.Instance.AmHost)
        {
            Main.Logger.LogInfo("无法权力踢出 已揭示");
            PlayerControl.LocalPlayer.RpcSendChat(
                $"{Main.ModName} 检测到 {__instance.GetRealName()} 是外挂 但无权力踢出");
            return;
        }
        else if(!Main.safemode && !AmongUsClient.Instance.AmHost)
        {
            Main.Logger.LogInfo("尝试踢出 已揭示");
            PlayerControl.LocalPlayer.RpcSendChat(
                $"{Main.ModName} 检测到 {__instance.GetRealName()} 是外挂 并且正在尝试踢出");
            return;
        }
        else if (AmongUsClient.Instance.AmHost)
        {
            Main.Logger.LogInfo("房主踢出 已揭示");
            PlayerControl.LocalPlayer.RpcSendChat(
                $"{Main.ModName} 检测到 {__instance.GetRealName()} 是外挂 并且正在尝试封禁");
            return;
        }
        return;
    }
}