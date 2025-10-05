using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using LogLevel = BepInEx.Logging.LogLevel;

namespace YuEzTools.Modules;

class Logger
{
    public static bool isEnable;
    public static List<string> disableList = [];
    public static List<string> sendToGameList = [];
    public static bool isDetail = false;
    public static bool isAlsoInGame = false;
    public static void Enable() => isEnable = true;
    public static void Disable() => isEnable = false;
    public static void Enable(string tag, bool toGame = false)
    {
        disableList.Remove(tag);
        if (toGame && !sendToGameList.Contains(tag)) sendToGameList.Add(tag);
        else sendToGameList.Remove(tag);
    }
    public static void Disable(string tag)
    {
        if (!disableList.Contains(tag)) disableList.Add(tag);
    }
    private static void SendToFile(string text, LogLevel level = LogLevel.Info, string tag = "", bool escapeCRLF = true, int lineNumber = 0, string fileName = "")
    {
        if (!isEnable || disableList.Contains(tag)) return;
        var logger = Main.Logger;
        string t = DateTime.Now.ToString("HH:mm:ss");
        //if(Main.ModMode == 0) SendInGamePatch.SendInGame($"[{tag}]{text}");
        if (escapeCRLF)
            text = text.Replace("\r", "\\r").Replace("\n", "\\n");
        string log_text = $"[{t}][{tag}]{text}";
        if (isDetail && Main.ModMode == 0)
        {
            StackFrame stack = new(2);
            string className = stack.GetMethod().ReflectedType.Name;
            string memberName = stack.GetMethod().Name;
            log_text = $"[{t}][{className}.{memberName}({Path.GetFileName(fileName)}:{lineNumber})][{tag}]{text}";
        }
        switch (level)
        {
            case LogLevel.Info:
                logger.LogInfo(log_text);
                break;
            case LogLevel.Warning:
                logger.LogWarning(log_text);
                break;
            case LogLevel.Error:
                logger.LogError(log_text);
                break;
            case LogLevel.Fatal:
                logger.LogFatal(log_text);
                break;
            case LogLevel.Message:
                logger.LogMessage(log_text);
                break;
            case LogLevel.Debug:
                logger.LogFatal(log_text);
                break;
            default:
                logger.LogWarning("Error:Invalid LogLevel");
                logger.LogInfo(log_text);
                break;
        }
    }
    public static void Test(object content, string tag = "======= Test =======", bool escapeCRLF = true, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string fileName = "") =>
        SendToFile(content.ToString(), LogLevel.Debug, tag, escapeCRLF, lineNumber, fileName);
    public static void Info(string text, string tag, bool escapeCRLF = true, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string fileName = "") =>
        SendToFile(text, LogLevel.Info, tag, escapeCRLF, lineNumber, fileName);
    public static void Warn(string text, string tag, bool escapeCRLF = true, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string fileName = "") =>
        SendToFile(text, LogLevel.Warning, tag, escapeCRLF, lineNumber, fileName);
    public static void Error(string text, string tag, bool escapeCRLF = true, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string fileName = "") =>
        SendToFile(text, LogLevel.Error, tag, escapeCRLF, lineNumber, fileName);
    public static void Fatal(string text, string tag, bool escapeCRLF = true, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string fileName = "") =>
        SendToFile(text, LogLevel.Fatal, tag, escapeCRLF, lineNumber, fileName);
    public static void Msg(string text, string tag, bool escapeCRLF = true, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string fileName = "") =>
        SendToFile(text, LogLevel.Message, tag, escapeCRLF, lineNumber, fileName);
    public static void Exception(Exception ex, string tag, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string fileName = "") =>
        SendToFile(ex.ToString(), LogLevel.Error, tag, false, lineNumber, fileName);
    public static void CurrentMethod([CallerLineNumber] int lineNumber = 0, [CallerFilePath] string fileName = "")
    {
        StackFrame stack = new(1);
        Msg($"\"{stack.GetMethod().ReflectedType.Name}.{stack.GetMethod().Name}\" Called in \"{Path.GetFileName(fileName)}({lineNumber})\"", "Method");
    }

    public static LogHandler Handler(string tag)
        => new(tag);
}