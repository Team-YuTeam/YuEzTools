using HarmonyLib;
using UnityEngine;
using YuEzTools.UI;

namespace YuEzTools;

[HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
public class LobbyStartPatch
{
    private static GameObject Paint;
    public static void Postfix(LobbyBehaviour __instance)
    {
        if (Paint != null) return;
        Paint = Object.Instantiate(__instance.transform.FindChild("Leftbox").gameObject, __instance.transform);
        Paint.name = "YuET Lobby Paint";
        Paint.transform.localPosition = new Vector3(0.042f, -2.59f, -10.5f);
        Paint.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        SpriteRenderer renderer = Paint.GetComponent<SpriteRenderer>();
        renderer.sprite = TitleLogoPatch.LoadSprite("YuEzTools.Resources.Yu-Logo-tm.png", 200f);
    }
}