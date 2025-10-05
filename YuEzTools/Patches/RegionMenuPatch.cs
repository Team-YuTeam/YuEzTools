using UnityEngine;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(RegionMenu))]
public class RegionMenuPatch
{
    public static Scroller Scroller;

    [HarmonyPatch(nameof(RegionMenu.Awake)), HarmonyPostfix]
    public static void Awake_Postfix(RegionMenu __instance)
    {
        if (Scroller != null) return;

        var back = __instance.ButtonPool.transform.FindChild("Backdrop");
        back.transform.localScale *= 10f;

        Scroller = __instance.ButtonPool.transform.parent.gameObject.AddComponent<Scroller>();
        Scroller.Inner = __instance.ButtonPool.transform;
        Scroller.MouseMustBeOverToScroll = true;
        Scroller.ClickMask = back.GetComponent<BoxCollider2D>();
        Scroller.ScrollWheelSpeed = 0.7f;
        Scroller.SetYBoundsMin(0f);
        Scroller.SetYBoundsMax(4f);
        Scroller.allowY = true;
    }
    [HarmonyPatch(nameof(RegionMenu.ChooseOption)), HarmonyPostfix]
    public static void ChooseOption_Postfix()
        => ServerAddManager.SetServerName();
}