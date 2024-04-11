using AmongUs.GameOptions;
using Hazel;
using System;
using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using InnerNet;
using UnityEngine;
using YuAnitCheat.Get;
using YuAnitCheat;

namespace YuAnitCheat;

public class SMCheat
{
    public static bool ReceiveInvalidRpc(PlayerControl pc, byte callId)
    {
        switch (callId)
        {
            case unchecked((byte)420):
                Main.Logger.LogError($"有SickoMenu玩家，{"好友编号："+pc.FriendCode+"/名字："+pc.GetRealName()+"/实验性PUID获取："+pc.Puid}");
                return true;
            case 168:
                Main.Logger.LogError($"有SickoMenu玩家，{"好友编号："+pc.FriendCode+"/名字："+pc.GetRealName()+"/实验性PUID获取："+pc.Puid}");
                return true;
        }
        return false;
    }
}