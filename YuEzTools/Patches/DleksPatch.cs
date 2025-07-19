using HarmonyLib;
using InnerNet;
using UnityEngine;
using YuEzTools.Get;

namespace YuEzTools.Patches;

// Thanks Galster (https://github.com/Galster-dev)
[HarmonyPatch(typeof(AmongUsClient._CoStartGameHost_d__28), nameof(AmongUsClient._CoStartGameHost_d__28.MoveNext))]
public static class DleksPatch
{
    private static bool Prefix(AmongUsClient._CoStartGameHost_d__28 __instance, ref bool __result)
    {
        if (__instance.__1__state != 0)
        {
            return true;
        }

        __instance.__1__state = -1;
        if (LobbyBehaviour.Instance)
        {
            LobbyBehaviour.Instance.Despawn();
        }

        if (ShipStatus.Instance)
        {
            __instance.__2__current = null;
            __instance.__1__state = 2;
            __result = true;
            return false;
        }

        // removed dleks check as it's always false
        var num2 = Mathf.Clamp(GameOptionsManager.Instance.CurrentGameOptions.MapId, 0, Constants.MapNames.Length - 1);
        __instance.__2__current = __instance.__4__this.ShipLoadingAsyncHandle = __instance.__4__this.ShipPrefabs[num2].InstantiateAsync();
        __instance.__1__state = 1;

        __result = true;
        return false;
    }
}
[HarmonyPatch(typeof(GameStartManager))]
class AllMapIconsPatch
{
    // Vanilla players getting error when trying get dleks map icon
    [HarmonyPatch(nameof(GameStartManager.Start)), HarmonyPostfix]
    public static void Postfix_AllMapIcons(GameStartManager __instance)
    {
        if (__instance == null) return;

        if (GetPlayer.isNormalGame && Main.NormalOptions.MapId == 3)
        {
            Main.NormalOptions.MapId = 0;
            __instance.UpdateMapImage(MapNames.Skeld);
        }
        else if (GetPlayer.IsHideNSeek && Main.HideNSeekOptions.MapId == 3)
        {
            Main.HideNSeekOptions.MapId = 0;
            __instance.UpdateMapImage(MapNames.Skeld);
        }

        MapIconByName DleksIncon = Object.Instantiate(__instance, __instance.gameObject.transform).AllMapIcons[0];
        DleksIncon.Name = MapNames.Dleks;
        DleksIncon.MapImage = Utils.Utils.LoadSprite($"YuEzTools.Resources.DleksBanner.png", 100f);
        DleksIncon.NameImage = Utils.Utils.LoadSprite($"YuEzTools.Resources.DleksBanner-Wordart.png", 100f);

        __instance.AllMapIcons.Add(DleksIncon);
    }
}
[HarmonyPatch(typeof(StringOption), nameof(StringOption.Start))]
class AutoSelectDleksPatch
{
    private static void Postfix(StringOption __instance)
    {
        if (__instance.Title == StringNames.GameMapName)
        {
            // vanilla clamps this to not auto select dleks
            __instance.Value = GameOptionsManager.Instance.CurrentGameOptions.MapId;
        }
    }
}