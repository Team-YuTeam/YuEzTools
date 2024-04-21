// using HarmonyLib;
// using System.Text;
// using TMPro;
// using UnityEngine;
//
// namespace YuAntiCheat.UI;
//
// [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
// internal class TitleLogoPatch
// {
//     public static GameObject Ambience;
//     public static GameObject amongUsLogo;
//     public static GameObject PlayLocalButton;
//     public static GameObject PlayOnlineButton;
//     public static GameObject HowToPlayButton;
//     public static GameObject FreePlayButton;
//     public static GameObject BottomButtons;
//     public static GameObject LoadingHint;
//
//     private static void Postfix(MainMenuManager __instance)
//     {
//         
//         if ((amongUsLogo = GameObject.Find("bannerLogo_AmongUs")) != null)
//         {
//             amongUsLogo.transform.localScale *= 0.4f;
//             amongUsLogo.transform.position += Vector3.up * 0.25f;
//         }
//
//         if ((PlayLocalButton = GameObject.Find("PlayLocalButton")) != null)
//         {
//             PlayLocalButton.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
//             PlayLocalButton.transform.position = new Vector3(-0.76f, -2.1f, 0f);
//         }
//
//         if ((PlayOnlineButton = GameObject.Find("PlayOnlineButton")) != null)
//         {
//             PlayOnlineButton.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
//             PlayOnlineButton.transform.position = new Vector3(0.725f, -2.1f, 0f);
//         }
//
//         if ((HowToPlayButton = GameObject.Find("HowToPlayButton")) != null)
//         {
//             HowToPlayButton.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
//             HowToPlayButton.transform.position = new Vector3(-2.225f, -2.175f, 0f);
//         }
//
//         if ((FreePlayButton = GameObject.Find("FreePlayButton")) != null)
//         {
//             FreePlayButton.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
//             FreePlayButton.transform.position = new Vector3(2.1941f, -2.175f, 0f);
//         }
//
//         if ((BottomButtons = GameObject.Find("BottomButtons")) != null)
//         {
//             BottomButtons.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
//             BottomButtons.transform.position = new Vector3(0f, -2.71f, 0f);
//         }
//
//         if ((Ambience = GameObject.Find("Ambience")) != null)
//         {
//             Ambience.SetActive(false);
//             var CustomBG = new GameObject("CustomBG");
//             CustomBG.transform.position = new Vector3(0, 0, 520f);
//             var bgRenderer = CustomBG.AddComponent<SpriteRenderer>();
//             //bgRenderer.sprite = Utils.LoadSprite("TOHE.Resources.Images.TOHE-BG.jpg", 179f);
//         }
//     }
// }