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
using Epic.OnlineServices.Presence;
using InnerNet;
using UnityEngine;
using YuEzTools.Get;
using YuEzTools;
using YuEzTools.Patches;
using Byte = Il2CppSystem.Byte;
using static YuEzTools.Logger;

namespace YuEzTools;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
class OnPlayerJoinedPatch
{
    //private static int CID;
    public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData client)
    {
        GetPlayer.numImpostors = 0;
        GetPlayer.numCrewmates = 0;
        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"<color=#1E90FF>{client.PlayerName}</color> <color=#00FF7F>{Translator.GetString("JoinRoom")}</color>");
        Main.Logger.LogInfo(
            $"{client.PlayerName}(ClientID:{client.Id}/FriendCode:{client.FriendCode}/ProductUserId:{client.ProductUserId}) 加入房间");
        SendInGamePatch.SendInGame($"<color=#1E90FF>{client.PlayerName}</color> <color=#00FF7F>{Translator.GetString("JoinRoom")}</color>");
    }
}

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
class OnGameJoined
{
    //private static int CID;
    public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData client)
    {
        ShowDisconnectPopupPatch.ReasonByHost = string.Empty;
    }
}
[HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.DisconnectInternal))]
class DisconnectInternalPatch
{
    public static void Prefix(InnerNetClient __instance, DisconnectReasons reason, string stringReason)
    {
        ShowDisconnectPopupPatch.Reason = reason;
        ShowDisconnectPopupPatch.StringReason = stringReason;

        Logger.Info($"断开连接(理由:{reason}:{stringReason}，Ping:{__instance.Ping})", "Session");

        if (AmongUsClient.Instance.AmHost && GetPlayer.IsInGame)
            GameManager.Instance.RpcEndGame(GameOverReason.ImpostorDisconnect, false);
    }
}

[HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.Spawn))]
class InnerNetClientSpawnPatch
{
    public static void Postfix([HarmonyArgument(1)] int ownerId, [HarmonyArgument(2)] SpawnFlags flags)
    {
        ClientData client = GetPlayer.GetClientById(ownerId);
        Logger.Msg($"Spawn player data: ID {ownerId}: {client.PlayerName}", "InnerNetClientSpawn");
        if (GetPlayer.IsOnlineGame)
        {
            _ = new LateTask(() =>
                {
                    if (GetPlayer.IsLobby && client.Character != null && LobbyBehaviour.Instance != null )//&& GetPlayer.IsVanillaServer)
                {
                     // Only for vanilla
                     if (!client.Character.OwnedByHost())
                     {
                         MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(LobbyBehaviour.Instance.NetId, (byte)RpcCalls.LobbyTimeExpiring, SendOption.None, client.Id);
                         writer.WritePacked((int)GameStartManagerPatch.timer);
                         writer.Write(false);
                         AmongUsClient.Instance.FinishRpcImmediately(writer);
                     }
                     // Non-host modded client
                     else if (!client.Character.OwnedByHost())
                     {
                         MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SyncLobbyTimer, SendOption.Reliable, client.Id);
                         writer.WritePacked((int)GameStartManagerPatch.timer);
                         AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }
            }, 3.1f, "Send RPC or Sync Lobby Timer");
        }
    }
}