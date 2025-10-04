using HarmonyLib;
using System.IO;
using System.Reflection;

namespace YuEzTools;

public class OnlyYuEzTools
{
    public static void DeleteOther()
    {
        foreach (var path in Directory.EnumerateFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "*.*"))
        {
            if (path.EndsWith(Path.GetFileName(Assembly.GetExecutingAssembly().Location))) continue;
            Main.Logger.LogInfo($"{Path.GetFileName(path)} 已删除");
            Harmony.UnpatchAll();
            File.Delete(path);
        }
    }
}