using AmongUs.GameOptions;
using HarmonyLib;
using YuEzTools.Get;
using static YuEzTools.Translator;
using static YuEzTools.Logger;

namespace YuEzTools;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public class PlayerPhysicPatch
{
    public static void Postfix(PlayerPhysics __instance)
    {
        //日志文件转储
        if (Toggles.DumpLog)
        {
            FunctionPatch.DumpLogKey();
            Toggles.DumpLog = !Toggles.DumpLog;
        }

        //重展职业
        if (Toggles.reShowRoleT && GetPlayer.IsInGame)
        {
            FunctionPatch.ShowRoleM();
            Toggles.reShowRoleT = !Toggles.reShowRoleT;
        }
        else if (Toggles.reShowRoleT && !GetPlayer.IsInGame) Toggles.reShowRoleT = !Toggles.reShowRoleT;

        //打开游戏目录
        if (Toggles.OpenGameDic)
        {
            FunctionPatch.OpenGameDic();
            Toggles.OpenGameDic = !Toggles.OpenGameDic;
        }

        //退出游戏
        if (Toggles.ExitGame)
        {
            FunctionPatch.ExitGame();
            Toggles.ExitGame = !Toggles.ExitGame;
        }
        //真ban
        if (Toggles.RealBan)
        {
            FunctionPatch.RealBan();
            Toggles.RealBan = !Toggles.RealBan;
        }



        //-- 下面是主机专用的按钮--//

        //立即开始
        if (Toggles.ChangeDownTimerToZero && GetPlayer.IsCountDown)
        {
            FunctionPatch.ChangeDownTimerTo(0);
            Toggles.ChangeDownTimerToZero = !Toggles.ChangeDownTimerToZero;
        }
        else if (Toggles.ChangeDownTimerToZero) Toggles.ChangeDownTimerToZero = !Toggles.ChangeDownTimerToZero;

        //恶搞倒计时
        if (Toggles.ChangeDownTimerTo114514 && GetPlayer.IsCountDown)
        {
            FunctionPatch.ChangeDownTimerTo(114514);
            Toggles.ChangeDownTimerTo114514 = !Toggles.ChangeDownTimerTo114514;
        }
        else if (Toggles.ChangeDownTimerTo114514) Toggles.ChangeDownTimerTo114514 = !Toggles.ChangeDownTimerTo114514;

        //倒计时取消
        if (Toggles.AbolishDownTimer && GetPlayer.IsCountDown)
        {
            FunctionPatch.AbolishDownTimer();
            Toggles.AbolishDownTimer = !Toggles.AbolishDownTimer;
        }
        else if (Toggles.AbolishDownTimer) Toggles.AbolishDownTimer = !Toggles.AbolishDownTimer;

    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public class ImpNumCheckPatch
{
    public static void Postfix(PlayerPhysics __instance)
    {
        if (GetPlayer.GetImpNums > 3)
        {
            SendInGamePatch.SendInGame(GetString("OptImpMoreThanThree"));
            Error("最大内鬼数比3还大呢！" + AmongUsClient.Instance.GetHost().Character.GetRealName() + "是房主！" + $"{NormalGameOptionsV10.MaxImpostors.Count}个内鬼", "ImpNumCheckPatch");
        }

        if (GetPlayer.numImpostors > GetPlayer.GetImpNums || GetPlayer.numImpostors > 3)
        {
            SendInGamePatch.SendInGame(GetString("NowImpMoreThan"));
            Error("最大内鬼数比预设/3还大呢！" + AmongUsClient.Instance.GetHost().Character.GetRealName() + "是房主！" + $"{NormalGameOptionsV10.MaxImpostors.Count}个内鬼", "ImpNumCheckPatch");
        }

        if (GetPlayer.GetImpNums > 1 && GetPlayer.isHideNSeek)
        {
            SendInGamePatch.SendInGame(GetString("OptHImpMoreThanThree"));
            Error("最大内鬼数比1还大呢！" + AmongUsClient.Instance.GetHost().Character.GetRealName() + "是房主！" + $"{NormalGameOptionsV10.MaxImpostors.Count}个内鬼", "ImpNumCheckPatch");
        }

        if (GetPlayer.numImpostors > 1 && GetPlayer.isHideNSeek)
        {
            SendInGamePatch.SendInGame(GetString("NowHImpMoreThan"));
            Error("最大内鬼数比1还大呢！" + AmongUsClient.Instance.GetHost().Character.GetRealName() + "是房主！" + $"{NormalGameOptionsV10.MaxImpostors.Count}个内鬼", "ImpNumCheckPatch");
        }
    }
}