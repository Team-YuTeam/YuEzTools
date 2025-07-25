using HarmonyLib;
using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuEzTools.Get;
using YuEzTools.Modules;
using static YuEzTools.Translator;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.UpdateSystem), typeof(SystemTypes), typeof(PlayerControl), typeof(MessageReader))]
public static class ShipStatus_FixedUpdate
{
    public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] SystemTypes systemType, [HarmonyArgument(1)] PlayerControl player, [HarmonyArgument(2)] MessageReader reader)
    {
        if (!Toggles.EnableAntiCheat) return true;
        var amount = MessageReader.Get(reader).ReadByte();
        if (AntiCheatForAll.RpcUpdateSystemCheck(player, systemType, amount)  || (GetPlayer.IsHideNSeek && AntiCheatForAll.RpcUpdateSystemCheckFHS(player, systemType, amount)))
        {
            if(!Main.HackerList.Contains(player)) Main.HackerList.Add(player);
            Logger.Info("AC 破坏 RPC", "MessageReaderUpdateSystemPatch");
            Main.Logger.LogInfo("Hacker " + player.GetRealName() + $"{"好友编号："+player.GetClient().FriendCode+"/名字："+player.GetRealName()+"/ProductUserId："+player.GetClient().ProductUserId}");
            //Main.PlayerStates[__instance.GetClient().Id].IsHacker = true;
            SendChat.Prefix(player); 
            if (AmongUsClient.Instance.AmHost)
            {
                Main.Logger.LogInfo("Host Try ban " + player.GetRealName());
                // __instance.RpcSendChat($"{Main.ModName}检测到我是外挂 并且正在尝试踢出我 [来自房主{AmongUsClient.Instance.PlayerPrefab.GetRealName()}的{Main.ModName}]");
                AmongUsClient.Instance.KickPlayer(player.GetClientId(), true);
                 GameManager.Instance.RpcEndGame(GameOverReason.ImpostorDisconnect, false);
                if(GetPlayer.IsInGame)
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
        __instance.RpcUpdateSystem(systemType, 16);
        return RepairSystemPatch.Prefix(__instance, systemType, player, amount);
    }
}

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.UpdateSystem), typeof(SystemTypes), typeof(PlayerControl),
    typeof(byte))]
class RepairSystemPatch
{
    public static bool Prefix(ShipStatus player,
        [HarmonyArgument(0)] SystemTypes systemType,
        [HarmonyArgument(1)] PlayerControl __instance,
        [HarmonyArgument(2)] byte amount)
    {
        Logger.Msg(
            "SystemType: " + systemType.ToString() + ", PlayerName: " + __instance.GetRealName() +
            ", amount: " + amount, "RepairSystem");
        return true;
    }
}
