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

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
internal class RPCHandlerPatch
{
    public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] byte callId,
        [HarmonyArgument(1)] MessageReader reader)
    {
        Main.Logger.LogMessage("From " +__instance.GetRealName() + "'s RPC:" + callId);
        if (AnitCheatForAll.ReceiveRpc(__instance, callId, reader) || AUMCheat.ReceiveInvalidRpc(__instance, callId) ||
            SMCheat.ReceiveInvalidRpc(__instance, callId))
        {
            Main.Logger.LogInfo("Hacker " + __instance.GetRealName());
            if (Main.safemode && !AmongUsClient.Instance.AmHost)
            {
                __instance.RpcSendChat($"<color={Main.ModColor}>{Main.ModName}</color>检测到我是<color=#191970>外挂</color>\n但无权力踢出我\n[来自<color=#4B0082>{AmongUsClient.Instance.PlayerPrefab.GetRealName()}</color>的<color={Main.ModColor}>{Main.ModName}</color>]");
                return false;//In safe mode,if you are not host,you can't ban other player
            }
            else if(!Main.safemode && !AmongUsClient.Instance.AmHost)
            {
                Main.Logger.LogInfo("Try kick " + __instance.GetRealName());
                __instance.RpcSendChat($"<color={Main.ModColor}>{Main.ModName}</color>检测到我是<color=#191970>外挂</color>\n并且正在尝试踢出我\n[来自<color=#4B0082>{AmongUsClient.Instance.PlayerPrefab.GetRealName()}</color>的<color={Main.ModColor}>{Main.ModName}</color>]");
                AmongUsClient.Instance.KickPlayer(__instance.GetClientId(), false);
                return false;
            }
            //PlayerControl Host = AmongUsClient.Instance.GetHost();
            else if (AmongUsClient.Instance.AmHost)
            {
                Main.Logger.LogInfo("Host Try kick " + __instance.GetRealName());
                __instance.RpcSendChat($"<color={Main.ModColor}>{Main.ModName}</color>检测到我是<color=#191970>外挂</color>\n并且正在尝试踢出我\n[来自房主<color=#4B0082>{AmongUsClient.Instance.PlayerPrefab.GetRealName()}</color>的<color={Main.ModColor}>{Main.ModName}</color>]");
                AmongUsClient.Instance.KickPlayer(__instance.GetClientId(), true);
            }
            return false;
        }

        return true;
    }
}