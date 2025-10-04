using YuEzTools.Patches;
using YuEzTools.UI;
using YuEzTools.Utils;

namespace YuEzTools.Send;

public class SendChat
{
    public static void Prefix(PlayerControl __instance)
    {
        if (Toggles.SafeMode && !AmongUsClient.Instance.AmHost)
        {
            SendInGamePatch.SendInGame(string.Format(GetString("AmnotHostSafeSeeHacker"), __instance.GetRealName()));
            Main.Logger.LogInfo($"已揭示 {__instance.GetRealName()}");
            return;
        }
        else if (!Toggles.SafeMode && !AmongUsClient.Instance.AmHost)
        {
            SendInGamePatch.SendInGame(string.Format(GetString("AmnotHostUnSafeSeeHacker"), __instance.GetRealName()));
            Main.Logger.LogInfo($"已尝试封禁 {__instance.GetRealName()}");
            return;
        }
        else if (AmongUsClient.Instance.AmHost)
        {
            SendInGamePatch.SendInGame(string.Format(GetString("AmHostSafeSeeHacker"), __instance.GetRealName()));
            Main.Logger.LogInfo($"已揭示 {__instance.GetRealName()}");
            return;
        }
        else
        {
            SendInGamePatch.SendInGame(string.Format(GetString("SeeHacker"), __instance.GetRealName()));
            Main.Logger.LogInfo($"已揭示 {__instance.GetRealName()}");
            return;
        }
    }
}