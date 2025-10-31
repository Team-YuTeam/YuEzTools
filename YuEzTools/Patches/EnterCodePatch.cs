using TMPro;
using UnityEngine;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(EnterCodeManager))]
public class EnterCodePatch
{
    public static bool ifFirst = true;
    public static GameObject MapShow;
    
    [HarmonyPatch(nameof(EnterCodeManager.OnEnable)), HarmonyPostfix]
    public static void OnEnable_Postfix(EnterCodeManager __instance)
    {
        var JoinGameButton = __instance.joinGamePassiveButton;
        var FieldsContainer = __instance.fieldsContainer;
        var EnterCodeField = __instance.enterCodeField;
        var Chat = FieldsContainer.transform.Find("Chat");
            
        if (ifFirst)
        {
            EnterCodeField.transform.localPosition += new Vector3(0f,0.2f,0f);
            FieldsContainer.transform.localPosition += new Vector3(0f,0.28f,0f);
            JoinGameButton.transform.localPosition -= new Vector3(0f,0.2f,0f);
            MapShow = Object.Instantiate(Chat.gameObject,Chat.parent);
            MapShow.gameObject.name = "Map";
            MapShow.transform.localPosition -= new Vector3(0f,0.55f,0f);
            var title = MapShow.transform.FindChild("Title");
            Object.Destroy(title.gameObject.GetComponent<TextTranslatorTMP>());
            var titletmp = title.gameObject.GetComponent<TextMeshPro>();
            titletmp.text = GetString("EnterCodePatch.Map");
            var text_tmp = MapShow.transform.FindChild("Text_TMP");
            var background = MapShow.transform.FindChild("Background");
            var sprite = Object.Instantiate(background.gameObject,background.parent);
            sprite.transform.localPosition = text_tmp.localPosition;
            sprite.transform.localRotation = text_tmp.localRotation;
            sprite.transform.localScale = text_tmp.localScale;
            sprite.transform.localScale -= new Vector3(0.35f,0.2f,0f);
            // sprite.gameObject.AddComponent<SpriteRenderer>();
            sprite.name = "Sprite";
            sprite.GetComponent<SpriteRenderer>().sortingOrder = background.gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
            sprite.SetActive(false);
            Object.Destroy(text_tmp.gameObject);
            ifFirst = false;
        }
        

    }
    
    [HarmonyPatch(nameof(EnterCodeManager.FindGameResult)), HarmonyPostfix]
    public static void FindGameResult_Postfix(EnterCodeManager __instance)
    {
        MapNames currentMap = (MapNames)__instance.gameFound.MapId;
        string mapNameText = currentMap.ToString();
        Info(mapNameText,"EnterCodePatch");
        var Sprite = MapShow.transform.FindChild("Sprite");
        var Sprite_sprite = Sprite.gameObject.GetComponent<SpriteRenderer>();
        Sprite_sprite.sprite = LoadSprite($"YuEzTools.Resources.MapsImages.{mapNameText}.png", 300f);
        Sprite.gameObject.SetActive(true);
    }
    
    [HarmonyPatch(nameof(EnterCodeManager.OnDisable)), HarmonyPostfix]
    public static void OnDisable_Postfix(EnterCodeManager __instance)
    {
        var Sprite = MapShow.transform.FindChild("Sprite");
        Sprite.gameObject.SetActive(false);
    }
}