using Hazel;
using HarmonyLib;
using Il2CppSystem;
using InnerNet;
using YuEzTools.Get;
using YuEzTools.Patches;
using YuEzTools.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using YuEzTools.Modules;
using static YuEzTools.Translator;
using Exception = Il2CppSystem.Exception;

namespace YuEzTools;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
class OnPlayerJoinedPatch
{
    //private static int CID;
    public static void Prefix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData client)
    {
        if (client == null)
        {
            Logger.Info(
                $"空加入,Client == null","OnPlayerJoined");
            if (AmongUsClient.Instance.AmHost)
            {
                AmongUsClient.Instance.KickPlayer(client.Id,true);
                Logger.Info(
                    $"空加入,Client == null,已尝试ban","OnPlayerJoined");
            }
            return;
        }
        Main.Logger.LogInfo(
            $"{client.PlayerName}(ClientID:{client.Id}/FriendCode:{client.FriendCode}/ProductUserId:{client.ProductUserId}) 加入房间");
        GetPlayer.numImpostors = 0;
        GetPlayer.numCrewmates = 0;
        if (client.FriendCode.IsDevUser())
        {
            DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"[Mod Dev] <color=#1E90FF>{client.PlayerName}</color> <color=#00FF7F>{Translator.GetString("JoinRoom")}</color>");
            SendInGamePatch.SendInGame($"[{client.FriendCode.GetDevJob()}] <color=#1E90FF>{client.PlayerName}</color> <color=#00FF7F>{Translator.GetString("JoinRoom")}</color>");
            return;
        }
        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"<color=#1E90FF>{client.PlayerName}</color> <color=#00FF7F>{Translator.GetString("JoinRoom")}</color>");
        SendInGamePatch.SendInGame($"<color=#1E90FF>{client.PlayerName}</color> <color=#00FF7F>{Translator.GetString("JoinRoom")}</color>");

        if (GetPlayer.IsLocalGame || client.FriendCode.IsDevUsers()) return;
        if (AmongUsClient.Instance.AmHost && client.FriendCode == "" && Toggles.KickNotLogin)
        {
            // 你知道的 Login是这样的
            AmongUsClient.Instance.KickPlayer(client.Id, true);
            Logger.Info($"{client?.PlayerName}未登录 已踢出", "Kick");
            DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"<color=#DC143C>{client.PlayerName}</color> <color=#EE82EE>{Translator.GetString("NotLogin")}</color>");
            SendInGamePatch.SendInGame($"<color=#DC143C>{client.PlayerName}</color> <color=#EE82EE>{Translator.GetString("NotLogin")}</color>");
            return;
        }
        else if(client.FriendCode == "")
        {
            DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"<color=#DC143C>{client.PlayerName}</color> <color=#EE82EE>{Translator.GetString("unKickNotLogin")}</color>");
            SendInGamePatch.SendInGame($"<color=#DC143C>{client.PlayerName}</color> <color=#EE82EE>{Translator.GetString("unKickNotLogin")}</color>");
        }

        if (Utils.Utils.CheckBanList(client.FriendCode,client?.ProductUserId) || Utils.Utils.CheckBanner(client.FriendCode,client?.ProductUserId)|| Utils.Utils.CheckCloudBanner(client.FriendCode,client?.ProductUserId) || Utils.Utils.CheckFirstBanList(client.FriendCode))
        {
            if(AmongUsClient.Instance.AmHost) AmongUsClient.Instance.KickPlayer(client.Id, true);
            Logger.Info($"{client?.PlayerName}黑名单 已揭示/踢出", "OnPlayerJoined");
            DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"<color=#DC143C>{client.PlayerName}</color> <color=#EE82EE>{Translator.GetString("BlackList")}</color>");
            SendInGamePatch.SendInGame($"<color=#DC143C>{client.PlayerName}</color> <color=#EE82EE>{Translator.GetString("BlackList")}</color>");
        }
    }

    public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData client)
    {
        if (AmongUsClient.Instance.AmHost)
        {
            // Main.JoinedPlayer.Add(client.Character);
            return;
        }
    }
}
[HarmonyPatch(typeof(AmongUsClient),nameof(AmongUsClient.OnPlayerLeft))]
class OnPlayerLeftPatch{
    public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData client){
        if (GetPlayer.IsInGame)
        {
            client.Character.SetDisconnected();
        }
        if (AmongUsClient.Instance.AmHost)
        {
            // Main.JoinedPlayer.Remove(client.Character);
        }
        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"<color=#1E90FF>{client.PlayerName}</color> <color=#00FF7F>{Translator.GetString("LeftRoom")}</color>");
        Utils.Utils.NotificationPop(
            $"<color=#1E90FF>{client.PlayerName}</color> <color=#00FF7F>{Translator.GetString("LeftRoom")}</color>");
        Main.Logger.LogInfo(
            $"{client.PlayerName}(ClientID:{client.Id}/FriendCode:{client.FriendCode}/ProductUserId:{client.ProductUserId}) 退出房间");
    }
}

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
class OnGameJoined
{
    //private static int CID;
    // public static void Prefix(AmongUsClient __instance)
    // {
    //     if (AmongUsClient.Instance.AmHost && Toggles.AutoStartGame)
    //         MurderHacker.murderHacker(PlayerControl.LocalPlayer, MurderResultFlags.Succeeded);
    // }
    public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData client)
    {
        ShowDisconnectPopupPatch.ReasonByHost = string.Empty;
    }
}

[HarmonyPatch(typeof(IntroCutscene))]
class IntroCutscenePatch
{
    [HarmonyPatch(nameof(IntroCutscene.OnDestroy)), HarmonyPostfix]
    public static void OnDestroy_Postfix(IntroCutscene __instance)
    {

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
        {
            try
            {
                GameManager.Instance.RpcEndGame(GameOverReason.ImpostorDisconnect, false);

            }
            catch (System.Exception e)
            {
                Logger.Error(e.ToString(), "Session");
            }
        }
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
        
        if(GameStartManagerPatch.roomMode != RoomMode.Plus25) return;
        
        Logger.Msg($"Spawn player data: ID {ownerId}: {client.PlayerName}", "InnerNetClientSpawn");
    }
}
[HarmonyPatch(typeof(LobbyBehaviour))]
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
    [HarmonyPatch(nameof(LobbyBehaviour.Start)),HarmonyPostfix]
    public static void Start_Postfix(LobbyBehaviour __instance)
    {
        
    }
}
