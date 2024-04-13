using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using YuAntiCheat;
using YuAntiCheat.Get;

namespace MCI.Patches
{
    [HarmonyPriority(Priority.Low)]
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class PingTracker_Update
    {
        [HarmonyPostfix]
        public static void Postfix(PingTracker __instance)
        {
            var position = __instance.GetComponent<AspectPosition>();
            position.DistanceFromEdge = new Vector3(3.6f, 0.1f, 0);
            position.AdjustPosition();
            __instance.text.text +=
                $"\n<color={Main.ModColor}>{Main.ModName}</color><color=#00FFFF> v{Main.PluginVersion}</color>\n{Main.MainMenuText}";

            if (Main.safemode)
                 __instance.text.text += "\n<color=#DC143C>[Safe Mode]</color>";
            else
                __instance.text.text += "\n<color=#1E90FF>[UnSafe Mode]</color>";

         __instance.text.text += "\n<color=#FFFF00>By</color> <color=#FF0000>Yu</color>";
#if Debug
__instance.text.text += "\n<color=#FFC0CB>Debug</color>";
#endif
#if CANARY
            __instance.text.text += "\n<color=#6A5ACD>Canary</color>";
#endif
        }
    }
}
