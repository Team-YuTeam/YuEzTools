namespace YuEzTools.Modules;

public class DevUser(string code = "", string color = "null", string tag = "null", bool isUp = false, bool isDev = false, bool isSpo = false, bool deBug = false, string upName = "未认证用户", string jobs = "NotJob", string name = "null")
{
    public string Code { get; set; } = code;
    public string Color { get; set; } = color;
    public string Tag { get; set; } = tag;
    public bool IsUp { get; set; } = isUp;
    public bool IsSpo { get; set; } = isSpo;
    public bool IsDev { get; set; } = isDev;
    public bool DeBug { get; set; } = deBug;
    public string UpName { get; set; } = upName;
    public string Jobs { get; set; } = jobs;
    public string Name { get; set; } = name;

    public string GetTag() => Color == "null" ? $"<size=1.7>{Tag}</size>\r\n" : $"<color={Color}><size=1.7>{Tag}</size></color>\r\n";
}

public static class DevManager
{
    public static DevUser DefaultDevUser = new();
    public static List<DevUser> DevUserList = [];
    public static void Init()
    {
        // Dev
        DevUserList.Add(new(code: "storeroan#0331", color: "#49FFA5", tag: "Yu #Dev", isUp: true, isDev: true, isSpo: false, deBug: true, upName: "烟Yu中", jobs: "MainDev", name: "Yu"));
        DevUserList.Add(new(code: "coltposh#9009", color: "#0090f4", tag: "Mousse #Dev", isUp: false, isDev: true, isSpo: false, deBug: true, upName: "null", jobs: "Developer", name: "Mousse"));
        // Trans
        DevUserList.Add(new(code: "morechoice#4224", color: "#FF6666", tag: "redphantom1000 #Trans", isUp: false, isDev: false, isSpo: false, deBug: false, upName: "null", jobs: "EnTranslator", name: "redphantom1000"));
        DevUserList.Add(new(code: "hoppypuree#2528", color: "#00FFFF", tag: "神金驾到 #Trans", isUp: false, isDev: false, isSpo: false, deBug: false, upName: "null", jobs: "JpTranslator", name: "Miaoice"));
        // Helper
        DevUserList.Add(new(code: "actorour#0029", color: "#ffc0cb", tag: "#Dev", isUp: true, isDev: false, isSpo: false, deBug: true, upName: "KARPED1EM", jobs: "NotJob", name: "KARPED1EM"));
    }
    public static bool IsDevUser(this string code) => DevUserList.Any(x => x.Code == code);
    public static DevUser GetDevUser(this string code) => code.IsDevUser() ? DevUserList.Find(x => x.Code == code) : DefaultDevUser;
    public static string GetDevJob(this string code) => GetString(code.GetDevUser().Jobs);
    public static bool IsSpoUser(this string code) => code.IsDevUser() && code.GetDevUser().IsSpo;
}
