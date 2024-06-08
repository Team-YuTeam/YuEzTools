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
        SendInGamePatch.SendInGame(TranslationController.Instance.currentLanguage.languageID == SupportedLangs.SChinese || TranslationController.Instance.currentLanguage.languageID == SupportedLangs.TChinese ? $"<color=#1E90FF>{client.PlayerName}</color> <color=#00FF7F>加入房间</color>" : $"<color=#1E90FF>{client.PlayerName}</color> <color=#00FF7F>Join This Room</color>");
        Main.PlayerStates[__instance.PlayerPrefab.PlayerId] = new(__instance.PlayerPrefab.PlayerId);
        // if(ChatUpdatePatch.TSLM > 3)
        //     PlayerControl.LocalPlayer.RpcSendChat(TranslationController.Instance.currentLanguage.languageID == SupportedLangs.SChinese || TranslationController.Instance.currentLanguage.languageID == SupportedLangs.TChinese ? $"Hi!我使用了{Main.ModName}反作弊(误踢或警告找Github的Night-GUA的YuAntiCheat)" : $"Hi!I use {Main.ModName}(False kick or warnings pls to Github's Night-GUA's YuAntiCheat)");
    }
}