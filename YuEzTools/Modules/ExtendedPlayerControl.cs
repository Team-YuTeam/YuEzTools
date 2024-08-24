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