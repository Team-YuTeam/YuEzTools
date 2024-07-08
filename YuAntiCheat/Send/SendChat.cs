using YuAntiCheat.Get;

namespace YuAntiCheat;

public class SendChat
{
    public static void Prefix(PlayerControl __instance)
    {
        if (Toggles.SafeMode && !AmongUsClient.Instance.AmHost)
        {
            SendInGamePatch.SendInGame(string.Format(Translator.GetString("AmnotHostSafeSeeHacker"), __instance.GetRealName()));
            Main.Logger.LogInfo($"已揭示 {__instance.GetRealName()}");
            return;
        }
        else if(!Toggles.SafeMode && !AmongUsClient.Instance.AmHost)
        {
            SendInGamePatch.SendInGame(string.Format(Translator.GetString("AmnotHostUnSafeSeeHacker"),__instance.GetRealName()));
            Main.Logger.LogInfo($"已揭示 {__instance.GetRealName()}");
            return;
        }
        else if(!Toggles.SafeMode && AmongUsClient.Instance.AmHost)
        {
            SendInGamePatch.SendInGame(string.Format(Translator.GetString("AmHostUnSafeSeeHacker"),__instance.GetRealName()));
            Main.Logger.LogInfo($"已揭示 {__instance.GetRealName()}");
            return;
        }
        else if (AmongUsClient.Instance.AmHost)
        {
            SendInGamePatch.SendInGame(string.Format(Translator.GetString("AmHostSafeSeeHacker"),__instance.GetRealName()));
            Main.Logger.LogInfo($"已揭示 {__instance.GetRealName()}");
            return;
        }
        else
        {
            SendInGamePatch.SendInGame(string.Format(Translator.GetString("SeeHacker"),__instance.GetRealName()));
            Main.Logger.LogInfo($"已揭示 {__instance.GetRealName()}");
            return;
        }
    }
}