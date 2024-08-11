using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleGroupInitializer : MonoBehaviour
{
    public ToggleGroup toggleGroup;


    void OnEnable()
    {
        // 手动调用更新状态
        foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
        {
            if (toggle.isOn)
            {
                toggle.GetComponent<Image>().color=Color.white;
            }
        }
    }

    public void OnToggleValueChanged(Toggle toggle)
    {
        toggle.GetComponent<Image>().color = toggle.isOn? Color.white: Color.grey;
    }
}
