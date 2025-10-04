using Hazel;
using HarmonyLib;
using YuEzTools.Get;
using YuEzTools.Modules;
using YuEzTools.Patches;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
internal class RPCHandlerPatch
{
    public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] byte callId,
        [HarmonyArgument(1)] MessageReader reader)
    {
        Main.Logger.LogMessage("From " + __instance.GetRealName() + "'s RPC:" + callId);
        if (!Toggles.EnableAntiCheat) return true;
        try
        {
            if (AntiCheatForAll.ReceiveRpc(__instance, callId, reader) || AUMCheat.ReceiveInvalidRpc(__instance, callId, reader) ||
                SMCheat.ReceiveInvalidRpc(__instance, callId))
            {
                if (Toggles.AutoStartGame && AmongUsClient.Instance.AmHost &&
                    __instance.GetPlayerData().PUID == AmongUsClient.Instance
                        .GetClient(AmongUsClient.Instance.ClientId).ProductUserId
                    && IntroCutscenePatch.Ifkill && (callId == (int)RpcCalls.MurderPlayer || callId == (int)RpcCalls.CheckMurder))
                {
                    IntroCutscenePatch.Ifkill = false;
                    Main.Logger.LogInfo("Just host automode, normal");
                    return true;
                }
                if (!Main.HackerList.Contains(__instance)) Main.HackerList.Add(__instance);
                Main.HasHacker = true;
                Logger.Fatal("Hacker " + __instance.GetRealName() + $"{"好友编号：" + __instance.GetClient().FriendCode + "/名字：" + __instance.GetRealName() + "/ProductUserId：" + __instance.GetClient().ProductUserId}", "RPCHandle");
                //Main.PlayerStates[__instance.GetClient().Id].IsHacker = true;
                SendChat.Prefix(__instance);
                if (!Toggles.SafeMode && !AmongUsClient.Instance.AmHost && GameStartManagerPatch.roomMode == RoomMode.Plus25)
                {
                    Main.Logger.LogInfo("Try Kick" + __instance.GetRealName());
                    KickHackerPatch.KickPlayer(__instance);

                    return false;
                }
                //PlayerControl Host = AmongUsClient.Instance.GetHost();
                else if (AmongUsClient.Instance.AmHost)
                {
                    Main.Logger.LogInfo("Host Try ban " + __instance.GetRealName());
                    AmongUsClient.Instance.KickPlayer(__instance.GetClientId(), true);
                    if (GetPlayer.IsInGame)
                    {
                        Main.Logger.LogInfo("Host Try end game with room " +
                                            GameStartManager.Instance.GameRoomNameCode.text);
                        try
                        {
                            GameManager.Instance.RpcEndGame(GameOverReason.ImpostorDisconnect, false);

                        }
                        catch (System.Exception e)
                        {
                            Logger.Error(e.ToString(), "Session");
                        }
                        Main.HasHacker = false;
                    }
                    return false;
                }
                return false;
            }
        }
        catch
        {
            // SendInGamePatch.SendInGame(Translator.GetString("ERROR.CHECKRPC"));
            // Logger.Warn("[Info or ERROR]可能是模组问题或者正常的RPC","RPC");
            return true;
        }
        return true;
    }
}
