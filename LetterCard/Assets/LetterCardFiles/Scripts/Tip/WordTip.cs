using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WordTip : MonoBehaviour
{
    [SerializeField]private GameObject tipWordPrefab;
    [SerializeField] private Transform tipWordParent;
    [SerializeField] private WordList sixWordList;
    [SerializeField] private ScrollRect scrollRect;


    private void OnEnable()
    {
        List<char> availableLetters = DeckManager.instance.letterHandCards.OfType<LetterCard>()               // 安全转换为 LetterCard 类型
                                        .Select(card => card.Letter)        // 提取 Letter 属性
                                        .ToList();                          // 转为 List<char>
        List<string> matched = WordFinder.instance.FindAllWords(availableLetters, sixWordList.Words);

        for (int i = 0; i < matched.Count; i++)
        {
             Instantiate(tipWordPrefab, tipWordParent).transform.GetChild(0).GetComponent<Text>().text=matched[i];
        }
        scrollRect.verticalNormalizedPosition = 1f;
    }

    private void OnDisable()
    {
        foreach (Transform child in tipWordParent)
        {
            Destroy(child.gameObject);
        }
    }
}
