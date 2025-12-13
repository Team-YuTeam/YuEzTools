using UnityEngine;
using YuEzTools.Helpers;
using YuEzTools.Modules;
using YuEzTools.Patches;
using YuEzTools.Utils;

namespace YuEzTools.UI;

public class MenuUI : MonoBehaviour
{
    public List<GroupInfo> groups = [];
    private bool isDragging = false;
    private Rect windowRect = new(10, 10, 500, 500);
    private bool isGUIActive = false;
    private GUIStyle submenuButtonStyle;
    public static bool firstoOpenMenuUI = true;

    // Create all groups (buttons) and their toggles on start
    private void Start() { }

    private void Update()
    {
#if Windows
        // 原有快捷键逻辑保留
        if (Input.GetKeyDown(stringToKeycode(Main.menuKeybind.Value)))
        {
            isGUIActive = !isGUIActive;
            Vector2 mousePosition = Input.mousePosition;
            windowRect.position = new Vector2(mousePosition.x, Screen.height - mousePosition.y);
        }

        if (GetPlayer.IsPlayer)
        {
            Toggles.ServerAllHostOrNoHost = GameStartManagerPatch.roomMode == RoomMode.Normal ? false : true;
        }
        Main.WinTextSize.Value = Toggles.WinTextSize;

        // -------------------------- 关键修改：从统一配置生成 MenuUI 分组 --------------------------
        if (!isGUIActive)
        {
            groups = GenerateGroupsFromConfig(); // 自动生成分组，无需硬编码
        }
#endif
    }
    
    private List<GroupInfo> GenerateGroupsFromConfig()
{
    var groups = new List<GroupInfo>();
    var allConfigs = ToggleHelperManager.AllButtons;

    // 1. 按“组Key”分组（先获取所有不重复的组）
    var groupKeys = allConfigs.Select(c => c.GroupKey).Distinct().ToList();
    foreach (var groupKey in groupKeys)
    {
        // 当前组的所有按钮配置
        var groupConfigs = allConfigs.Where(c => c.GroupKey == groupKey).ToList();
        var groupName = GetString("MenuUI."+groupKey); // 组的本地化名称

        // 2. 拆分“直接在组下的按钮”和“子菜单下的按钮”
        var directToggles = new List<ToggleInfo>(); // 无 submenu 的按钮
        var submenus = new List<SubmenuInfo>();    // 有 submenu 的按钮

        // 处理“直接在组下的按钮”
        foreach (var config in groupConfigs.Where(c => string.IsNullOrEmpty(c.SubmenuKey)))
        {
            directToggles.Add(new ToggleInfo(
                label: GetString("MenuUI."+config.NameKey),
                getState: config.GetState,
                setState: val =>
                {
                    config.SetState(val);       // 更新 Toggles 字段
                    ClientToolsItem.RefreshToggle(val,config.NameKey);
                    config.AdditionalAction?.Invoke(); // 触发额外逻辑
                }
            ));
        }

        // 处理“子菜单下的按钮”（先按 submenuKey 分组）
        var submenuKeys = groupConfigs.Where(c => !string.IsNullOrEmpty(c.SubmenuKey))
                                     .Select(c => c.SubmenuKey)
                                     .Distinct()
                                     .ToList();
        foreach (var submenuKey in submenuKeys)
        {
            var submenuConfigs = groupConfigs.Where(c => c.SubmenuKey == submenuKey).ToList();
            var submenuName = GetString("MenuUI." + submenuKey); // 子菜单的本地化名称
            var submenuToggles = new List<ToggleInfo>();

            foreach (var config in submenuConfigs)
            {
                submenuToggles.Add(new ToggleInfo(
                    label: GetString("MenuUI." + config.NameKey),
                    getState: config.GetState,
                    setState: val => 
                    {
                        config.SetState(val);
                        // ClientToolsItem.RefreshToggle(val,config.NameKey); // ToDo 有报错
                        config.AdditionalAction?.Invoke();
                    }
                ));
            }

            submenus.Add(new SubmenuInfo(submenuName, isExpanded: false, submenuToggles));
        }

        // 3. 添加当前组到列表
        groups.Add(new GroupInfo(groupName, isExpanded: false, directToggles, submenus));
    }

    return groups;
}

