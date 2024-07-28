using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;
using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;
using AmongUs.Data;
using Assets.InnerNet;
using System.Linq;
using System.Net;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using System.IO;
using System.Reflection;

namespace YuEzTools;

public class OnlyYuEzToolsCheat
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