using System;
using System.Diagnostics;
using System.IO;

namespace YuAntiCheat;

public class FunctionPatch
{
    public static void DumpLogKey()
    {
        Main.Logger.LogInfo("输出日志");
        DumpLog();
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
        SendInGamePatch.SendInGame(Translator.GetString("FunctionPatch.AbolishDownTimer"));
    }
    
    public static void DumpLog()
    {
        string f = $"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}/YuAC-logs/";
        string t = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss");
        string filename = $"{f}YuAC-v{Main.PluginVersion}-{t}.log";
        if (!Directory.Exists(f)) Directory.CreateDirectory(f);
        FileInfo file = new(@$"{Environment.CurrentDirectory}/BepInEx/LogOutput.log");
        file.CopyTo(@filename);
        SendInGamePatch.SendInGame($"{Translator.GetString("FunctionPatch.DumpLog")} YuAC - v{Main.PluginVersion}-{t}.log");
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