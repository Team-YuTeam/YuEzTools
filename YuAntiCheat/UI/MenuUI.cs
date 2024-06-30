using UnityEngine;
using System.Collections.Generic;
using YuAntiCheat.Utils;
using HarmonyLib;

namespace YuAntiCheat.UI;
public class MenuUI : MonoBehaviour
{

    public List<GroupInfo> groups = new List<GroupInfo>();
    private bool isDragging = false;
    private Rect windowRect = new Rect(10, 10, 300, 500);
    private bool isGUIActive = false;
    private GUIStyle submenuButtonStyle;

    // Create all groups (buttons) and their toggles on start
    private void Start()
    {
        groups.Add(new GroupInfo("界面", false, new List<ToggleInfo>() {
            new ToggleInfo("深色界面", () => Toggles.DarkMode, x => Toggles.DarkMode = x),
            }, new List<SubmenuInfo> {
            new SubmenuInfo("Ping部分", false, new List<ToggleInfo>() {
                new ToggleInfo("显示commit号", () => Toggles.ShowCommit, x => Toggles.ShowCommit = x),
                new ToggleInfo("显示模组标语", () => Toggles.ShowModText, x => Toggles.ShowModText = x),
                new ToggleInfo("显示是否安全模式状态", () => Toggles.ShowIsSafe, x => Toggles.ShowIsSafe = x),
                new ToggleInfo("显示安全模式提示语", () => Toggles.ShowSafeText, x => Toggles.ShowSafeText = x),
                new ToggleInfo("显示是否深色模式状态", () => Toggles.ShowIsDark, x => Toggles.ShowIsDark = x),
                new ToggleInfo("显示Ping", () => Toggles.ShowPing, x => Toggles.ShowPing = x),
                new ToggleInfo("显示FPS", () => Toggles.ShowFPS, x => Toggles.ShowFPS = x),
            }),
            }
        ));
        groups.Add(new GroupInfo("反作弊", false, new List<ToggleInfo>() {
                new ToggleInfo("安全模式", () => Toggles.SafeMode, x => Toggles.SafeMode = x),
            }, new List<SubmenuInfo> {
                
            }
        ));
        groups.Add(new GroupInfo("快捷按钮", false, new List<ToggleInfo>() {
                new ToggleInfo("输出日志", () => Toggles.DumpLog, x => Toggles.DumpLog = x),
                new ToggleInfo("打开游戏运行目录", () => Toggles.OpenGameDic, x => Toggles.OpenGameDic = x),
            }, new List<SubmenuInfo> {
                new SubmenuInfo("仅房主", false, new List<ToggleInfo>() {
                    new ToggleInfo("更改倒计时为0", () => Toggles.ChangeDownTimerToZero, x => Toggles.ChangeDownTimerToZero = x),
                    new ToggleInfo("恶搞倒计时", () => Toggles.ChangeDownTimerTo114514, x => Toggles.ChangeDownTimerTo114514 = x),
                    new ToggleInfo("取消倒计时", () => Toggles.AbolishDownTimer, x => Toggles.AbolishDownTimer = x),
                }),
            }
        ));
        groups.Add(new GroupInfo("其他功能", false, new List<ToggleInfo>() {
                new ToggleInfo("突破FPS上限", () => Toggles.FPSPlus, x => Toggles.FPSPlus = x),
            }, new List<SubmenuInfo> {
                
                
            }
        ));
    }
    
    private void Update(){

        if (Input.GetKeyDown(Utils.Utils.stringToKeycode(Main.menuKeybind.Value)))
        {
            //Enable-disable GUI with DELETE key
            isGUIActive = !isGUIActive;

            //Also teleport the window to the mouse for immediate use
            Vector2 mousePosition = Input.mousePosition;
            windowRect.position = new Vector2(mousePosition.x, Screen.height - mousePosition.y);
        }
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

        Color uiColor;

        string configHtmlColor = Main.menuHtmlColor.Value;

        if (!ColorUtility.TryParseHtmlString(configHtmlColor, out uiColor))
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

            if (GUI.Button(new Rect(10, currentYPosition, 280, 40), group.name))
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
                    bool newState = GUI.Toggle(new Rect(20, currentYPosition, 260, 30), currentState, toggle.label);
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
                    if (GUI.Button(new Rect(20, currentYPosition, 260, 30), submenu.name, submenuButtonStyle))
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
                            bool newState = GUI.Toggle(new Rect(30, currentYPosition, 250, 30), currentState, toggle.label);
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
