using System;
using System.Text;
using UnityEngine;

namespace YuEzTools.Helpers;

public static class StringHelper
{
    public static readonly Encoding shiftJIS = CodePagesEncodingProvider.Instance.GetEncoding("Shift_JIS");

    /// <summary>
    /// 计算使用SJIS编码时的字节数
    /// </summary>
    public static int GetByteCount(this string self) => shiftJIS.GetByteCount(self);

    public static bool Has(this string s,string h)
    {
         return s.Contains(h, StringComparison.OrdinalIgnoreCase);
    }
}
public class ColorGradient
{
    // thanks tonx & tonex
    public List<Color> Colors { get; private set; }
    private readonly float Spacing;
    public ColorGradient(params Color[] colors)
    {
        Colors = [.. colors];
        Spacing = 1f / (Colors.Count - 1);
    }
    public bool IsValid => Colors.Count >= 2;
    public string Apply(string input)
    {
        if (input.Length == 0) return input;
        if (input.Length == 1) return Utils.Utils.ColorString(Colors[0], input);
        float step = 1f / (input.Length - 1);
        StringBuilder sb = new();
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            var color = Evaluate(step * i);
            sb.Append(Utils.Utils.ColorString(color, c.ToString()));
        }
        return sb.ToString();
    }
    public Color Evaluate(float percent)
    {
        if (percent > 1) percent = 1;
        int indexLow = Mathf.FloorToInt(percent / Spacing);
        if (indexLow >= Colors.Count - 1) return Colors[^1];
        int indexHigh = indexLow + 1;
        float percentClamp = (Colors.Count - 1) * (percent - indexLow * Spacing);

        Color colorA = Colors[indexLow];
        Color colorB = Colors[indexHigh];

        float r = colorA.r + percentClamp * (colorB.r - colorA.r);
        float g = colorA.g + percentClamp * (colorB.g - colorA.g);
        float b = colorA.b + percentClamp * (colorB.b - colorA.b);

        return new Color(r, g, b);
    }
    public class Component
    {
        public float? SizePercentage { get; set; }
        public string Text { get; set; }
        public Color32? TextColor { get; set; }
        public ColorGradient Gradient { get; set; }
        public bool Spaced { get; set; } = true;
        public string Generate(bool applySpace = true, bool applySize = true)
        {
            if (Text == null) return "";
            var text = Text;
            if (Gradient != null && Gradient.IsValid) text = Gradient.Apply(text);
            else if (TextColor != null) text = Utils.Utils.ColorString(TextColor.Value, text);
            if (Spaced && applySpace) text = " " + text + " ";
            if (SizePercentage != null && applySize) text = $"<size={SizePercentage}%>{text}</size>";
            return text;
        }
    }
}