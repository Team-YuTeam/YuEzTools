using System.Text;
using TMPro;
using UnityEngine;
using YuEzTools.Helper;
using YuEzTools.Helpers;
using YuEzTools.Modules;

namespace YuEzTools.UI;

// Thanks FinalSuspect_Xtreme
public static class InGameRoleInfoMenu
{
    public static bool Showing => Fill != null && Fill.active && Menu != null && Menu.active;

    public static GameObject Fill;
    public static SpriteRenderer FillSP => Fill.GetComponent<SpriteRenderer>();

    public static GameObject Menu;

    public static GameObject MainInfo;
    public static TextMeshPro MainInfoTMP => MainInfo.GetComponent<TextMeshPro>();

    public static void Init()
    {
        var DOBScreen = AccountManager.Instance.transform.FindChild("DOBEnterScreen");

        Fill = new("YuEzTools Role Info Menu Fill") { layer = 5 };
        Fill.transform.SetParent(HudManager.Instance.transform.parent, true);
        Fill.transform.localPosition = new(0f, 0f, -980f);
        Fill.transform.localScale = new(20f, 10f, 1f);
        Fill.AddComponent<SpriteRenderer>().sprite = DOBScreen.FindChild("Fill").GetComponent<SpriteRenderer>().sprite;
        FillSP.color = new(0f, 0f, 0f, 0.75f);

        Menu = Object.Instantiate(DOBScreen.FindChild("InfoPage").gameObject, HudManager.Instance.transform.parent);
        Menu.name = "YuEzTools Role Info Menu Page";
        Menu.transform.SetLocalZ(-990f);

        Object.Destroy(Menu.transform.FindChild("Title Text").gameObject);
        Object.Destroy(Menu.transform.FindChild("BackButton").gameObject);
        Object.Destroy(Menu.transform.FindChild("EvenMoreInfo").gameObject);

        MainInfo = Menu.transform.FindChild("InfoText_TMP").gameObject;
        MainInfo.name = "Main Role Info";
        MainInfo.DestroyTranslator();
        MainInfo.transform.localPosition = new(-2.3f, 0.8f, 4f);
        MainInfo.GetComponent<RectTransform>().sizeDelta = new(4.5f, 10f);
        MainInfoTMP.alignment = TextAlignmentOptions.Left;
        MainInfoTMP.fontSize = 2f;
    }

    public static void SetRoleInfoRef(PlayerControl player)
    {
        if (player == null) return;
        if (!Fill || !Menu) Init();
        var builder = new StringBuilder(256);
        builder.AppendFormat("<size={0}>\n", BlankLineSize);
        // 职业名
        var role = player.Data.Role.Role;
        builder.AppendFormat("<size={0}>{1}", FirstHeaderSize, Utils.Utils.ColorString(RoleColorHelper.GetRoleColor32(player.Data.RoleType), player.GetRoleName()));
        // 职业阵营 / 原版职业
        var roleTeam = player.IsImpostor() ? "Impostor" : "Crewmate";
        builder.AppendFormat("<size={0}> ({1})\n\n", BodySize, Translator.GetString($"Team{roleTeam}"));
        builder.AppendFormat("<size={0}>{1}\n", BodySize, player.Data.RoleType.GetRoleLInfoForVanilla());
        MainInfoTMP.text = builder.ToString();
    }

    public static void Show()
    {
        if (!Fill || !Menu) Init();
        if (!Showing)
        {
            Fill?.SetActive(true);
            Menu?.SetActive(true);
        }
        //HudManager.Instance?.gameObject.SetActive(false);
    }
    public static void Hide()
    {
        if (Showing)
        {
            Fill?.SetActive(false);
            Menu?.SetActive(false);
        }
        //HudManager.Instance?.gameObject?.SetActive(true);
    }
    public const string FirstHeaderSize = "130%";
    public const string SecondHeaderSize = "100%";
    public const string BodySize = "70%";
    public const string BlankLineSize = "30%";
}