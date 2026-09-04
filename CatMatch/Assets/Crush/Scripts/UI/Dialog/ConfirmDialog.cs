using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmDialog : MonoBehaviour
{
    public Text titleTxt;
    public Text desTxt;

    public void SetUp(string title, string des)
    {
        titleTxt.text = title;
        desTxt.text = des;
    }
}
