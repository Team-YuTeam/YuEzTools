using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using AmongUs.Data;
using AmongUs.GameOptions;
using Hazel;
using Il2CppInterop.Runtime.InteropTypes;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using YuAntiCheat.Get;
using YuAntiCheat;

namespace YuAntiCheat.Keys;

[HarmonyPatch(typeof(ControllerManager), nameof(ControllerManager.Update))]
internal class Keys
{
    public static void Postfix(ControllerManager __instance)
    {
        //日志文件转储
        if (GetKeysDown(KeyCode.F1, KeyCode.LeftControl))
        {
            Main.Logger.LogInfo("输出日志");
            DumpLog();
        }
        
        //开启非安全模式
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Main.safemode = !Main.safemode;
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
        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe")
            { Arguments = "/e,/select," + @filename.Replace("/", "\\") };
        System.Diagnostics.Process.Start(psi);
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