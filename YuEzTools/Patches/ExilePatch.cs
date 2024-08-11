using HarmonyLib;
using LibCpp2IL.MachO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuEzTools.Modules;
using YuEzTools.Utils;

namespace YuEzTools.Patches;

internal class ExilePatch
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    class BaseExileControllerPatch
    {
        public static void Prefix(ExileController __instance)
        {
            try
            {
                if (__instance.exiled != null)
                {
                    __instance.exiled.PlayerId.GetPlayerDataById().SetExiled();
                    ModPlayerData.GetModPlayerDataById(__instance.exiled.PlayerId).SetDeadReason(DeadReasonData.Exile);
                }
            }
            catch { }
        }
    }
}