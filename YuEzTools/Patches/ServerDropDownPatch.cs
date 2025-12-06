#if Windows
//Thanks TONX
using System;
using System.Linq;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using YuEzTools.Patches;
using Object = Il2CppSystem.Object;

namespace YuEzTools.Patches;

[HarmonyPatch(typeof(ServerDropdown))]
public static class ServerDropdownPatch
{
    private static int CurrentPage = 1;
    private static int MaxPage = 1;
    private const int ButtonsPerPage = 4;

    [HarmonyPatch(nameof(ServerDropdown.FillServerOptions)), HarmonyPostfix]
    public static void FillServerOptions_Postfix(ServerDropdown __instance)
    {
        // 1. 获取所有服务器按钮并排序
        List<ServerListButton> serverListButtons = __instance.ButtonPool.GetComponentsInChildren<ServerListButton>()
            .OrderByDescending(x => x.transform.localPosition.y)
            .ToList();
        
        // 2. 提前获取所有可用区域信息（用于匹配按钮对应的RegionInfo）
        var allRegions = DestroyableSingleton<ServerManager>.Instance.AvailableRegions
            .OrderBy(ServerManager.DefaultRegions.Contains).ToList();

        MaxPage = Mathf.Max(1, Mathf.CeilToInt((float)serverListButtons.Count / ButtonsPerPage));
        if (CurrentPage > MaxPage) CurrentPage = MaxPage;

        // 调整服务器选项按钮位置 + 设置带颜色的文本
        int num = 0;
        int count = 1;
        foreach (ServerListButton button in serverListButtons)
        {
            if (num < (CurrentPage - 1) * ButtonsPerPage || num >= CurrentPage * ButtonsPerPage)
            {
                button.gameObject.SetActive(false);
                num++;
                continue;
            }

            // 3. 核心：匹配按钮对应的RegionInfo，设置带颜色的文本
            // 从所有区域中匹配按钮关联的区域（通过名称匹配，需确保按钮逻辑中已关联Name）
            var targetRegion = allRegions.FirstOrDefault(region => region.Name == button.Text.text);
            if (targetRegion != null)
            {
                // 复用第一段的ColorString逻辑，设置带颜色的文本
                button.Text.text = ColorString(
                    ServerAddManager.GetServerColor32(targetRegion.Name), 
                    GetString(targetRegion.Name)
                );
                button.Text.ForceMeshUpdate(); // 强制更新文本渲染
            }

            // 调整按钮位置（原有逻辑）
            button.transform.localPosition = new Vector3(0f, __instance.y_posButton + -0.55f * count, -1f);
            button.gameObject.SetActive(true); // 确保激活
            num++;
            count++;
        }

        // 调整背景大小和位置（原有逻辑）
        __instance.background.transform.localPosition = new Vector3(0f, __instance.initialYPos + -0.3f * (ButtonsPerPage + 1), -1f);
        __instance.background.size = new Vector2(__instance.background.size.x, 1.2f + 0.6f * (ButtonsPerPage + 1));

        // 创建翻页按钮（原有逻辑）
        CreateServerListButton(__instance, "PreviousPageButton", GetString("PreviousPage"), new Vector3(0f, __instance.y_posButton, -1f), () =>
        {
            CurrentPage = CurrentPage > 1 ? CurrentPage - 1 : MaxPage;
            RefreshServerOptions(__instance);
        });
        CreateServerListButton(__instance, "NextPageButton", GetString("NextPage"), new Vector3(0f, __instance.y_posButton + -0.55f * (ButtonsPerPage + 1), -1f), () =>
        {
            CurrentPage = CurrentPage < MaxPage ? CurrentPage + 1 : 1;
            RefreshServerOptions(__instance);
        });
    }

    private static void CreateServerListButton(ServerDropdown __instance, string name, string text, Vector3 position, Action onclickaction)
    {
        ServerListButton button = __instance.ButtonPool.Get<ServerListButton>();
        button.name = name;
        button.transform.localPosition = position;
        button.transform.localScale = Vector3.one;
        button.Text.text = text;
        button.Text.ForceMeshUpdate();
        button.Button.OnClick.RemoveAllListeners();
        button.Button.OnClick.AddListener(onclickaction);
        button.gameObject.SetActive(true);
    }

    private static void RefreshServerOptions(ServerDropdown __instance)
    {
        __instance.ButtonPool.ReclaimAll();
        __instance.controllerSelectable = new Il2CppSystem.Collections.Generic.List<UiElement>();
        __instance.FillServerOptions();
    }
}
#endif