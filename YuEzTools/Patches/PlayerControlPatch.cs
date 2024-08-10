using HarmonyLib;
using System.Text;
using UnityEngine;
using static YuEzTools.Translator;
using AmongUs.GameOptions;
using YuEzTools.Get;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace YuEzTools;


[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.BootFromVent))]
class BootFromVentPatch
{

    public static bool Prefix()
    {
        if (GetPlayer.IsLobby)
        {
            SendInGamePatch.SendInGame(GetString("Warning.RoomBroken"));
            return false;
        }
        return true;
    }
}
[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
class MurderPlayerPatch
{

    public static bool Prefix()
    {
        if (GetPlayer.IsLobby)
        {
            SendInGamePatch.SendInGame(GetString("Warning.RoomBroken"));
            return false;
        }
        return true;
    }
    public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
    {
        if (target.GetPlayerData().RealKiller != null) return;
        target.SetDeathReason(DataDeathReason.Kill);
        target.SetRealKiller(__instance);
        target.SetDead();
    }
}
[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
class DiePatch
{

    public static bool Prefix()
    {
        if (GetPlayer.IsLobby)
        {
            SendInGamePatch.SendInGame(GetString("Warning.RoomBroken"));
            return false;
        }
        return true;
    }
    public static void Postfix(PlayerControl __instance, [HarmonyArgument(1)] bool assginGhostRole)
    {
        if (!assginGhostRole) return;
        __instance.SetDeathReason(DataDeathReason.Kill);
        __instance.SetDead();
    }
}
[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CoSetRole))]
class CoSetRolePatch
{
    public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] RoleTypes roleTypes)
    {
        __instance.SetRole(roleTypes);
        __instance.SetIsImp(Utils.Utils.IsImpostor(roleTypes));
    }
}
[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
class FixedUpdatePatch
{
    public static void Postfix(PlayerControl __instance)
    {
        if (__instance == null) return;

        
        var self = __instance == PlayerControl.LocalPlayer;
        var color ="#ffffff";
        var nametext = __instance.GetRealName();

        if (GetPlayer.IsLobby)
        {
            // 到时候可以做外挂判定
        }
        else if (GetPlayer.IsInGame)
        {
            // 可以做职业颜色名称和死因等
            color = Utils.Utils.GetRoleHtmlColor(__instance.Data.RoleType);
        }

        if(__instance == PlayerControl.LocalPlayer) __instance.cosmetics.nameText.text = $"<color={color}>" + nametext + "</color>";
        else __instance.cosmetics.nameText.text = nametext;
        if (PlayerControl.LocalPlayer.Data.IsDead && __instance.Data.IsDead)
            __instance.cosmetics.nameText.text += Utils.Utils.GetVitalText(__instance.PlayerId);


    }
}
[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
class PlayerStartPatch
{
    public static void Postfix(PlayerControl __instance)
    {
        var roleText = UnityEngine.Object.Instantiate(__instance.cosmetics.nameText);
        roleText.transform.SetParent(__instance.cosmetics.nameText.transform);
        roleText.transform.localPosition = new Vector3(0f, 0.2f, 0f);
        roleText.transform.localScale = new(1f, 1f, 1f);
        roleText.fontSize = 2f;
        roleText.text = "RoleText";
        roleText.gameObject.name = "RoleText";
        roleText.enabled = false;
    }
}
[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetTasks))]
class PlayerControlSetTasksPatch
{
    public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo.TaskInfo> tasks)
    {
        var pc = __instance;
        pc.SetTaskTotalCount(tasks.Count);
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
class PlayerControlCompleteTaskPatch
{

    public static void Postfix(PlayerControl __instance)
    {
        var pc = __instance;
        Logger.Info($"TaskComplete:{pc.GetNameWithRole()}", "CompleteTask");
        pc.OnCompleteTask();

        GameData.Instance.RecomputeTaskCounts();
        Logger.Info($"TotalTaskCounts = {GameData.Instance.CompletedTasks}/{GameData.Instance.TotalTasks}", "TaskState.Update");
    }
}