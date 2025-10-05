using UnityEngine;
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
    private static void Start() { }

    private void Update()
    {

        if (Input.GetKeyDown(Utils.Utils.stringToKeycode(Main.menuKeybind.Value)))
        {
            //Enable-disable GUI with DELETE key
            isGUIActive = !isGUIActive;

            //Also teleport the window to the mouse for immediate use
            Vector2 mousePosition = Input.mousePosition;
            windowRect.position = new Vector2(mousePosition.x, Screen.height - mousePosition.y);
        }

        if (GetPlayer.IsPlayer)
        {
            Toggles.ServerAllHostOrNoHost = GameStartManagerPatch.roomMode == RoomMode.Normal ? false : true;
            Toggles.EnableAntiCheat = GameStartManagerPatch.EnableAC;
        }

        Main.WinTextSize.Value = Toggles.WinTextSize;

        if (isGUIActive) firstoOpenMenuUI = false;
        if (!isGUIActive)
        {
            groups.Clear();
            groups.Add(new GroupInfo(GetString("MenuUI.AntiCheat"), false,
                [
                    new(GetString("MenuUI.EnableAntiCheat"), () => Toggles.EnableAntiCheat,
                        x => Toggles.EnableAntiCheat = x),
                    new(GetString("MenuUI.AutoExit"), () => Toggles.AutoExit,
                        x => Toggles.AutoExit = x),
                    new(GetString("MenuUI.KickNotLogin"), () => Toggles.KickNotLogin,
                        x => Toggles.KickNotLogin = x),
                ],
                [
                    new(GetString("MenuUI.AntiCheat.ModMode"), false,
                        [
                            new(GetString("MenuUI.ServerAllHostOrNoHost"),
                                () => Toggles.ServerAllHostOrNoHost, x => Toggles.ServerAllHostOrNoHost = x),
                            new(GetString("MenuUI.SafeMode"), () => Toggles.SafeMode,
                                x => Toggles.SafeMode = x),
                        ]),

                ]
            ));
            groups.Add(new GroupInfo(GetString("Interface"), false,
                [
                    new(GetString("DarkUI"), () => Toggles.DarkMode, x => Toggles.DarkMode = x),
                ],
                [
                    new(GetString("PingPart"), false,
                    [
                        new(GetString("ShowCommit"), () => Toggles.ShowCommit,
                            x => Toggles.ShowCommit = x),
                        new(GetString("ShowModText"), () => Toggles.ShowModText,
                            x => Toggles.ShowModText = x),
                        new(GetString("ShowIsSafe"), () => Toggles.ShowIsSafe,
                            x => Toggles.ShowIsSafe = x),
                        new(GetString("ShowIsDark"), () => Toggles.ShowIsDark,
                            x => Toggles.ShowIsDark = x),
                        new(GetString("ShowPing"), () => Toggles.ShowPing,
                            x => Toggles.ShowPing = x),
                        new(GetString("ShowFPS"), () => Toggles.ShowFPS,
                            x => Toggles.ShowFPS = x),
                        new(GetString("ShowServer"), () => Toggles.ShowServer,
                            x => Toggles.ShowServer = x),
                        new(GetString("ShowRoomTime"), () => Toggles.ShowRoomTime,
                            x => Toggles.ShowRoomTime = x),
                        new(GetString("ShowIsAutoExit"), () => Toggles.ShowIsAutoExit,
                            x => Toggles.ShowIsAutoExit = x),
                        // new ToggleInfo(Translator.GetString("ShowLocalNowTime"), () => Toggles.ShowLocalNowTime,
                        //     x => Toggles.ShowLocalNowTime = x),
                        // new ToggleInfo(Translator.GetString("ShowUTC"), () => Toggles.ShowUTC,
                        //     x => Toggles.ShowUTC = x),
                        new(GetString("ShowGM"), () => Toggles.ShowGM,
                            x => Toggles.ShowGM = x),
                    ]),
                    new(GetString("EndPart"), false,
                        [
                            new(GetString("WinTextSize"), () => Toggles.WinTextSize,
                                x => Toggles.WinTextSize = x),
                        ]),
                ]
            ));
            groups.Add(new GroupInfo(GetString("MenuUI.ShortcutButton"), false,
                [
                    new(GetString("MenuUI.DumpLog"), () => Toggles.DumpLog,
                        x => Toggles.DumpLog = x),
                    new(GetString("MenuUI.OpenGameDic"), () => Toggles.OpenGameDic,
                        x => Toggles.OpenGameDic = x),
                    new(GetString("MenuUI.CloseMusicOfOr"), () => Toggles.CloseMusicOfOr,
                        x => Toggles.CloseMusicOfOr = x),
                    new(GetString("MenuUI.reShowRoleT"), () => Toggles.reShowRoleT,
                            x => Toggles.reShowRoleT = x),
                    new(GetString("MenuUI.ShowInfoInLobby"), () => Toggles.ShowInfoInLobby,
                            x => Toggles.ShowInfoInLobby = x),

                ],
                [
                    new(GetString("MenuUI.ShortcutButton.Left"), false,
                    [
                        new(GetString("MenuUI.ExitGame"), () => Toggles.ExitGame,
                            x => Toggles.ExitGame = x),
                        new(GetString("MenuUI.RealBan"), () => Toggles.RealBan,
                            x => Toggles.RealBan = x),
                    ]),
                    new(GetString("MenuUI.ShortcutButton.PeopleMode"), false,
                    [
                        new(GetString("MenuUI.HorseMode"), () => Toggles.HorseMode,
                            x => Toggles.HorseMode = x),
                        new(GetString("MenuUI.LongMode"), () => Toggles.LongMode,
                            x => Toggles.LongMode = x),
                    ]),
                    new(GetString("MenuUI.ShortcutButton.OnlyHost"), false,
                        [
                            new(GetString("MenuUI.ChangeDownTimerToZero"),
                                () => Toggles.ChangeDownTimerToZero, x => Toggles.ChangeDownTimerToZero = x),
                            new(GetString("MenuUI.ChangeDownTimerTo114514"),
                                () => Toggles.ChangeDownTimerTo114514, x => Toggles.ChangeDownTimerTo114514 = x),
                            new(GetString("MenuUI.AutoStartGame"), () => Toggles.AutoStartGame,
                                x => Toggles.AutoStartGame = x),
                            new(GetString("MenuUI.AbolishDownTimer"),
                                () => Toggles.AbolishDownTimer, x => Toggles.AbolishDownTimer = x),
                        ]),
                ]
            ));
            groups.Add(new GroupInfo(GetString("MenuUI.Other"), false,
                [
                    new(GetString("MenuUI.FPSPlus"), () => Toggles.FPSPlus,
                        x => Toggles.FPSPlus = x),
                ], []
            ));
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

    private static void CloseAllSubmenusExcept(GroupInfo group, int submenuIndexToKeepOpen)
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