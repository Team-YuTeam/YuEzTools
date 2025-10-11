using YuEzTools.Helpers;
using YuEzTools.Modules;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(IntroCutscene))]
public class IntroPatch
{
    [HarmonyPatch(nameof(IntroCutscene.ShowRole)), HarmonyPostfix]
    public static void ShowRole_Postfix(IntroCutscene __instance)
    {
        _ = new LateTask(() =>
        {
            var roleType = PlayerControl.LocalPlayer.Data.Role.Role;
            __instance.YouAreText.color = RoleColorHelper.GetRoleColor32(roleType);
            //__instance.RoleText.text = Utils.Utils.GetRoleName(roleType);
            __instance.RoleText.color = RoleColorHelper.GetRoleColor32(roleType);
            __instance.RoleText.fontWeight = TMPro.FontWeight.Thin;
            __instance.RoleText.SetOutlineColor(Utils.Utils.ShadeColor(RoleColorHelper.GetRoleColor32(roleType), 0.1f).SetAlpha(0.38f));
            __instance.RoleText.SetOutlineThickness(0.17f);
            __instance.RoleBlurbText.color = RoleColorHelper.GetRoleColor32(roleType);
            __instance.RoleBlurbText.text = roleType.GetRoleInfoForVanilla();

        }, 0.0001f, "Override Role Text");
        return;
    }

    [HarmonyPatch(nameof(IntroCutscene.BeginCrewmate)), HarmonyPostfix]
    public static void BeginCrewmate_Postfix(IntroCutscene __instance,
        ref Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay)
    {
        __instance.TeamTitle.text = $"{GetString("TeamCrewmate")}";
        __instance.ImpostorText.text = $"{string.Format(GetString("ImpostorNumCrew"), GameOptionsManager.Instance.currentNormalGameOptions.NumImpostors)}";
        __instance.ImpostorText.text += "\n" + string.Format(GetString("CrewmateIntroText"),RoleColorHelper.GetRoleColorHex(PlayerControl.LocalPlayer.Data.RoleType));
        __instance.TeamTitle.color = RoleColorHelper.GetRoleColor32(PlayerControl.LocalPlayer.Data.RoleType);
    }
    [HarmonyPatch(nameof(IntroCutscene.BeginImpostor)), HarmonyPostfix]
    public static void BeginImpostor_Postfix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
    {
        __instance.ImpostorText.gameObject.SetActive(true);

        __instance.TeamTitle.text = GetString("TeamImpostor");
        __instance.ImpostorText.text = $"{string.Format(GetString("ImpostorNumImp"), GameOptionsManager.Instance.currentNormalGameOptions.NumImpostors)}";

        __instance.ImpostorText.text += "\n" + string.Format(GetString("ImpostorIntroText"),RoleColorHelper.GetRoleColorHex(PlayerControl.LocalPlayer.Data.RoleType));
        __instance.TeamTitle.color = __instance.BackgroundBar.material.color = RoleColorHelper.GetRoleColor32(PlayerControl.LocalPlayer.Data.RoleType);
    }
}