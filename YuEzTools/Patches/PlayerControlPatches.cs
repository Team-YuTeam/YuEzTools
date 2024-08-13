using HarmonyLib;
using TMPro;
using YuEzTools.Get;
using YuEzTools.Modules;
using static YuEzTools.Translator;
using YuEzTools.Get;

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
        }

        if (GetPlayer.IsInGame)
        {
            color = Utils.Utils.GetRoleHtmlColor(__instance.Data.RoleType);
            if(__instance == PlayerControl.LocalPlayer || (PlayerControl.LocalPlayer.Data.IsDead && __instance.Data.IsDead))
                name = Utils.Utils.ColorString(Utils.Utils.GetRoleColor32(__instance.Data.RoleType), __instance.GetRoleName()  + "\n" + name);
            if (PlayerControl.LocalPlayer.Data.IsDead && __instance.Data.IsDead)
                name += $"({Utils.Utils.GetDeadText(__instance)})";
        }

        if (Main.HackerList.Contains(__instance))
        {
            name += $"<color=#3FBAFF>[{GetString("Hacker")}]</color>";
        }
        
        __instance.cosmetics.nameText.text = name;
        __instance.cosmetics.nameText.alignment = TextAlignmentOptions.Center;
    }
}
