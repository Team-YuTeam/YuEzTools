using HarmonyLib;
using AmongUs.Data;
using UnityEngine;
using System;
using System.Security.Cryptography;

namespace YuAnitCheat;

[HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
public static class VersionShower_Start
{
    // Postfix patch of VersionShower.Start to show MalumMenu version
    public static void Postfix(VersionShower __instance)
    {
            __instance.text.text = $"\n<color={Main.ModColor}>{Main.ModName}</color><color=#00FFFF> v{Main.PluginVersion}</color>(v{Application.version})";
    }
}