using AmongUs.Data;
using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static YuEzTools.Translator;


namespace YuEzTools.Modules;

static class ExtendedPlayerControl
{
    public static void SetRole(this PlayerControl player, RoleTypes role, bool canOverride = false)
    {
        player.StartCoroutine(player.CoSetRole(role, canOverride));
    }
    public static void RpcSetRoleV2(this PlayerControl player, RoleTypes role)
    {
        if (player == null) return;
        // Main.StandardRoles[player.PlayerId] = role;
        player.StartCoroutine(player.CoSetRole(role, true));
        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(player.NetId, (byte)RpcCalls.SetRole, SendOption.Reliable, -1);
        writer.Write((ushort)role);
        writer.Write(true);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
        // foreach (var pc in PlayerControl.AllPlayerControls)
        // {
        //     // if (Main.DesyncRoles.ContainsKey((player.PlayerId, pc.PlayerId)))
        //     //     Main.DesyncRoles.Remove((player.PlayerId, pc.PlayerId));
        // }
        // AntiCheat.TimeSinceRoleChange[player.PlayerId] = 0f;
    }

    public static void RpcSetRoleV3(this PlayerControl player, RoleTypes role, bool forEndGame)
    {
        if (player == null) return;
        if (forEndGame)
            RoleManager.Instance.SetRole(player, role);
        else
            player.StartCoroutine(player.CoSetRole(role, true));
        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(player.NetId, (byte)RpcCalls.SetRole, SendOption.Reliable, -1);
        writer.Write((ushort)role);
        writer.Write(true);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
        player.RpcSetRole(role,true);
    }
    public static string GetRoleInfoForVanilla(this RoleTypes role)
    {

        var text = role.ToString();

        var Info = "Short";

        return GetString($"{text}{Info}");
    }
    public static string GetRoleLInfoForVanilla(this RoleTypes role)
    {
        var text = role.ToString();

        var Info = "Long";

        return GetString($"{text}{Info}");
    }
}