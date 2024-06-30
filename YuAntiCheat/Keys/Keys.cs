using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using AmongUs.Data;
using AmongUs.GameOptions;
using Hazel;
using Il2CppInterop.Runtime.InteropTypes;
using InnerNet;
using YuAntiCheat.Get;
using YuAntiCheat;
using YuAntiCheat.UI;

namespace YuAntiCheat.Keys;

[HarmonyPatch(typeof(ControllerManager), nameof(ControllerManager.Update))]
internal class Keys
{
    public static void Postfix(ControllerManager __instance)
    {
        //日志文件转储
        if (GetKeysDown(KeyCode.F1))
        {
            YuACKeysOnMenu.DumpLogKey();
        }
        
        
        //开启非安全模式
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Toggles.SafeMode = !Toggles.SafeMode;
        }
        
        //打开游戏目录
        if (GetKeysDown(KeyCode.F10))
        {
            YuACKeysOnMenu.OpenGameDic();
        }
        
        //-- 下面是主机专用的命令--//
        if (!AmongUsClient.Instance.AmHost) return;
        //立即开始
        if (Input.GetKeyDown(KeyCode.LeftShift) && GetPlayer.IsCountDown)
        {
            YuACKeysOnMenu.ChangeDownTimerTo(0);
        }
        
        //倒计时取消
        if (Input.GetKeyDown(KeyCode.C) && GetPlayer.IsCountDown)
        {
            YuACKeysOnMenu.AbolishDownTimer();
        }
    }
    private static bool GetKeysDown(params KeyCode[] keys)
    {
        if (keys.Any(k => Input.GetKeyDown(k)) && keys.All(k => Input.GetKey(k)))
        {
            Main.Logger.LogInfo($"快捷键：{keys.Where(k => Input.GetKeyDown(k)).First()} in [{string.Join(",", keys)}]");
            return true;
        }
        return false;
    }
}