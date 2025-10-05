using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.IO;
using System.Text;
using YamlDotNet.RepresentationModel;
using YuEzTools.Attributes;
using YuEzTools.Helpers;

namespace YuEzTools.Modules;

public static class Translator
{
    public static Dictionary<int, Dictionary<string, string>> translateMaps = new();
    public const string LANGUAGE_FOLDER_NAME = "Language";

    [PluginModuleInitializer]
    public static void Init()
    {
        Info("加载语言文件...", "Translator");
        LoadLangs();
        Info("加载语言文件成功", "Translator");
    }
    public static void LoadLangs()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fileNames = assembly.GetManifestResourceNames().Where(x => x.StartsWith($"YuEzTools.Resources.Languages."));
        foreach (var fileName in fileNames)
        {
            Msg("This fileName:" + fileName, "DebugTestYuET");
            var yaml = new YamlStream();
            var stream = assembly.GetManifestResourceStream(fileName);
            yaml.Load(new StringReader(new StreamReader(stream).ReadToEnd()));
            var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

            int langId = -1;
            var dic = new Dictionary<string, string>();

            foreach (var entry in mapping.Children)
            {
                (string key, string value) = (((YamlScalarNode)entry.Key).Value, ((YamlScalarNode)entry.Value).Value);

                if (key == "LangID")
                {
                    langId = int.Parse(value);
                    continue;
                }

                if (!dic.TryAdd(key, value))
                    Warn($"翻译文件 [{fileName}] 出现重复字符串 => {key} / {value}", "Translator");
            }

            if (langId != -1)
            {
                translateMaps.Remove(langId);
                translateMaps.Add(langId, dic);
                Msg($"翻译文件 [{fileName}] 提供语言ID{langId}", "Translator");
            }
            else
                Error($"翻译文件 [{fileName}] 没有提供语言ID", "Translator");
        }

        // カスタム翻訳ファイルの読み込み
        if (!Directory.Exists(LANGUAGE_FOLDER_NAME)) Directory.CreateDirectory(LANGUAGE_FOLDER_NAME);

        // 翻訳テンプレートの作成
        CreateTemplateFile();
        foreach (var lang in EnumHelper.GetAllValues<SupportedLangs>())
        {
            if (File.Exists(@$"./{LANGUAGE_FOLDER_NAME}/{lang}.dat"))
                LoadCustomTranslation($"{lang}.dat", lang);
        }
    }

    public static string GetString(string s, Dictionary<string, string> replacementDic = null, bool console = false)
    {
        var langId = TranslationController.Instance?.currentLanguage?.languageID ?? GetUserLangByRegion();
        if (console) langId = SupportedLangs.SChinese;
        string str = GetString(s, langId);
        if (replacementDic != null)
            foreach (var rd in replacementDic)
                str = str.Replace(rd.Key, rd.Value);
        return str;
    }

    public static string GetString(string str, SupportedLangs langId)
    {
        var res = $"<NOT FOUND:{str}>";

        try
        {
            // 在当前语言中寻找翻译
            if (translateMaps[(int)langId].TryGetValue(str, out var trans))
                res = trans;
            // 繁中用户寻找简中翻译替代
            else if (langId is SupportedLangs.TChinese && translateMaps[(int)SupportedLangs.SChinese].TryGetValue(str, out trans))
                res = "*" + trans;
            // 非中文用户寻找英语翻译替代
            else if (langId is not SupportedLangs.English and not SupportedLangs.TChinese && translateMaps[(int)SupportedLangs.English].TryGetValue(str, out trans))
                res = "*" + trans;
            // 非中文用户寻找中文（原生）字符串替代
            else if (langId is not SupportedLangs.SChinese && translateMaps[(int)SupportedLangs.SChinese].TryGetValue(str, out trans))
                res = "*" + trans;
            // 在游戏自带的字符串中寻找
            else
            {
                var stringNames = EnumHelper.GetAllValues<StringNames>().Where(x => x.ToString() == str);
                if (stringNames != null && stringNames.Any())
                    res = GetString(stringNames.FirstOrDefault());
            }
            if (res == $"<NOT FOUND:{str}>")
                Info(res, "test");
        }
        catch (Exception Ex)
        {
            Fatal($"Error oucured at [{str}] in String.yaml", "Translator");
            Error("Here was the error:\n" + Ex, "Translator");
        }
        return res;
    }

    public static string GetString(StringNames stringName)
        => DestroyableSingleton<TranslationController>.Instance.GetString(stringName, new Il2CppReferenceArray<Il2CppSystem.Object>(0));
    public static string GetRoleString(string str, bool forUser = true)
    {
        var CurrentLanguage = TranslationController.Instance?.currentLanguage?.languageID ?? SupportedLangs.English;
        var lang = forUser ? CurrentLanguage : SupportedLangs.SChinese;

        return GetString(str, lang);
    }

    public static SupportedLangs GetUserLangByRegion()
    {
        try
        {
            //var name = TranslationController.Instance?.currentLanguage?.languageID;
            //Info("the id is "+TranslationController.Instance?.currentLanguage?.languageID, "Translator");
            return TranslationController.Instance?.currentLanguage?.languageID ?? SupportedLangs.English;
        }
        catch
        {
            return SupportedLangs.English;
        }
    }

    public static bool IsChineseUser => GetUserLangByRegion() == SupportedLangs.SChinese;
    public static bool IsChineseLanguageUser => GetUserLangByRegion() is SupportedLangs.SChinese or SupportedLangs.TChinese;
    public static void LoadCustomTranslation(string filename, SupportedLangs lang)
    {
        string path = @$"./{LANGUAGE_FOLDER_NAME}/{filename}";
        if (File.Exists(path))
        {
            Info($"加载自定义翻译文件：{filename}", "LoadCustomTranslation");
            using StreamReader sr = new(path, Encoding.GetEncoding("UTF-8"));
            string text;
            string[] tmp = Array.Empty<string>();
            while ((text = sr.ReadLine()) != null)
            {
                tmp = text.Split(":");
                if (tmp.Length > 1 && tmp[1] != "")
                {
                    try
                    {
                        translateMaps[(int)lang][tmp[0]] = tmp.Skip(1).Join(delimiter: ":").Replace("\\n", "\n").Replace("\\r", "\r");
                    }
                    catch (KeyNotFoundException)
                    {
                        Warn($"无效密钥：{tmp[0]}", "LoadCustomTranslation");
                    }
                }
            }
        }
        else
        {
            Error($"找不到自定义翻译文件：{filename}", "LoadCustomTranslation");
        }
    }

    private static void CreateTemplateFile()
    {
        var sb = new StringBuilder();
        foreach (var title in translateMaps) sb.Append($"{title.Key}:\n");
        File.WriteAllText(@$"./{LANGUAGE_FOLDER_NAME}/template.dat", sb.ToString());
    }

    public static void ExportCustomTranslation()
    {
        LoadLangs();
        var sb = new StringBuilder();
        var lang = TranslationController.Instance.currentLanguage.languageID;
        foreach (var kvp in translateMaps[13])
        {
            var text = kvp.Value;
            if (!translateMaps.ContainsKey((int)lang)) text = "";
            sb.Append($"{kvp.Key}:{text}\n");
        }
        File.WriteAllText(@$"./{LANGUAGE_FOLDER_NAME}/export_{lang}.dat", sb.ToString());
    }
}