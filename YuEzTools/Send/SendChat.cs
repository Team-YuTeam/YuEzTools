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
using YuEzTools.Get;
using YuEzTools;

namespace YuEzTools;

public class SendChat
{
    public static void Prefix(PlayerControl __instance)
    {
        if (Toggles.SafeMode && !AmongUsClient.Instance.AmHost)
        {
            SendInGamePatch.SendInGame(string.Format(Translator.GetString("AmnotHostSafeSeeHacker"), __instance.GetRealName()));
            Main.Logger.LogInfo($"已揭示 {__instance.GetRealName()}");
            return;
        }
        else if(!Toggles.SafeMode && !AmongUsClient.Instance.AmHost)
        {
            SendInGamePatch.SendInGame(string.Format(Translator.GetString("AmnotHostUnSafeSeeHacker"),__instance.GetRealName()));
            Main.Logger.LogInfo($"已尝试封禁 {__instance.GetRealName()}");
            return;
        }
        else if (AmongUsClient.Instance.AmHost)
        {
            SendInGamePatch.SendInGame(string.Format(Translator.GetString("AmHostSafeSeeHacker"),__instance.GetRealName()));
            Main.Logger.LogInfo($"已揭示 {__instance.GetRealName()}");
            return;
        }
        else
        {
            SendInGamePatch.SendInGame(string.Format(Translator.GetString("SeeHacker"),__instance.GetRealName()));
            Main.Logger.LogInfo($"已揭示 {__instance.GetRealName()}");
            return;
        }
        return;
    }
}

public class SendChatCloseDoor
{
    public static void Prefix(SystemTypes room)
    {
        SendInGamePatch.SendInGame(string.Format(Translator.GetString("SeeCloseDoorHacker"),room.ToString()));
        Main.Logger.LogInfo($"已揭示 {room.ToString()} 被非法关门");
        return;
    }
}
