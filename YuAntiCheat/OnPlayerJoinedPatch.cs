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
using Byte = Il2CppSystem.Byte;

namespace YuAntiCheat;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
class OnPlayerJoinedPatch
{
    public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData client)
    {
        Main.Logger.LogInfo($"{client.PlayerName}(ClientID:{client.Id}/FriendCode:{client.FriendCode}/ProductUserId:{client.ProductUserId}) 加入房间");
        if(ChatUpdatePatch.TSLM > 3)
            PlayerControl.LocalPlayer.RpcSendChat($"Hi!我使用了'{Main.ModName}'反作弊模组!超强!请小心谨慎哦(误踢/警告加q群711908097或Github的Night-GUA/YuAntiCheat项目) Hi!I use '{Main.ModName}'!So good!Don't Cheat!(False kick/warnings Github's Night-GUA/YuAntiCheat project)");
    }
}