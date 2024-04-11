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
using YuAnitCheat.Get;
using YuAnitCheat;

namespace YuAnitCheat;

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
            if(Main.safemode) if (!AmongUsClient.Instance.AmHost) return false; //In safe mode,if you are not host,you can't ban other player
            Main.Logger.LogInfo("Try kick " + __instance.GetRealName());
            AmongUsClient.Instance.KickPlayer(__instance.GetClientId(), true);
            return false;
        }

        return true;
    }
}