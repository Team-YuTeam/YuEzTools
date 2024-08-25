using System.Collections.Generic;
using HarmonyLib;
using InnerNet;
using static YuEzTools.Translator;

namespace YuEzTools;

[HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.KickPlayer))]
internal class KickPlayerPatch
{
    public static Dictionary<string, int> AttemptedKickPlayerList = [];

    public static bool Prefix(InnerNetClient __instance, int clientId, bool ban)
    {
        if (!AmongUsClient.Instance.AmHost) return true;
        if (AmongUsClient.Instance.ClientId == clientId && !ban) 
        {
            SendInGamePatch.SendInGame(string.Format(GetString("KickHostByAUSystem"), ban ? GetString("BanText") : GetString("KickText")));
            Logger.Info("我靠 房主居然能封禁/踢自己！", "KickPlayerPatch");
            return false;
        }
        SendInGamePatch.SendInGame(string.Format(GetString("Message.AddedPlayerToBanList"), $"{AmongUsClient.Instance.GetRecentClient(clientId).PlayerName}"));
        if (ban) Utils.Utils.AddHacker(AmongUsClient.Instance.GetRecentClient(clientId));

        return true;
    }
}