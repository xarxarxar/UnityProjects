using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CacheText : MonoBehaviour
{
    private static Text textComponent;
    private static RectTransform background;
    // 使用字典映射 ColorType 到 Color
    static Dictionary<char, Color32> colorMap = new Dictionary<char, Color32>
        {
            { 'R', new Color32(194,24,91,255) },
            { 'G', new Color32(56,142,60,255) },
            { 'B', new Color32(48,63,159,255) },
            { 'Y', new Color32(255,162,0,255) }
        };
    // 用来存储最终的文本内容
    public static StringBuilder textBuilder = new StringBuilder();
    private static StringBuilder originalText = new StringBuilder();  // 用于存储最初的文本内容

    public LetterDisplay letterDisplayMode = LetterDisplay.None;

    /// <summary>
    /// 缓存Text的大小写显示
    /// </summary>
    public enum LetterDisplay
    {
        None,//保持原样
        CapitalLetter,//大写显示
        LowercaseLetter//小写显示
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        textComponent = transform.GetChild(1).GetComponent<Text>();
        background= transform.GetChild(0).GetComponent<RectTransform>();

        ImageAdapt();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            ToggleTextShow();
        }
    }

    // 方法：添加字符并设置颜色
    public static void AddCharacterWithColor(char colorType, char character)
    {
        // 获取颜色
        Color32 color = colorMap[colorType];

        // 构建富文本字符串，指定颜色
        textBuilder.AppendFormat("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>",
            color.r, color.g, color.b, character);
        originalText.AppendFormat("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>",
            color.r, color.g, color.b, character);//更新原始文本

        // 更新Text组件的文本内容
        textComponent.text = textBuilder.ToString();

        ImageAdapt();//适配背景
       
    }

    // 方法：移除指定颜色类型的字符
    public static void RemoveCharacterWithColor(char colorType, char character)
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
        originalText.Clear();
        originalText.Append(updatedText);//更新原始文本
        textComponent.text = textBuilder.ToString();

        ImageAdapt();//适配背景
        
    }

    /// <summary>
    /// 清空缓存文本
    /// </summary>
    public static void ClearTextShow()
    {
        // 更新文本内容
        textBuilder.Clear();
        originalText.Clear();
        textComponent.text = textBuilder.ToString();
        ImageAdapt();//适配背景
    }

    public void ToggleTextShow()
    {
        // 获取当前显示模式
        LetterDisplay currentMode = GetLetterDisplayMode();

        // 切换大小写模式
        if (currentMode == LetterDisplay.None)
        {
            // 如果当前模式是 None，则设置为 CapitalLetter（大写）
            SetLetterDisplayMode(LetterDisplay.CapitalLetter);
            UpdateTextToUppercase();
        }
        else if (currentMode == LetterDisplay.CapitalLetter)
        {
            // 如果当前模式是 CapitalLetter，则切换到 LowercaseLetter（小写）
            SetLetterDisplayMode(LetterDisplay.LowercaseLetter);
            UpdateTextToLowercase();
        }
        else if (currentMode == LetterDisplay.LowercaseLetter)
        {
            // 如果当前模式是 LowercaseLetter，则切换到 None（保持原样）
            SetLetterDisplayMode(LetterDisplay.None);
            RestoreOriginalText();
        }
    }

    /// <summary>
    /// 获取当前文字的显示模式（大小写）
    /// </summary>
    private LetterDisplay GetLetterDisplayMode()
    {
        // 获取当前文本显示模式，可以通过某种方式存储它
        // 在这里我们通过一个静态变量 `letterDisplayMode` 来存储当前模式
        // 你可以根据实际需求来实现这一功能
        // 这里是一个假设的实现
        return letterDisplayMode;  // 你可以根据实际需要更改为保存的状态
    }

    /// <summary>
    /// 设置当前文字的显示模式
    /// </summary>
    private void SetLetterDisplayMode(LetterDisplay mode)
    {
        // 设置文本显示模式
        // 你可以在这里保存状态，例如存储在某个静态字段中
        letterDisplayMode = mode;
    }

    /// <summary>
    /// 将文本转换为大写
    /// </summary>
    private void UpdateTextToUppercase()
    {
        // 遍历 textBuilder 中的字符，转换为大写
        for (int i = 0; i < textBuilder.Length; i++)
        {
            if (textBuilder[i] >= 'a' && textBuilder[i] <= 'z')
            {
                textBuilder[i] = char.ToUpper(textBuilder[i]);
            }
        }

        // 更新文本内容
        textComponent.text = textBuilder.ToString();

        // 适配背景图片
        ImageAdapt();
    }

    /// <summary>
    /// 将文本转换为小写
    /// </summary>
    private void UpdateTextToLowercase()
    {
        // 遍历 textBuilder 中的字符，转换为小写
        for (int i = 0; i < textBuilder.Length; i++)
        {
            if (textBuilder[i] >= 'A' && textBuilder[i] <= 'Z')
            {
                textBuilder[i] = char.ToLower(textBuilder[i]);
            }
        }

        // 更新文本内容
        textComponent.text = textBuilder.ToString();

        // 适配背景图片
        ImageAdapt();
    }

    /// <summary>
    /// 恢复原始的文本显示（保持原样）
    private void RestoreOriginalText()
    {
        // 恢复到原始文本内容
        textBuilder.Clear();
        textBuilder.Append(originalText);

        // 更新文本内容
        textComponent.text = textBuilder.ToString();

        // 适配背景图片
        ImageAdapt();
    }

    /// <summary>
    /// 文本背景的图片适配
    /// </summary>
    private static void ImageAdapt()
    {
        float textWidth = textComponent.preferredWidth==0?-50: textComponent.preferredWidth;
        float textHeight = textComponent.preferredHeight == 0 ? -50 : textComponent.preferredHeight;
        // 调整背景图片的大小
        background.sizeDelta = new Vector2(textWidth+50, textHeight+50); // 设置宽度，保持高度不变
    }
}
