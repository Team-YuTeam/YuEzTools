using Microsoft.Win32;
using System;
using System.IO;

namespace YuEzTools.Modules;

# pragma warning disable CA1416
public static class RegistryManager
{
    #if Windows // 实验性修改 如果有问题请删除 Main.cs也有此代码 请一并删除
    public static RegistryKey SoftwareKeys => Registry.CurrentUser.OpenSubKey("Software", true);
    public static RegistryKey Keys = SoftwareKeys.OpenSubKey("AU-YuET", true);
    public static Version LastVersion;

    public static void Init()
    {
        if (Keys == null)
        {
            Info("Create YuET Registry Key", "Registry Manager");
            Keys = SoftwareKeys.CreateSubKey("AU-YuET", true);
        }
        if (Keys == null)
        {
            Error("Create Registry Failed", "Registry Manager");
            return;
        }

        if (Keys.GetValue("Last launched version") is not string regLastVersion)
            LastVersion = new Version(0, 0, 0);
        else LastVersion = Version.Parse(regLastVersion);

        Keys.SetValue("Last launched version", Main.version.ToString());
        Keys.SetValue("Path", Path.GetFullPath("./"));
    }
#endif
}