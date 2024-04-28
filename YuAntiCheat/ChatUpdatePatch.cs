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
using YuAntiCheat.Get;
using YuAntiCheat;
using Byte = Il2CppSystem.Byte;

namespace YuAntiCheat;

[HarmonyPatch(typeof(ChatController),nameof(ChatController.Update))]
class ChatUpdatePatch
{
    public static float TSLM = 0.0f;
    public static void Postfix(ChatController __instance)
    {
        TSLM = __instance.timeSinceLastMessage;
    }
}