    public void OnGUI()
    {

        if (!isGUIActive) return;

        if (submenuButtonStyle == null)
        {
            submenuButtonStyle = new GUIStyle(GUI.skin.button);

            submenuButtonStyle.normal.textColor = Color.white;

            submenuButtonStyle.fontSize = 18;
            GUI.skin.toggle.fontSize = GUI.skin.button.fontSize = 20;

            submenuButtonStyle.normal.background = Texture2D.grayTexture;
            submenuButtonStyle.normal.background.Apply();
        }

        //Only change the window height while the user is not dragging it
        //Or else dragging breaks
        if (!isDragging)
        {
            int windowHeight = CalculateWindowHeight();
            windowRect.height = windowHeight;
        }

        string configHtmlColor = Main.menuHtmlColor.Value;

        if (!ColorUtility.TryParseHtmlString(configHtmlColor, out Color uiColor))
        {
            if (!configHtmlColor.StartsWith("#"))
            {
                if (ColorUtility.TryParseHtmlString("#" + configHtmlColor, out uiColor))
                {
                    GUI.backgroundColor = uiColor;
                }
            }
        }
        else
        {
            GUI.backgroundColor = uiColor;
        }

        windowRect = GUI.Window(0, windowRect, (GUI.WindowFunction)WindowFunction, $"<color={Main.ModColor}>{Main.ModName}</color><color=#00FFFF> v{Main.PluginVersion}</color>");
    }

    public void WindowFunction(int windowID)
    {
        int groupSpacing = 50;
        int toggleSpacing = 40;
        int submenuSpacing = 40;
        int currentYPosition = 20;

        for (int groupId = 0; groupId < groups.Count; groupId++)
        {
            GroupInfo group = groups[groupId];

            if (GUI.Button(new Rect(10, currentYPosition, 480, 40), group.name))
            {
                group.isExpanded = !group.isExpanded;
                groups[groupId] = group;
                CloseAllGroupsExcept(groupId); // Close all other groups when one is expanded
            }
            currentYPosition += groupSpacing;

            if (group.isExpanded)
            {
                // Render direct toggles for the group
                foreach (var toggle in group.toggles)
                {
                    bool currentState = toggle.getState();
                    bool newState = GUI.Toggle(new Rect(20, currentYPosition, 460, 30), currentState, toggle.label);
                    if (newState != currentState)
                    {
                        toggle.setState(newState);
                    }
                    currentYPosition += toggleSpacing;
                }

                for (int submenuId = 0; submenuId < group.submenus.Count; submenuId++)
                {
                    var submenu = group.submenus[submenuId];

                    // Add a button for each submenu and toggle its expansion state when clicked
                    if (GUI.Button(new Rect(20, currentYPosition, 460, 30), submenu.name, submenuButtonStyle))
                    {
                        submenu.isExpanded = !submenu.isExpanded;
                        group.submenus[submenuId] = submenu;
                        if (submenu.isExpanded)
                        {
                            CloseAllSubmenusExcept(group, submenuId);
                        }
                    }
                    currentYPosition += submenuSpacing;

                    if (submenu.isExpanded)
                    {
                        // Show all the toggles in the expanded submenu
                        foreach (var toggle in submenu.toggles)
                        {
                            bool currentState = toggle.getState();
                            bool newState = GUI.Toggle(new Rect(30, currentYPosition, 450, 30), currentState, toggle.label);
                            if (newState != currentState)
                            {
                                toggle.setState(newState);
                            }
                            currentYPosition += toggleSpacing;
                        }
                    }
                }
            }
        }

        if (Event.current.type == EventType.MouseDrag)
        {
            isDragging = true;
        }

        if (Event.current.type == EventType.MouseUp)
        {
            isDragging = false;
        }

        GUI.DragWindow(); //Allows dragging the GUI window with mouse
    }
    // Dynamically calculate the window's height depending on
    // The number of toggles & group expansion
    private int CalculateWindowHeight()
    {
        int totalHeight = 70; // Base height for the window
        int groupHeight = 50; // Height for each group title
        int toggleHeight = 30; // Height for each toggle
        int submenuHeight = 40; // Height for each submenu title

        foreach (GroupInfo group in groups)
        {
            totalHeight += groupHeight; // Always add height for the group title

            if (group.isExpanded)
            {
                totalHeight += group.toggles.Count * toggleHeight; // Add height for toggles in the group

                foreach (SubmenuInfo submenu in group.submenus)
                {
                    totalHeight += submenuHeight; // Always add height for the submenu title

                    if (submenu.isExpanded)
                    {
                        totalHeight += submenu.toggles.Count * toggleHeight; // Add height for toggles in the expanded submenu
                    }
                }
            }
        }
        return totalHeight;
    }
    // Closes all expanded groups other than indexToKeepOpen
    private void CloseAllGroupsExcept(int indexToKeepOpen)
    {
        for (int i = 0; i < groups.Count; i++)
        {
            if (i != indexToKeepOpen)
            {
                GroupInfo group = groups[i];
                group.isExpanded = false;
                groups[i] = group;
            }
        }
    }

    private void CloseAllSubmenusExcept(GroupInfo group, int submenuIndexToKeepOpen)
    {
        for (int i = 0; i < group.submenus.Count; i++)
        {
            if (i != submenuIndexToKeepOpen)
            {
                var submenu = group.submenus[i];
                submenu.isExpanded = false;
                group.submenus[i] = submenu;
            }
        }
    }
}