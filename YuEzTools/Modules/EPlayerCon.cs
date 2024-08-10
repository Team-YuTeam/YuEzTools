using AmongUs.Data;
using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuEzTools.Modules;
using UnityEngine;
using YuEzTools;
using static YuEzTools.Translator;
using  YuEzTools.Utils;
using YuEzTools.Attributes;
using YuEzTools.Get;
using YuEzTools.Utils;


namespace YuEzTools;

static class ExtendedPlayerControl
{
    public const MurderResultFlags SucceededFlags = MurderResultFlags.Succeeded | MurderResultFlags.DecisionByHost;
    public static bool OwnedByHost(this InnerNetObject innerObject)
        => innerObject.OwnerId == AmongUsClient.Instance.HostId;
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
    public static ClientData GetClient(this PlayerControl player)
    {
        try
        {
            var client = AmongUsClient.Instance.allClients.ToArray().Where(cd => cd.Character.PlayerId == player.PlayerId).FirstOrDefault();
            return client;
        }
        catch
        {
            return null;
        }
    }
    public static int GetClientId(this PlayerControl player)
    {
        if (player == null) return -1;
        var client = player.GetClient();
        return client == null ? -1 : client.Id;
    }
    public static RoleTypes GetRoleType(this PlayerControl player)
    {
        if (player != null)
        {
            return  GetRoleType(player.PlayerId);
        }
        return RoleTypes.Crewmate;
    }
    public static RoleTypes GetRoleType(byte id)
    {
        return GamePlayerData.GetRoleById(id);
    }
    public static bool IsImpostor(this PlayerControl pc)
    {
        switch (pc.GetRoleType())
        {
            case RoleTypes.Impostor:
            case RoleTypes.Shapeshifter:
            case RoleTypes.Phantom:
            case RoleTypes.ImpostorGhost:
                return true;
        }
        return false;
    }
    public static bool IsImpostor(byte id) => IsImpostor(GetPlayer.GetPlayerById(id));

    /*public static GameOptionsData DeepCopy(this GameOptionsData opt)
    {
        var optByte = opt.ToBytes(5);
        return GameOptionsData.FromBytes(optByte);
    }*/

    public static string GetNameWithRole(this PlayerControl player, bool forUser = false)
    {
        var ret = $"{player?.Data?.PlayerName}" + (GetPlayer.IsInGame? $"({Utils.Utils.GetRoleName(player.GetRoleType())})" : "");
        return (forUser ? ret : ret.RemoveHtmlTags());
    }
    public static string GetRoleColorCode(this PlayerControl player)
    {
        return Utils.Utils.GetRoleHtmlColor(player.GetRoleType());
    }
    public static Color GetRoleColor(this PlayerControl player)
    {
        return Utils.Utils.GetRoleColor(player.GetRoleType());
    }
    
    public static string GetRealName(this PlayerControl player, bool isMeeting = false)
    {
        return isMeeting ? player?.Data?.PlayerName : player?.name;
    }
}