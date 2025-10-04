using System.Data;
using UnityEngine;
using YuEzTools.Patches;
using YuEzTools.UI;
using YuEzTools.Utils;

namespace YuEzTools.Keys;

[HarmonyPatch(typeof(ControllerManager), nameof(ControllerManager.Update))]
internal class Keys
{
    public static void Postfix(ControllerManager __instance)
    {
        //日志文件转储
        if (GetKeysDown(KeyCode.F1))
        {
            FunctionPatch.DumpLogKey();
        }

        //重来职业展示
        if (GetKeysDown(KeyCode.F2) && GetPlayer.IsInGame)
        {
            FunctionPatch.ShowRoleM();
        }

        // 显示职业介绍&预设
        if (GetPlayer.IsInGame && (GetPlayer.IsCanMove || GetPlayer.IsMeeting))
        {
            if (Input.GetKey(KeyCode.F3))
            {
                if (!InGameRoleInfoMenu.Showing)
                    InGameRoleInfoMenu.SetRoleInfoRef(PlayerControl.LocalPlayer);
                InGameRoleInfoMenu.Show();
            }
            else InGameRoleInfoMenu.Hide();
        }
        else InGameRoleInfoMenu.Hide();

        //开启非安全模式
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Toggles.SafeMode = !Toggles.SafeMode;
        }

        //打开游戏目录
        if (GetKeysDown(KeyCode.F10))
        {
            FunctionPatch.OpenGameDic();
        }

        //-- 下面是主机专用的命令--//
        if (!AmongUsClient.Instance.AmHost) return;

        //立即开始
        if (Input.GetKeyDown(KeyCode.LeftShift) && GetPlayer.IsCountDown)
        {
            FunctionPatch.ChangeDownTimerTo(0);
        }

        //倒计时取消
        if (Input.GetKeyDown(KeyCode.C) && GetPlayer.IsCountDown)
        {
            FunctionPatch.AbolishDownTimer();
        }
    }

    private static bool GetKeysDown(params KeyCode[] keys)
    {
        if (keys.Any(k => Input.GetKeyDown(k)) && keys.All(k => Input.GetKey(k)))
        {
            Logger.Info($"快捷键：{keys.Where(k => Input.GetKeyDown(k)).First()} in [{string.Join(",", keys)}]", "Keys");
            return true;
        }
        return false;
    }
}