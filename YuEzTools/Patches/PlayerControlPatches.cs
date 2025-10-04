using AmongUs.GameOptions;
using TMPro;
using YuEzTools.Modules;
using YuEzTools.UI;
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
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
class FixedUpdatePatch
{
    public static void Postfix(PlayerControl __instance)
    {
        if (__instance == null) return;

        var color = "#ffffff";
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
            if (__instance == PlayerControl.LocalPlayer || (PlayerControl.LocalPlayer.Data.IsDead && __instance.Data.IsDead))
                name = Utils.Utils.ColorString(Utils.Utils.GetRoleColor32(__instance.Data.RoleType), __instance.GetRoleName() + "\n" + name);
            if (PlayerControl.LocalPlayer.Data.IsDead && __instance.Data.IsDead)
                name += $"({Utils.Utils.GetDeadText(__instance)})";
            if (PlayerControl.LocalPlayer.Data.IsDead && __instance.Data.RoleType == RoleTypes.Impostor)
                name += $"({Utils.Utils.GetDeadText(__instance)})";
        }

        if (Main.HackerList.Contains(__instance))
        {
            name += $"<color=#3FBAFF>[{GetString("Hacker")}]</color>";
        }

        __instance.cosmetics.nameText.text = name + "\n";
        __instance.cosmetics.nameText.alignment = TextAlignmentOptions.Top;
    }
}
