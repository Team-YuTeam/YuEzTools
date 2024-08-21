using HarmonyLib;
using TMPro;
using YuEzTools.Get;
using YuEzTools.Modules;
using static YuEzTools.Translator;
using YuEzTools.Get;
using YuEzTools.Utils;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
class MurderPlayerPatch
{
    public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
    {
        if (target.GetPlayerData().Killer != null) return;
        target.GetPlayerData().SetDeadReason(DeadReasonData.Kill);
        target.GetPlayerData().SetKiller(__instance);
        target.GetPlayerData().SetDead();
        __instance.AddKillCount();
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
class FixedUpdatePatch
{
    public static void Postfix(PlayerControl __instance)
    {
        if (__instance == null) return;

        var color ="#ffffff";
        var name = __instance.GetRealName();

        if (GetPlayer.IsLobby)
        {
            if (__instance.FriendCode.IsDevUser())
                name = __instance.FriendCode.GetDevUser().GetTag() + name;
               
            if (Toggles.ShowInfoInLobby)
            {
                name = $"<size=70%><color=#33EEFF>Lv.{__instance.GetClient().PlayerLevel} {__instance.GetClient().PlatformData.Platform.GetPlatformText()} {__instance.GetClient().Id}</color></size>\n" +
                        $"{name}\n" +
                        $"<size=65%><color=#33FF91>{__instance.PlayerId} {__instance.GetClient().FriendCode} {__instance.GetClient().GetHashedPuid()}</color></size>";
            }
        }

        if (GetPlayer.IsInGame)
        {
            color = Utils.Utils.GetRoleHtmlColor(__instance.Data.RoleType);
            if (__instance == PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.Data.IsDead)
            {
                name = Utils.Utils.ColorString(Utils.Utils.GetRoleColor32(__instance.Data.RoleType), __instance.GetRoleName()  + "\n" + name);
                name += "(" + __instance.PlayerId.GetKillOrTaskCountText() + ")";
            }
            if (PlayerControl.LocalPlayer.Data.IsDead && __instance.Data.IsDead)
                name += $"({Utils.Utils.GetDeadText(__instance)})";
        }

        if (Main.HackerList.Contains(__instance))
        {
            name += $"<color=#3FBAFF>[{GetString("Hacker")}]</color>";
        }
        
        __instance.cosmetics.nameText.text = name + "\n";
        __instance.cosmetics.nameText.alignment = TextAlignmentOptions.Center;
    }
}
[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
class CompleteTaskPatch
{
    public static void Postfix(PlayerControl __instance)
    {
        __instance.AddTaskCount();
    }
}
[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetTasks))]
class PlayerControlSetTasksPatch
{
    public static int TaskCount = 0;
    public static void Postfix([HarmonyArgument(0)] Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo.TaskInfo> Tasks)
    {
        TaskCount = Tasks.Count;
    }
}