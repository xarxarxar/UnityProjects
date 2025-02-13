using UnityEditor;
using UnityEngine;
using System.IO;

[CustomEditor(typeof(WordList))]
public class WordListEditor : Editor
{
    private WordList wordList;

    private void OnEnable()
    {
        wordList = (WordList)target;
    }

    public override void OnInspectorGUI()
    {
        // 默认的 Inspector 界面
        DrawDefaultInspector();

        // 使用按钮来添加单词
        if (GUILayout.Button("Add Words from CSV"))
        {
            string path = EditorUtility.OpenFilePanel("Choose CSV File", "", "csv");
            if (!string.IsNullOrEmpty(path))
            {
                AddWordsFromCSV(path); // 读取 CSV 文件并添加单词
                EditorUtility.SetDirty(wordList); // 标记对象已被修改
            }
        }

        // 显示所有现有单词
        GUILayout.Label("Existing Words:");
        foreach (var word in wordList.Words)
        {
            GUILayout.Label(word); // 显示当前列表中的所有单词
        }

        // 更新 Inspector
        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// 从 CSV 文件中读取单词并添加到 HashSet 中
    /// </summary>
    void AddWordsFromCSV(string filePath)
    {
        try
        {
            string[] lines = File.ReadAllLines(filePath);  // 读取 CSV 文件的每一行
            foreach (var line in lines)
            {
                string word = line.Trim();  // 去除多余的空格
                if (!string.IsNullOrEmpty(word) && !wordList.Words.Contains(word))
                {
                    wordList.Words.Add(word);  // 添加到 HashSet 中
                }
            }
            wordList.Save();  // 保存修改后的 HashSet
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error reading CSV file: " + e.Message);
        }
    }
}
