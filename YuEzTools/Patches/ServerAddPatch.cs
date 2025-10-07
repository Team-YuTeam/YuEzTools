using YuEzTools.Attributes;
using UnityEngine;

namespace YuEzTools.Patches;

public static class ServerAddManager
{
    #if Windows
    private static ServerManager serverManager = DestroyableSingleton<ServerManager>.Instance;
    
    [PluginModuleInitializer]
    public static void Init()
    {
        serverManager.AvailableRegions = ServerManager.DefaultRegions;
        List<IRegionInfo> regionInfos =
        [
            // Niko
            CreateHttp("au-eu.niko233.me", "Niko233(EU)", 443, true, new Color32(251, 200, 200,255)),
            CreateHttp("au-us.niko233.me", "Niko233(NA)", 443, true, new Color32(251, 200, 200,255)),
            CreateHttp("au-as.niko233.me", "Niko233(AS)", 443, true, new Color32(251, 200, 200,255)),
            // Modded
            CreateHttp("au-as.duikbo.at", "Modded Asia (MAS)", 443, true, new Color32(251, 226, 200,255)),
            CreateHttp("aumods.org", "Modded NA (MNA)", 443, true, new Color32(251, 226, 200,255)),
            CreateHttp("au-eu.duikbo.at", "Modded EU (MEU)", 443, true, new Color32(251, 226, 200,255)),
            // Qingfeng
            CreateHttp("bj.server.qingfengawa.top", "QingFeng(Beijing)", 443, true, new Color32(249, 255, 170,255)),
            CreateHttp("sh.server.qingfengawa.top", "QingFeng(Shanghai)", 443, true,new Color32(249, 255, 170,255)),
            CreateHttp("nb.server.qingfengawa.top", "QingFeng(Ningbo)", 443, true, new Color32(249, 255, 170,255)),
            CreateHttp("gz.server.qingfengawa.top", "QingFeng(Guangzhou)", 443, true, new Color32(249, 255, 170,255)),
            // Fanchuan
            CreateHttp("gz.fcaugame.cn", "Fanchuan(Guangzhou)", 443, true, new Color32(207, 255, 170,255)),
            CreateHttp("zxc.lcayun.cn", "Fanchuan(Zaozhuang)", 443, true, new Color32(207, 255, 170,255)),
            // Tianmeng
            // CreateHttp("139.224.74.5", "Tianmeng(New)", 443, true),
            // Hedianzhan
            CreateHttp("aunpp.cn", "Hedianzhan(Shanghai)", 443, true, new Color32(170, 255, 186,255)),
            CreateHttp("nb.aunpp.cn", "Hedianzhan(Ningbo)", 443, true, new Color32(170, 255, 186,255)),
            // Xiaohei
            CreateHttp("zlxhimp.amongusclub.cn", "Xiaohei(Ningbo)", 443, true, new Color32(170, 255, 241,255)),
            // Fangkuai
            CreateHttp("player.amongusclub.cn", "Fangkuai(Suqian,NoS)", 443, true, new Color32(170, 255, 241,255)),
            // NoS
            CreateHttp("www.nebula-on-the-ship.com", "NebulaOnTheShip(Japan)", 443, true, new Color32(170, 220, 255,255)),
        ];
        
        var defaultRegion = serverManager.CurrentRegion;
        regionInfos.Where(x => !serverManager.AvailableRegions.Contains(x)).Do(serverManager.AddOrUpdateRegion);
        serverManager.SetRegion(defaultRegion);

        SetServerName(defaultRegion.Name);
    }
#endif
    
    private static Dictionary<string, Color32> serverColor32Map = new Dictionary<string, Color32>
    {
        {"Asia", new Color32(58, 166, 117, 255)},
        {"Europe", new Color32(58, 166, 117, 255)},
        {"North America", new Color32(58, 166, 117, 255)},
    };
    
    public static void SetServerName(string serverName = "")
    {
        if (serverName == "") serverName = ServerManager.Instance.CurrentRegion.Name;
        var name = serverName;

        Color32 color32 = GetServerColor32(name);

        // Color32 color = serverName switch
        // {
        //     "Asia" => new(58, 166, 117, 255),
        //     "Europe" => new(58, 166, 117, 255),
        //     "North America" => new(58, 166, 117, 255),
        //     "Modded Asia (MAS)" => new(255, 132, 0, 255),
        //     "Modded NA (MNA)" => new(255, 132, 0, 255),
        //     "Modded EU (MEU)" => new(255, 132, 0, 255),
        //     "FangKuai" => new(105, 105, 193, 255),
        //     "Nikocat233(CN)" => new(255, 255, 0, 255),
        //     "Nikocat233(US)" => new(255, 255, 0, 255),
        //     "XiaoLu" => new(255, 151, 255, 255),
        //
        //     _ => new(255, 255, 255, 255),
        // };
        PingTrackerUpdatePatch.ServerName = Utils.Utils.ColorString(color32, GetString(name));
        InnerNetClientSpawnPatch.serverName = name;
    }

    public static IRegionInfo CreateHttp(string ip, string name, ushort port, bool ishttps, Color32? color = null)
    {
        Color32 serverColor32 = color ?? new Color32(255, 255, 255, 255);
        serverColor32Map.Add(name,serverColor32);
        
        string serverIp = (ishttps ? "https://" : "http://") + ip;
        ServerInfo serverInfo = new(name, serverIp, port, false);
        ServerInfo[] ServerInfo = [serverInfo];
        return new StaticHttpRegionInfo(name, (StringNames)1003, ip, ServerInfo).Cast<IRegionInfo>();
    }

    private static readonly Color32 _defaultColor = new Color32(255, 255, 255, 255);
    
    public static Color32 GetServerColor32(string name)
    {
        if (serverColor32Map.TryGetValue(name, out var colorData))
        {
            return colorData;
        }
        
        return _defaultColor;
    }
}