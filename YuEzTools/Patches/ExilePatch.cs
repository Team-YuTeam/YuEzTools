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
                if (__instance.initData.networkedPlayer != null)
                {
                    __instance.initData.networkedPlayer.PlayerId.GetPlayerDataById().SetExiled();
                    __instance.initData.networkedPlayer.PlayerId.GetPlayerDataById().SetDeadReason(DeadReasonData.Exile);
                    __instance.initData.networkedPlayer.PlayerId.GetPlayerDataById().SetDead();
                }
            }
            catch { }
        }
    }
}