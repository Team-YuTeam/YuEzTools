using Hazel;
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using YuEzTools.Modules;

namespace YuEzTools.Patches;

public class FunctionPatch
{
    public static float exitTimer = -1f;
    public static bool kickGameActive;
    public static void DumpLogKey()
    {
        Main.Logger.LogInfo("输出日志");
        DumpLog();
    }

    public static void ShowRoleM()
    {
        HudManager.Instance.StartCoroutine(HudManager.Instance.CoFadeFullScreen(Color.clear, Color.black));
        HudManager.Instance.StartCoroutine(HudManager.Instance.CoShowIntro());
    }

    public static void ExitGame()
    {
        AmongUsClient.Instance.ExitGame(DisconnectReasons.ExitGame);
    }

    public static void RealBan()
    {
        var HostData = AmongUsClient.Instance.GetHost();
        if (HostData != null)
        {
            foreach (var item in PlayerControl.AllPlayerControls)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.CheckVanish, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.CheckShapeshift, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }

    public static void OpenGameDic()
    {
        OpenDirectory(Environment.CurrentDirectory);
    }

    public static void ChangeDownTimerTo(int c)
    {
        Main.Logger.LogInfo("倒计时修改为" + c);
        GameStartManager.Instance.countDownTimer = c;
    }

    public static void AbolishDownTimer()
    {
        Main.Logger.LogInfo("重置倒计时");
        GameStartManager.Instance.ResetStartState();
        SendInGamePatch.SendInGame(GetString("FunctionPatch.AbolishDownTimer"));
    }

    public static void DumpLog()
    {
        string f = $"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}/YuET-logs/";
        string t = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss");
        string filename = $"{f}YuET-v{Main.PluginVersion}-{t}.log";
        if (!Directory.Exists(f)) Directory.CreateDirectory(f);
        FileInfo file = new(@$"{Environment.CurrentDirectory}/BepInEx/LogOutput.log");
        file.CopyTo(@filename);
        SendInGamePatch.SendInGame($"{GetString("FunctionPatch.DumpLog")} YuET - v{Main.PluginVersion}-{t}.log");
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
}