using UnityEngine;
using UnityEngine.UI;

public class TipPanel : MonoBehaviour
{
    [SerializeField]private GameObject _tipBox;//提示框
    [SerializeField]private Text _tipText;//提示文字

    /// <summary>
    /// 显示提示
    /// </summary>
    /// <param name="tip"></param>
    public void ShowTip(string tip)
    {
        _tipBox.SetActive(true);
        _tipText.text = tip;
        TimerUtility.Instance.Timer(1, ()=>{ gameObject.SetActive(false);});
    }

    
}
