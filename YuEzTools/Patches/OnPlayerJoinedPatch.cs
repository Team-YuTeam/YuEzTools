using Hazel;
using InnerNet;
using YuEzTools.Utils;
using YuEzTools.Modules;
using YuEzTools.UI;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
class OnPlayerJoinedPatch
{
    //private static int CID;
    public static void Prefix([HarmonyArgument(0)] ClientData client)
    {
        if (client == null)
        {
            Info(
                $"空加入,Client == null", "OnPlayerJoined");
            if (AmongUsClient.Instance.AmHost)
            {
                AmongUsClient.Instance.KickPlayer(client.Id, true);
                Info(
                    $"空加入,Client == null,已尝试ban", "OnPlayerJoined");
            }
            return;
        }
        Info(
            $"{client.PlayerName}(ClientID:{client.Id}/FriendCode:{client.FriendCode}/ProductUserId:{client.ProductUserId}) 加入房间", "OnPlayerJoined");
        GetPlayer.numImpostors = 0;
        GetPlayer.numCrewmates = 0;
        if (client.FriendCode.IsDevUser())
        {
            DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer,
                $"[Mod Dev] <color=#1E90FF>{client.PlayerName}</color> <color=#00FF7F>{Translator.GetString("JoinRoom")}</color>");
            SendInGamePatch.SendInGame($"[{client.FriendCode.GetDevJob()}] <color=#1E90FF>{client.PlayerName}</color> " +
                                       $"<color=#00FF7F>{Translator.GetString("JoinRoom")}</color>");
            return;
        }
        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"<color=#1E90FF>{client.PlayerName}</color> <color=#00FF7F>{Translator.GetString("JoinRoom")}</color>");
        SendInGamePatch.SendInGame($"<color=#1E90FF>{client.PlayerName}</color> <color=#00FF7F>{Translator.GetString("JoinRoom")}</color>");
        if (AmongUsClient.Instance.AmHost && client.FriendCode == "" && Toggles.KickNotLogin)
        {
            // 你知道的 Login是这样的
            AmongUsClient.Instance.KickPlayer(client.Id, true);
            Info($"{client?.PlayerName}未登录 已踢出", "Kick");
            DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"<color=#DC143C>{client.PlayerName}</color> <color=#EE82EE>{Translator.GetString("NotLogin")}</color>");
            SendInGamePatch.SendInGame($"<color=#DC143C>{client.PlayerName}</color> <color=#EE82EE>{Translator.GetString("NotLogin")}</color>");
            return;
        }
        else if (client.FriendCode == "")
        {
            DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"<color=#DC143C>{client.PlayerName}</color> <color=#EE82EE>{Translator.GetString("unKickNotLogin")}</color>");
            SendInGamePatch.SendInGame($"<color=#DC143C>{client.PlayerName}</color> <color=#EE82EE>{Translator.GetString("unKickNotLogin")}</color>");
        }

        if (Utils.Utils.CheckBanList(client.FriendCode, client?.ProductUserId) || Utils.Utils.CheckBanner(client.FriendCode, client?.ProductUserId) || Utils.Utils.CheckFirstBanList(client.FriendCode))
        {
            if (AmongUsClient.Instance.AmHost) AmongUsClient.Instance.KickPlayer(client.Id, true);
            Info($"{client?.PlayerName}黑名单 已揭示/踢出", "OnPlayerJoined");
            DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"<color=#DC143C>{client.PlayerName}</color> <color=#EE82EE>{Translator.GetString("BlackList")}</color>");
            SendInGamePatch.SendInGame($"<color=#DC143C>{client.PlayerName}</color> <color=#EE82EE>{Translator.GetString("BlackList")}</color>");
        }
    }

    public static void Postfix()
    {
        if (AmongUsClient.Instance.AmHost)
        {
            // Main.JoinedPlayer.Add(client.Character);
            return;
        }
    }
}
[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerLeft))]
class OnPlayerLeftPatch
{
    public static void Postfix([HarmonyArgument(0)] ClientData client)
    {
        if (GetPlayer.IsInGame)
        {
            client.Character.SetDisconnected();
        }
        if (AmongUsClient.Instance.AmHost)
        {
            // Main.JoinedPlayer.Remove(client.Character);
        }
        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"<color=#1E90FF>{client.PlayerName}</color> <color=#00FF7F>{Translator.GetString("LeftRoom")}</color>");
        Info(
            $"{client.PlayerName}(ClientID:{client.Id}/FriendCode:{client.FriendCode}/ProductUserId:{client.ProductUserId}) 退出房间", "OnPlayerJoined");
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
    public static void Postfix()
    {
        ServerAddManager.SetServerName();
        ShowDisconnectPopupPatch.ReasonByHost = string.Empty;
    }
}

