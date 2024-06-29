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
            Main.Logger.LogInfo("输出日志");
            DumpLog();
        }
        
        
        //开启非安全模式
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Toggles.SafeMode = !Toggles.SafeMode;
        }
        
        //打开游戏目录
        if (GetKeysDown(KeyCode.F10))
        {
            OpenDirectory(Environment.CurrentDirectory);
        }
        
        //-- 下面是主机专用的命令--//
        if (!AmongUsClient.Instance.AmHost) return;
        //立即开始
        if (Input.GetKeyDown(KeyCode.LeftShift) && GetPlayer.IsCountDown)
        {
            Main.Logger.LogInfo("倒计时修改为0");
            GameStartManager.Instance.countDownTimer = 0;
        }
        
        //倒计时取消
        if (Input.GetKeyDown(KeyCode.C) && GetPlayer.IsCountDown)
        {
            Main.Logger.LogInfo("重置倒计时");
            GameStartManager.Instance.ResetStartState();
            SendInGamePatch.SendInGame("取消倒计时");
        }
    }
    public static void DumpLog()
    {
        string f = $"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}/YuAC-logs/";
        string t = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss");
        string filename = $"{f}YuAC-v{Main.PluginVersion}-{t}.log";
        if (!Directory.Exists(f)) Directory.CreateDirectory(f);
        FileInfo file = new(@$"{Environment.CurrentDirectory}/BepInEx/LogOutput.log");
        file.CopyTo(@filename);
        SendInGamePatch.SendInGame($"日志已保存 YuAC - v{Main.PluginVersion}-{t}.log");
        ProcessStartInfo psi = new ProcessStartInfo("Explorer.exe")
            { Arguments = "/e,/select," + @filename.Replace("/", "\\") };
        Process.Start(psi);
    }
    public static void OpenDirectory(string path)
    {
        var startInfo = new ProcessStartInfo(path)
        {
            UseShellExecute = true,
        };
        Process.Start(startInfo);
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