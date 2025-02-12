using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class CacheText : MonoBehaviour
{
    private static Text textComponent;
    // 使用字典映射 ColorType 到 Color
    static Dictionary<ColorType, Color32> colorMap = new Dictionary<ColorType, Color32>
        {
            { ColorType.Red, new Color32(194,24,91,255) },
            { ColorType.Green, new Color32(56,142,60,255) },
            { ColorType.Blue, new Color32(48,63,159,255) },
            { ColorType.Yellow, new Color32(255,162,0,255) }
        };
    // 用来存储最终的文本内容
    private static StringBuilder textBuilder = new StringBuilder();
    // Start is called before the first frame update
    void OnEnable()
    {
        textComponent = GetComponent<Text>();
    }

    // 方法：添加字符并设置颜色
    public static void AddCharacterWithColor(ColorType colorType, char character)
    {
        // 获取颜色
        Color32 color = colorMap[colorType];

        // 构建富文本字符串，指定颜色
        textBuilder.AppendFormat("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>",
            color.r, color.g, color.b, character);

        // 更新Text组件的文本内容
        textComponent.text = textBuilder.ToString();
    }

    // 方法：移除指定颜色类型的字符
    public static void RemoveCharacterWithColor(ColorType colorType, char character)
    {
        // 获取颜色
        Color32 color = colorMap[colorType];

        // 创建一个模式来匹配包含特定颜色的字符
        string pattern = string.Format(@"<color=#{0:X2}{1:X2}{2:X2}>{3}</color>",
            color.r, color.g, color.b, character);

        // 使用正则表达式从StringBuilder中移除匹配的内容
        string updatedText = Regex.Replace(textBuilder.ToString(), pattern, string.Empty);

        // 更新文本内容
        textBuilder.Clear();
        textBuilder.Append(updatedText);
        textComponent.text = textBuilder.ToString();
    }
}
