using System.Collections.Generic;
using UnityEngine;

public class WordChecker : MonoBehaviour
{
    public static WordChecker Instance;

    public WordList wordList;  // 通过 Inspector 赋值 ScriptableObject

    private void Awake()
    {
        Instance = this;
    }

    // 判断单词是否存在于 words 集合中
    public bool IsWordInList(string word)
    {
        // 使用 Words 属性来访问 HashSet<string>
        return wordList.Words.Contains(word);
    }

    // 调用示例
    private void Start()
    {

    }
}
