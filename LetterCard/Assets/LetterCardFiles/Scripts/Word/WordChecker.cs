using System.Collections.Generic;
using UnityEngine;

public class WordChecker : MonoBehaviour
{
    public WordList wordList;  // 通过 Inspector 赋值 ScriptableObject

    // 判断单词是否存在于 words 集合中
    public bool IsWordInList(string word)
    {
        // 使用 Words 属性来访问 HashSet<string>
        return wordList.Words.Contains(word);
    }

    // 调用示例
    private void Start()
    {
        // 要检查的单词
        string wordToCheck = "abandon";
        // 判断单词是否存在
        bool result = IsWordInList(wordToCheck);

        // 打印结果
        Debug.Log(result ? "Word found!" : "Word not found.");
    }
}
