using HarmonyLib;
using YuAntiCheat.Get;
using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static YuAntiCheat.Translator;

namespace YuAntiCheat.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.UpdateSystem), typeof(SystemTypes), typeof(PlayerControl), typeof(MessageReader))]
public static class ShipStatus_FixedUpdate
{
//     public static void Postfix(ShipStatus __instance)
//     {
//         // 日志文件传输
//         if (Toggles.DumpLog)
//         {
//             FunctionPatch.DumpLogKey();
//             Toggles.DumpLog = !Toggles.DumpLog;
//         }
//         
//         //打开游戏目录
//         if (Toggles.OpenGameDic)
//         {
//             FunctionPatch.OpenGameDic();
//             Toggles.OpenGameDic = !Toggles.OpenGameDic;
//         }
//         
//         //退出游戏
//         if (Toggles.ExitGame)
//         {
//             FunctionPatch.ExitGame();
//             Toggles.ExitGame = !Toggles.ExitGame;
//         }
//         
//         //-- 下面是主机专用的按钮--//
//         
//         //立即开始
//         if (Toggles.ChangeDownTimerToZero && GetPlayer.IsCountDown)
//         {
//             FunctionPatch.ChangeDownTimerTo(0);
//             Toggles.ChangeDownTimerToZero = !Toggles.ChangeDownTimerToZero;
//         }
//         else if(Toggles.ChangeDownTimerToZero) Toggles.ChangeDownTimerToZero = !Toggles.ChangeDownTimerToZero;
//         
//         //恶搞倒计时
//         if (Toggles.ChangeDownTimerTo114514 && GetPlayer.IsCountDown)
//         {
//             FunctionPatch.ChangeDownTimerTo(114514);
//             Toggles.ChangeDownTimerTo114514 = !Toggles.ChangeDownTimerTo114514;
//         }
//         else if(Toggles.ChangeDownTimerTo114514) Toggles.ChangeDownTimerTo114514 = !Toggles.ChangeDownTimerTo114514;
//         
//         //倒计时取消
//         if (Toggles.AbolishDownTimer && GetPlayer.IsCountDown)
//         {
//             FunctionPatch.AbolishDownTimer();
//             Toggles.AbolishDownTimer = !Toggles.AbolishDownTimer;
//         }
//         else if(Toggles.AbolishDownTimer) Toggles.AbolishDownTimer = !Toggles.AbolishDownTimer;
//     }
    public static bool Prefix(ShipStatus player, [HarmonyArgument(0)] SystemTypes systemType, [HarmonyArgument(1)] PlayerControl __instance, [HarmonyArgument(2)] MessageReader reader)
    {
        if (systemType is 
            SystemTypes.Ventilation
            or SystemTypes.Security
            or SystemTypes.Decontamination
            or SystemTypes.Decontamination2
            or SystemTypes.Decontamination3) return true;

        var amount = MessageReader.Get(reader).ReadByte();
        if (AntiCheatForAll.RpcUpdateSystemCheck(__instance, systemType, amount)  || GetPlayer.IsHideNSeek)
        {
            Logger.Info("AC 破坏 RPC", "MessageReaderUpdateSystemPatch");
            Main.Logger.LogInfo("Hacker " + __instance.GetRealName() + $"{"好友编号："+__instance.GetClient().FriendCode+"/名字："+__instance.GetRealName()+"/实验性ProductUserId获取："+__instance.GetClient().ProductUserId}");
            //Main.PlayerStates[__instance.GetClient().Id].IsHacker = true;
            SendChat.Prefix(__instance);
            if(!Toggles.SafeMode && !AmongUsClient.Instance.AmHost)
            {
                Main.Logger.LogInfo("Try Murder" + __instance.GetRealName());
                //__instance.RpcSendChat($"{Main.ModName}检测到我是外挂 并且正在尝试踢出我 [来自{AmongUsClient.Instance.PlayerPrefab.GetRealName()}的{Main.ModName}]");
                //Try_to_ban(__instance);
                MurderHacker.murderHacker(__instance,MurderResultFlags.Succeeded);
                return false;
            }
            //PlayerControl Host = AmongUsClient.Instance.GetHost();
            else if (AmongUsClient.Instance.AmHost)
            {
                Main.Logger.LogInfo("Host Try ban " + __instance.GetRealName());
                //__instance.RpcSendChat($"{Main.ModName}检测到我是外挂 并且正在尝试踢出我 [来自房主{AmongUsClient.Instance.PlayerPrefab.GetRealName()}的{Main.ModName}]");
                if (!Toggles.SafeMode)
                {
                    Main.Logger.LogInfo("Host Try murder " + __instance.GetRealName());
                    MurderHacker.murderHacker(__instance,MurderResultFlags.Succeeded);
                }
                if(!GetPlayer.IsLobby)
                {
                    GameManager.Instance.LogicFlow.CheckEndCriteria();
                    GameManager.Instance.RpcEndGame(GameOverReason.ImpostorDisconnect, false);
                    GameManager.Instance.EndGame();
                }
                AmongUsClient.Instance.KickPlayer(__instance.GetClientId(), true);
                return false;
            }
            return false;
        }

        return RepairSystemPatch.Prefix(player, systemType, __instance, amount);
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
            "SystemType: " + systemType.ToString() + ", PlayerName: " + __instance.GetRealName() + "(" + __instance.Data.RoleType + ")" +
            ", amount: " + amount, "RepairSystem");
        return true;
    }
}
