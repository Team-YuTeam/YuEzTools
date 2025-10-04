using YuEzTools.Attributes;
using UnityEngine;

namespace YuEzTools.Patches;

public static class ServerAddManager
{
    private static ServerManager serverManager = DestroyableSingleton<ServerManager>.Instance;

    [PluginModuleInitializer]
    public static void Init()
    {
        serverManager.AvailableRegions = ServerManager.DefaultRegions;
        List<IRegionInfo> regionInfos =
        [
            CreateHttp("au-us.233466.xyz", "Nikocat233(US)", 443, true),
            CreateHttp("aucn.233466.xyz", "Nikocat233(CN)", 443, true),
            CreateHttp("newplayer.fangkuai.fun", "FangKuai", 443, true),
            CreateHttp("ah.rainplay.cn", "XiaoLu", 50751, false),
            CreateHttp("au-as.duikbo.at", "Modded Asia (MAS)", 443, true),
            CreateHttp("www.aumods.xyz", "Modded NA (MNA)", 443, true),
            CreateHttp("au-eu.duikbo.at", "Modded EU (MEU)", 443, true),
        ];

        var defaultRegion = serverManager.CurrentRegion;
        regionInfos.Where(x => !serverManager.AvailableRegions.Contains(x)).Do(serverManager.AddOrUpdateRegion);
        serverManager.SetRegion(defaultRegion);

        SetServerName(defaultRegion.Name);
    }
    public static void SetServerName(string serverName = "")
    {
        if (serverName == "") serverName = ServerManager.Instance.CurrentRegion.Name;
        var name = serverName;

        Color32 color = serverName switch
        {
            "Asia" => new(58, 166, 117, 255),
            "Europe" => new(58, 166, 117, 255),
            "North America" => new(58, 166, 117, 255),
            "Modded Asia (MAS)" => new(255, 132, 0, 255),
            "Modded NA (MNA)" => new(255, 132, 0, 255),
            "Modded EU (MEU)" => new(255, 132, 0, 255),
            "FangKuai" => new(105, 105, 193, 255),
            "Nikocat233(CN)" => new(255, 255, 0, 255),
            "Nikocat233(US)" => new(255, 255, 0, 255),
            "XiaoLu" => new(255, 151, 255, 255),

            _ => new(255, 255, 255, 255),
        };
        PingTrackerUpdatePatch.ServerName = Utils.Utils.ColorString(color, name);
        InnerNetClientSpawnPatch.serverName = name;
    }

    public static IRegionInfo CreateHttp(string ip, string name, ushort port, bool ishttps)
    {
        string serverIp = (ishttps ? "https://" : "http://") + ip;
        ServerInfo serverInfo = new(name, serverIp, port, false);
        ServerInfo[] ServerInfo = [serverInfo];
        return new StaticHttpRegionInfo(name, (StringNames)1003, ip, ServerInfo).Cast<IRegionInfo>();
    }
}