using HarmonyLib;
using UnityEngine;
using Il2CppSystem.Collections.Generic;
using System;
using AmongUs.GameOptions;
using YuAntiCheat;
using YuAntiCheat.Get;

namespace YuAntiCheat.Key;

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
        
        // if (Input.GetKeyDown(KeyCode.RightShift))
        // {
        //     GameManager.Instance.LogicFlow.CheckEndCriteria();
        // }

        // if (Input.GetKeyDown(KeyCode.LeftShift))
        // {
        //     Main.Logger.LogInfo("倒计时修改为0");
        //     GameStartManager.Instance.MinPlayers = 0;//设置0人启动
        //     GameStartManager.Instance.StartButton.enabled = false;
        //     GameStartManager.Instance.startLabelText.enabled = true;
        //     GameStartManager.Instance.startLabelText.text = "请按下我以开始游戏修复";
        // }

    }
}
