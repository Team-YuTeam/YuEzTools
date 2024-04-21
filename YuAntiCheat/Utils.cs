using UnityEngine;
using InnerNet;
using System.Linq;
using Il2CppSystem.Collections.Generic;
using System.IO;
using Hazel;
using System.Reflection;
using AmongUs.GameOptions;
using Sentry.Internal.Extensions;

namespace YuAntiCheat.Utils;

public class Utils
{
    public static string getColoredPingText(int ping){

        if (ping <= 100){ // Green for ping < 100

            return $"<color=#00ff00ff>\nPing: {ping} ms</color>";

        } else if (ping < 400){ // Yellow for 100 < ping < 400

            return $"<color=#ffff00ff>\nPing: {ping} ms</color>";

        } else{ // Red for ping > 400

            return $"<color=#ff0000ff>\nPing: {ping} ms</color>";
        }
    }
}