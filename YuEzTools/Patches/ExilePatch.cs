using HarmonyLib;
using YuEzTools.Modules;

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
                    ModPlayerData.GetModPlayerDataById(__instance.initData.networkedPlayer.PlayerId).SetDeadReason(DeadReasonData.Exile);
                }
            }
            catch { }
        }
    }
}