[HarmonyPatch(typeof(IntroCutscene))]
class IntroCutscenePatch
{
    public static bool Ifkill = false;
    [HarmonyPatch(nameof(IntroCutscene.OnDestroy)), HarmonyPostfix]
    public static void OnDestroy_Postfix(IntroCutscene __instance)
    {
        if (Toggles.AutoStartGame && AmongUsClient.Instance.AmHost)
        {

            PlayerControl.LocalPlayer.RpcTeleport(Utils.Utils.GetBlackRoomPS());
            Info("尝试TP玩家", "GM");
            //PlayerControl.LocalPlayer.RpcExile();
            Ifkill = true;
            MurderHacker.murderHacker(PlayerControl.LocalPlayer, MurderResultFlags.Succeeded);
            Info("尝试击杀玩家", "GM");
            // PlayerState.GetByPlayerId(PlayerControl.LocalPlayer.PlayerId).SetDead();
            if (AmongUsClient.Instance.AmHost && Main.HasHacker)
            {
                Info("Host Try end game with room " +
                            GameStartManager.Instance.GameRoomNameCode.text, "StartPatch");
                try
                {
                    GameManager.Instance.RpcEndGame(GameOverReason.ImpostorDisconnect, false);
                }
                catch (System.Exception e)
                {
                    Error(e.ToString(), "StartPatch");
                }
                Main.HasHacker = false;
            }
        }
    }
}

[HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.DisconnectInternal))]
class DisconnectInternalPatch
{
    public static void Prefix(InnerNetClient __instance, DisconnectReasons reason, string stringReason)
    {
        ShowDisconnectPopupPatch.Reason = reason;
        ShowDisconnectPopupPatch.StringReason = stringReason;

        Info($"断开连接(理由:{reason}:{stringReason}，Ping:{__instance.Ping})", "Session");
        if (AmongUsClient.Instance.AmHost && GetPlayer.IsInGame)
        {
            try
            {
                GameManager.Instance.RpcEndGame(GameOverReason.ImpostorDisconnect, false);

            }
            catch (System.Exception e)
            {
                Error(e.ToString(), "Session");
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

        if (flags != SpawnFlags.IsClientCharacter) return;

        if (ServerUpdatePatch.re == 50605450 || GameStartManagerPatch.roomMode != RoomMode.Plus25) return;

        _ = new LateTask(() =>
        {
            if (client.Character == null) return;
            //if (Main.OverrideWelcomeMsg != "")
            Utils.Utils.SendMessage(string.Format(GetString("Message.Welcome"), GetPlayer.IsOnlineGame ? serverName : "Local", GameStartManager.Instance.GameRoomNameCode.text), client.Character.PlayerId);
            // else TemplateManager.SendTemplate("welcome", client.Character.PlayerId, true);
        }, 3f, "Welcome Message");

        Msg($"Spawn player data: ID {ownerId}: {client.PlayerName}", "InnerNetClientSpawn");
        if (GetPlayer.IsOnlineGame)
        {
            _ = new LateTask(() =>
            {
                if (GetPlayer.IsLobby && client.Character != null && LobbyBehaviour.Instance != null)//&& GetPlayer.IsVanillaServer)
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
}