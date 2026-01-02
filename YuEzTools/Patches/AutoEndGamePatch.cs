using YuEzTools.Modules;

namespace YuEzTools.Patches;

[HarmonyPatch]
public class EngGamePatch
{
    [HarmonyPatch(typeof(EndGameNavigation), nameof(EndGameNavigation.ShowDefaultNavigation)), HarmonyPostfix]
    public static void ShowDefaultNavigation_Postfix(EndGameNavigation __instance)
    {
        if (!Toggles.AutoStartGame) return;
        _ = new LateTask(__instance.NextGame, 2f, "Auto End Game");
        __instance.CoJoinGame();
    }
}