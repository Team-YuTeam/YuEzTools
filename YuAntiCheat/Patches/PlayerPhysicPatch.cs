using HarmonyLib;
using UnityEngine;
using YuAntiCheat.Get;

namespace YuAntiCheat;

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
        
        //打开游戏目录
        if (Toggles.OpenGameDic)
        {
            FunctionPatch.OpenGameDic();
            Toggles.OpenGameDic = !Toggles.OpenGameDic;
        }
        
        //-- 下面是主机专用的按钮--//
        
        //立即开始
        if (Toggles.ChangeDownTimerToZero && GetPlayer.IsCountDown)
        {
            FunctionPatch.ChangeDownTimerTo(0);
            Toggles.ChangeDownTimerToZero = !Toggles.ChangeDownTimerToZero;
        }
        else if(Toggles.ChangeDownTimerToZero) Toggles.ChangeDownTimerToZero = !Toggles.ChangeDownTimerToZero;
        
        //恶搞倒计时
        if (Toggles.ChangeDownTimerTo114514 && GetPlayer.IsCountDown)
        {
            FunctionPatch.ChangeDownTimerTo(114514);
            Toggles.ChangeDownTimerTo114514 = !Toggles.ChangeDownTimerTo114514;
        }
        else if(Toggles.ChangeDownTimerTo114514) Toggles.ChangeDownTimerTo114514 = !Toggles.ChangeDownTimerTo114514;
        
        //倒计时取消
        if (Toggles.AbolishDownTimer && GetPlayer.IsCountDown)
        {
            FunctionPatch.AbolishDownTimer();
            Toggles.AbolishDownTimer = !Toggles.AbolishDownTimer;
        }
        else if(Toggles.AbolishDownTimer) Toggles.AbolishDownTimer = !Toggles.AbolishDownTimer;

    }
}