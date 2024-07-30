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
using Steamworks;
using UnityEngine;
using YuEzTools.Get;
using YuEzTools;
using YuEzTools.Patches;
using Byte = Il2CppSystem.Byte;
using static YuEzTools.Logger;
using static YuEzTools.Translator;

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
[HarmonyPatch(typeof(AmongUsClient),nameof(AmongUsClient.OnPlayerLeft))]
class OnPlayerLeftPatch{
    public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData client){
        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"<color=#1E90FF>{client.PlayerName}</color> <color=#00FF7F>{Translator.GetString("LeftRoom")}</color>");
        Main.Logger.LogInfo(
            $"{client.PlayerName}(ClientID:{client.Id}/FriendCode:{client.FriendCode}/ProductUserId:{client.ProductUserId}) 退出房间");
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
    public static string serverName = "";
    public static void Postfix([HarmonyArgument(1)] int ownerId, [HarmonyArgument(2)] SpawnFlags flags)
    {
        
        ClientData client = GetPlayer.GetClientById(ownerId);
        
        if(flags != SpawnFlags.IsClientCharacter) return;
        
        if(ServerUpdatePatch.re == 50605450) return;
        
        _ = new LateTask(() =>
        {
            if (client.Character == null) return;
            //if (Main.OverrideWelcomeMsg != "")
            Utils.Utils.SendMessage(string.Format(GetString("Message.Welcome"),(GetPlayer.IsOnlineGame ? serverName : GetString("Local"))), client.Character.PlayerId);
            // else TemplateManager.SendTemplate("welcome", client.Character.PlayerId, true);
        }, 3f, "Welcome Message");
        
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
}[HarmonyPatch(typeof(LobbyBehaviour))]
public class LobbyBehaviourPatch
{
    [HarmonyPatch(nameof(LobbyBehaviour.Update)), HarmonyPostfix]
    public static void Update_Postfix(LobbyBehaviour __instance)
    {
        System.Func<ISoundPlayer, bool> lobbybgm = x => x.Name.Equals("MapTheme");
        ISoundPlayer MapThemeSound = SoundManager.Instance.soundPlayers.Find(lobbybgm);
        if (!Toggles.CloseMusicOfOr)
        {
            if (MapThemeSound == null) return;
            SoundManager.Instance.StopNamedSound("MapTheme");
        }
        else
        {
            if (MapThemeSound != null) return;
            SoundManager.Instance.CrossFadeSound("MapTheme", __instance.MapTheme, 0.5f);
        }
    }
}