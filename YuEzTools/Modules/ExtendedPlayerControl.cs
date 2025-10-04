using AmongUs.GameOptions;
using static YuEzTools.Translator;


namespace YuEzTools.Modules;

static class ExtendedPlayerControl
{
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