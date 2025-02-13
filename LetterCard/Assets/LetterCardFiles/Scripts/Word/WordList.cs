using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WordList", menuName = "Game Config/WordList", order = 1)]
public class WordList : ScriptableObject
{
    // 使用 List<string> 代替 HashSet<string> 进行序列化
    [SerializeField]
    private List<string> serializedWords = new List<string>();

    private HashSet<string> words;

    // 初始化时将 List 转换为 HashSet
    public HashSet<string> Words
    {
        get
        {
            if (words == null)
            {
                words = new HashSet<string>(serializedWords);
            }
            return words;
        }
    }

    // 将 HashSet 转回 List 并存储
    public void Save()
    {
        serializedWords = new List<string>(words);
    }
}
