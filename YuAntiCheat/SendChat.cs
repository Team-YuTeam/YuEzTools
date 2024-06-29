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
        if (Toggles.SafeMode && !AmongUsClient.Instance.AmHost)
        {
            SendInGamePatch.SendInGame($"<color=#1E90FF>{__instance.GetRealName()}</color> 是外挂\n您不是房主 <color=#FFFF00>无权</color>惩罚");
            Main.Logger.LogInfo($"已揭示 {__instance.GetRealName()}");
            return;
        }
        else if(!Toggles.SafeMode && !AmongUsClient.Instance.AmHost)
        {
            SendInGamePatch.SendInGame($"<color=#1E90FF>{__instance.GetRealName()}</color> 是外挂\n您已开启<color=#1E90FF>[UnSafe]</color>模式 <color=#FFFF00>尝试<color=#FF4500>击杀</color>");
            Main.Logger.LogInfo($"已揭示 {__instance.GetRealName()}");
            return;
        }
        else if(!Toggles.SafeMode && AmongUsClient.Instance.AmHost)
        {
            SendInGamePatch.SendInGame($"<color=#1E90FF>{__instance.GetRealName()}</color> 是外挂\n您是房主 尝试<color=#FF4500>击杀</color>和<color=#FF0000>封禁</color>");
            Main.Logger.LogInfo($"已揭示 {__instance.GetRealName()}");
            return;
        }
        else if (AmongUsClient.Instance.AmHost)
        {
            SendInGamePatch.SendInGame($"<color=#1E90FF>{__instance.GetRealName()}</color> 是外挂\n您是房主 <color=#FFFF00>尝试<color=#FF0000>封禁</color>");
            Main.Logger.LogInfo($"已揭示 {__instance.GetRealName()}");
            return;
        }
        else
        {
            SendInGamePatch.SendInGame($"<color=#1E90FF>{__instance.GetRealName()}</color> 是外挂");
            Main.Logger.LogInfo($"已揭示 {__instance.GetRealName()}");
            return;
        }
        return;
    }
}