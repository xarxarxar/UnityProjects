using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 随机选择英雄
/// </summary>
public class RandomChoose : MonoBehaviour
{
    public List<BaseRole> baseRoles=new List<BaseRole>();
    public GameObject roleChooseUI;
    public Transform roleChooseUIParent;

    private List<GameObject> roleChooseUIs=new List<GameObject>();
    private void Start()
    {
        for (int i = 0; i < baseRoles.Count; i++)
        {
            roleChooseUIs.Add(Instantiate(roleChooseUI, roleChooseUIParent));
            roleChooseUIs[i].transform.GetChild(0).gameObject.SetActive(false);
            roleChooseUIs[i].transform.GetChild(1).GetComponent<Image>().sprite = baseRoles[i].roleImage.sprite;
        }

        List<int> randomNumbers= GetRandomUniqueNumbers(baseRoles.Count,3);
        for (int i = 0;i < randomNumbers.Count;i++)
        {
            Debug.Log($"抽取{randomNumbers[i]}" );
        }


        StartCoroutine(ChooseCoro(randomNumbers));
    }

    IEnumerator ChooseCoro(List<int> randomNumbers)
    {
        for (int i = 0; i < randomNumbers.Count; i++)
        {
            GameObject lastObject = null;
            for (int j = 0; j <= roleChooseUIs.Count*2+ randomNumbers[i]; j++)
            {
                yield return new WaitForSeconds(0.1f);
                roleChooseUIs[j% roleChooseUIs.Count].transform.GetChild(0).gameObject.SetActive(true);
                if (lastObject != null)
                {
                    lastObject.transform.GetChild(0).gameObject.SetActive(false);
                }
                lastObject = roleChooseUIs[j % roleChooseUIs.Count];
            }
            lastObject.transform.GetChild(0).gameObject.SetActive(false);
            roleChooseUIs[randomNumbers[i]].transform.GetChild(0).gameObject.SetActive(true);
            roleChooseUIs[randomNumbers[i]].transform.GetChild(1).GetComponent<Image>().color = Color.white;
            yield return new WaitForSeconds(2);
        }
    }



    /// <summary>
    /// x为范围，(0,x)，n为抽取的个数
    /// </summary>
    /// <param name="x"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public List<int> GetRandomUniqueNumbers(int x, int n)
    {
        if (n > x)
        {
            Debug.LogError("不能从比目标数量更小的范围中抽取不重复的数！");
            return null;
        }

        List<int> numbers = new List<int>();
        for (int i = 0; i < x; i++)
        {
            numbers.Add(i);
        }

        // 洗牌（Fisher–Yates Shuffle）
        for (int i = numbers.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = numbers[i];
            numbers[i] = numbers[j];
            numbers[j] = temp;
        }

        return numbers.GetRange(0, n);
    }
}
