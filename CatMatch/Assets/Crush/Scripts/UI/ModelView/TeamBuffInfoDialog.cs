using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TeamBuffInfoDialog : MonoBehaviour
{
    public Text titleTxt;
    public TextMeshProUGUI desTxt;

    public void SetUp(string title, string des)
    {
        titleTxt.text = title;
        desTxt.text = des;
    }
}
