using HarmonyLib;
using UnityEngine;
using Il2CppSystem.Collections.Generic;
using System;
using AmongUs.GameOptions;
using YuAnitCheat;
using YuAnitCheat.Get;

namespace YuAnitCheat.Key;

[HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
public sealed class Keyboard_Joystick
{
    private static int controllingFigure;

    public static void Postfix()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Main.safemode = !Main.safemode;
        }

    }
}
