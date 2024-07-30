using HarmonyLib;
using YuEzTools.Get;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(Constants), nameof(Constants.GetBroadcastVersion))]
class ServerUpdatePatch
{
    static void Postfix(ref int __result)
    {
        if (GetPlayer.IsLocalGame)
        {
            Logger.Info($"IsLocalGame: {__result}", "VersionServer");
        }
        if (GetPlayer.IsOnlineGame)
        {
            // Changing server version for AU mods
            if (Toggles.ServerAllHostOrNoHost)
                __result += 25;
            Logger.Info($"IsOnlineGame: {__result}", "VersionServer");
        }
    }
}

[HarmonyPatch(typeof(Constants), nameof(Constants.IsVersionModded))]
public static class IsVersionModdedPatch
{
    public static bool Prefix(ref bool __result)
    {
        __result = Toggles.ServerAllHostOrNoHost;
        return false;
    }
}