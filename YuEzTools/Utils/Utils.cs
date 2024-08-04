using UnityEngine;
using InnerNet;
using System.Linq;
using Il2CppSystem.Collections.Generic;
using System.IO;
using Hazel;
using System.Reflection;
using AmongUs.GameOptions;
using Sentry.Internal.Extensions;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using static YuEzTools.Translator;
using System;
using System.Text.RegularExpressions;
using YuEzTools.Get;

namespace YuEzTools.Utils;

public static class Utils
{
    public static void SendMessage(string text, byte sendTo = byte.MaxValue, string title = "<Default>", bool removeTags = false)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        if (title == "<Default>") title = "<color=#aaaaff>" + GetString("DefaultSystemMessageTitle") + "</color>";
        Main.isChatCommand = true;
        Main.MessagesToSend.Add((removeTags ? text.RemoveHtmlTags() : text, sendTo, title + '\0'));
    }
    public static Vector2 GetBlackRoomPS()
    {
        return GetPlayer.GetActiveMapId() switch
        {
            0 => new(-27f, 3.3f), // The Skeld
            1 => new(-11.4f, 8.2f), // MIRA HQ
            2 => new(42.6f, -19.9f), // Polus
            4 => new(-16.8f, -6.2f), // Airship
            5 => new(9.4f, 17.9f), // The Fungle
            _ => throw new System.NotImplementedException(),
        };
    }
    public static Vector2 LocalPlayerLastTp;
    public static bool LocationLocked = false;
    public static void RpcTeleport(this PlayerControl player, Vector2 location)
    {
        Logger.Info($" {GetPlayer.GetNameRole(player)} => {location}", "RpcTeleport");
        Logger.Info($" Player Id: {player.PlayerId}", "RpcTeleport");
        if (player.inVent
            || player.MyPhysics.Animations.IsPlayingEnterVentAnimation())
        {
            Logger.Info($"Target: ({GetPlayer.GetNameRole(player)}) in vent", "RpcTeleport");
            player.MyPhysics.RpcBootFromVent(0);
        }
        if (player.onLadder
            || player.MyPhysics.Animations.IsPlayingAnyLadderAnimation())
        {
            Logger.Warn($"Teleporting canceled - Target: ({GetPlayer.GetNameRole(player)}) is in on Ladder", "RpcTeleport");
            return;
        }
        var net = player.NetTransform;
        var numHost = (ushort)(net.lastSequenceId + 2);
        var numClient = (ushort)(net.lastSequenceId + 48);

        // Host side
        if (AmongUsClient.Instance.AmHost)
        {
            var playerlastSequenceId = (int)player.NetTransform.lastSequenceId;
            playerlastSequenceId += 10;
            player.NetTransform.SnapTo(location, (ushort)playerlastSequenceId);
            player.NetTransform.SnapTo(location, numHost);
        }
        else
        {
            // Local Teleport For Client
            MessageWriter localMessageWriter = AmongUsClient.Instance.StartRpcImmediately(net.NetId, (byte)RpcCalls.SnapTo, SendOption.None, player.GetClientId());
            NetHelpers.WriteVector2(location, localMessageWriter);
            localMessageWriter.Write(numClient);
            AmongUsClient.Instance.FinishRpcImmediately(localMessageWriter);
        }

        // For Client side
        MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(player.NetTransform.NetId, (byte)RpcCalls.SnapTo, SendOption.None);
        NetHelpers.WriteVector2(location, messageWriter);
        messageWriter.Write(player.NetTransform.lastSequenceId + 100U);
        AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
        // Global Teleport
        MessageWriter globalMessageWriter = AmongUsClient.Instance.StartRpcImmediately(net.NetId, (byte)RpcCalls.SnapTo, SendOption.None);
        NetHelpers.WriteVector2(location, globalMessageWriter);
        globalMessageWriter.Write(numClient);
        AmongUsClient.Instance.FinishRpcImmediately(globalMessageWriter);

        if (PlayerControl.LocalPlayer == player)
            LocalPlayerLastTp = location;
    }
    public static void SendMessageAsPlayerImmediately(PlayerControl player, string text, bool hostCanSee = true, bool sendToModded = true)
    {
        if (hostCanSee) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(player, text);
        if (!sendToModded) text += "\0";

        var writer = CustomRpcSender.Create("MessagesToSend", SendOption.None);
        writer.StartMessage(-1);
        writer.StartRpc(player.NetId, (byte)RpcCalls.SendChat)
            .Write(text)
            .EndRpc();
        writer.EndMessage();
        writer.SendMessage();
    }
    //public static string ColorString(Color32 color, string str) => $"<color=#{color.r:x2}{color.g:x2}{color.b:x2}{color.a:x2}>{str}</color>";
    public static string RemoveHtmlTags(this string str) => Regex.Replace(str, "<[^>]*?>", string.Empty);
    public static void KickPlayer(int playerId, bool ban, string reason)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetKickReason, SendOption.Reliable, -1);
        writer.Write(GetString($"DCNotify.{reason}"));
        AmongUsClient.Instance.FinishRpcImmediately(writer);
        _ = new LateTask(() =>
        {
            AmongUsClient.Instance.KickPlayer(playerId, ban);
        }, Math.Max(AmongUsClient.Instance.Ping / 500f, 1f), "Kick Player");
    }
    
    public static string getColoredPingText(int ping){

        if (ping <= 100){ // Green for ping < 100

            return $"<color=#00ff00ff>{ping}";//</color>";

        } else if (ping < 400){ // Yellow for 100 < ping < 400

            return $"<color=#ffff00ff>{ping}";//</color>";

        } else{ // Red for ping > 400

            return $"<color=#ff0000ff>{ping}";//</color>";
        }
    }
    public static string ColorString(Color32 color, string str) => $"<color=#{color.r:x2}{color.g:x2}{color.b:x2}{color.a:x2}>{str}</color>";

    public static string getColoredFPSText(float fps)
    {
        string a = "";
        if (fps >= 100){ // Green for fps > 100

            return a + $"<color=#00ff00ff>{fps}";//</color>";

        } else if (fps < 100 & fps > 50){ // Yellow for 100 > fps > 50

            return a + $"<color=#ffff00ff>{fps}";//</color>";

        } else{ // Red for fps < 50

            return a + $"<color=#ff0000ff>{fps}";//</color>";
        }
    }
    public static KeyCode stringToKeycode(string keyCodeStr){

        if(!string.IsNullOrEmpty(keyCodeStr)){ // Empty strings are automatically invalid

            try{
                
                // Case-insensitive parse of UnityEngine.KeyCode to check if string is validssss
                KeyCode keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyCodeStr, true);
                
                return keyCode;

            }catch{}
        
        }

        return KeyCode.Delete; // If string is invalid, return Delete as the default key
    }
}