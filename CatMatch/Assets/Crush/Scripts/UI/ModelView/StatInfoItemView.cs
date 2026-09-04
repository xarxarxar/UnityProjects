using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatInfoItemView : MonoBehaviour
{
    public Text statName;
    public Text statValue1;
    public Text statValues2;
    public Text percentTxt;

    public void SetUp(string name, string value1, string value2, string percent)
    {
        statName.text = name;
        statValue1.text = value1;
        statValues2.text = value2;
        percentTxt.text = percent;
    }
}
