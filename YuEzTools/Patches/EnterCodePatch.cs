using TMPro;
using UnityEngine;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(EnterCodeManager))]
public class EnterCodePatch
{
    public static bool ifFirst = true;
    public static GameObject MapShow;
    public static TextMeshPro Host_TMP;
    public static TextMeshPro Capacity_TMP;
    public static TextMeshPro Server_TMP;
    

    [HarmonyPatch(nameof(EnterCodeManager.OnEnable)), HarmonyPostfix]
    public static void OnEnable_Postfix(EnterCodeManager __instance)
    {
        var JoinGameButton = __instance.joinGamePassiveButton;
        var FieldsContainer = __instance.fieldsContainer;
        var EnterCodeField = __instance.enterCodeField;
        var Chat = FieldsContainer.transform.Find("Chat");
        var Host = FieldsContainer.transform.Find("Host");
        var Capacity = FieldsContainer.transform.Find("Capacity");
        var Server = FieldsContainer.transform.Find("Server");
        
        if (ifFirst)
        {
            EnterCodeField.transform.localPosition += new Vector3(0f,0.2f,0f);
            FieldsContainer.transform.localPosition += new Vector3(0f,0.28f,0f);
            JoinGameButton.transform.localPosition -= new Vector3(0f,0.2f,0f);
            
            // 房主
            var host_title = Host.transform.FindChild("Title");
            Object.Destroy(host_title.gameObject.GetComponent<TextTranslatorTMP>());
            var host_titletmp = host_title.gameObject.GetComponent<TextMeshPro>();
            host_titletmp.text = GetString("EnterCodePatch.Host");
            Host_TMP = Host.transform.FindChild("Text_TMP").gameObject.GetComponent<TextMeshPro>();
            
            // 人数
            var Capacity_title = Capacity.transform.FindChild("Title");
            Object.Destroy(Capacity_title.gameObject.GetComponent<TextTranslatorTMP>());
            var Capacity_titletmp = Capacity_title.gameObject.GetComponent<TextMeshPro>();
            Capacity_titletmp.text = GetString("EnterCodePatch.Capacity");
            Capacity_TMP = Capacity.transform.FindChild("Container").FindChild("Text_TMP").gameObject.GetComponent<TextMeshPro>();
            
            // 地区（语言）
            Server_TMP = Server.transform.FindChild("Text_TMP").gameObject.GetComponent<TextMeshPro>();
            
            // 地图
            MapShow = Object.Instantiate(Chat.gameObject,Chat.parent);
            MapShow.gameObject.name = "Map";
            MapShow.transform.localPosition -= new Vector3(0f,0.55f,0f);
            var map_title = MapShow.transform.FindChild("Title");
            Object.Destroy(map_title.gameObject.GetComponent<TextTranslatorTMP>());
            var map_titletmp = map_title.gameObject.GetComponent<TextMeshPro>();
            map_titletmp.text = GetString("EnterCodePatch.Map");
            var map_text_tmp = MapShow.transform.FindChild("Text_TMP");
            var map_background = MapShow.transform.FindChild("Background");
            var map_sprite = Object.Instantiate(map_background.gameObject,map_background.parent);
            map_sprite.transform.localPosition = map_text_tmp.localPosition;
            map_sprite.transform.localRotation = map_text_tmp.localRotation;
            map_sprite.transform.localScale = map_text_tmp.localScale;
            map_sprite.transform.localScale -= new Vector3(0.35f,0.2f,0f);
            // sprite.gameObject.AddComponent<SpriteRenderer>();
            map_sprite.name = "Sprite";
            map_sprite.GetComponent<SpriteRenderer>().sortingOrder = map_background.gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
            map_sprite.SetActive(false);
            
            Object.Destroy(map_text_tmp.gameObject);
            
            ifFirst = false;
        }
        

    }

    public static bool isJoin = false;
    
    [HarmonyPatch(nameof(EnterCodeManager.FindGameResult)), HarmonyPostfix]
    public static void FindGameResult_Postfix(EnterCodeManager __instance, [HarmonyArgument(0)] HttpMatchmakerManager.FindGameByCodeResponse response)
    {
        var gameFound = __instance.gameFound;
        MapNames currentMap = (MapNames)gameFound.MapId;
        string mapNameText = currentMap.ToString();
        
        var Sprite = MapShow.transform.FindChild("Sprite");
        var Sprite_sprite = Sprite.gameObject.GetComponent<SpriteRenderer>();
        Sprite_sprite.sprite = LoadSprite($"YuEzTools.Resources.MapsImages.{mapNameText}.png", 300f);
        Sprite.gameObject.SetActive(true);
        
        ServerAddManager.SetServerName(response.UntranslatedRegion);
        isJoin = true;

        Server_TMP.text = Server_TMP.text.isInServerDictionary() ? ColorString(ServerAddManager.GetServerColor32(Server_TMP.text), GetString(Server_TMP.text)) : Server_TMP.text;
        // Server_TMP.text += gameFound.Language.ToString();
        Host_TMP.text += $"-{gameFound.Platform.GetPlatformColorText()}";
        Capacity_TMP.text += $" <color=#FF0000>({gameFound.NumImpostors})</color>";
    }
    
    [HarmonyPatch(nameof(EnterCodeManager.OnDisable)), HarmonyPostfix]
    public static void OnDisable_Postfix(EnterCodeManager __instance)
    {
        var Sprite = MapShow.transform.FindChild("Sprite");
        Sprite.gameObject.SetActive(false);
    }
}